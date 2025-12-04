using System.Net;
using System.Text.Json;
using github.api.net.Clients;
using github.api.net.Models.Gists;
using github.api.net.tests.Base;
using github.api.net.tests.TestData;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace github.api.net.tests.Clients;

[TestClass]
public class GistsClientTests : GitHubApiTestFixture
{
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;
    private GistsClient _gistsClient = null!;
    private JsonSerializerOptions _jsonOptions = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockHandler = CreateMockHttpHandler();
        _httpClient = CreateMockHttpClient(_mockHandler);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _gistsClient = new GistsClient(_httpClient, _jsonOptions);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _httpClient?.Dispose();
    }

    #region List Gists Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListMyGistsAsync_ValidToken_ReturnsGistList()
    {
        // Arrange
        var gists = new List<Gist>
        {
            TestDataFactory.CreateTestGist("abc123"),
            TestDataFactory.CreateTestGist("def456")
        };
        var json = JsonSerializer.Serialize(gists, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.ListMyGistsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListPublicGistsAsync_DefaultParameters_ReturnsGistList()
    {
        // Arrange
        var gists = new List<Gist>
        {
            TestDataFactory.CreateTestGist("public1"),
            TestDataFactory.CreateTestGist("public2")
        };
        var json = JsonSerializer.Serialize(gists, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.ListPublicGistsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.All(g => g.Public).Should().BeTrue();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListStarredGistsAsync_ValidToken_ReturnsStarredGists()
    {
        // Arrange
        var gists = new List<Gist>
        {
            TestDataFactory.CreateTestGist("starred1")
        };
        var json = JsonSerializer.Serialize(gists, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.ListStarredGistsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListUserGistsAsync_ValidUsername_ReturnsUserGists()
    {
        // Arrange
        var gists = new List<Gist>
        {
            TestDataFactory.CreateTestGist("user1"),
            TestDataFactory.CreateTestGist("user2")
        };
        var json = JsonSerializer.Serialize(gists, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.ListUserGistsAsync("octocat");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region GetGistAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetGistAsync_ValidGistId_ReturnsGist()
    {
        // Arrange
        var gist = TestDataFactory.CreateTestGist("abc123def456");
        var json = JsonSerializer.Serialize(gist, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.GetGistAsync("abc123def456");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be("abc123def456");
        result.Data.Files.Should().NotBeEmpty();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetGistAsync_GistNotFound_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound,
            "{\"message\":\"Not Found\"}");

        // Act
        var result = await _gistsClient.GetGistAsync("nonexistent");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    #endregion

    #region CreateGistAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateGistAsync_ValidRequest_ReturnsCreatedGist()
    {
        // Arrange
        var createRequest = new CreateGistRequest
        {
            Description = "Test gist",
            Public = true,
            Files = new Dictionary<string, GistFileContent>
            {
                ["test.cs"] = new GistFileContent
                {
                    Content = "Console.WriteLine(\"Hello\");"
                }
            }
        };
        var createdGist = TestDataFactory.CreateTestGist("newgist123");
        createdGist.Description = "Test gist";
        var json = JsonSerializer.Serialize(createdGist, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _gistsClient.CreateGistAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Description.Should().Be("Test gist");
    }

    #endregion

    #region UpdateGistAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task UpdateGistAsync_ValidRequest_ReturnsUpdatedGist()
    {
        // Arrange
        var updateRequest = new UpdateGistRequest
        {
            Description = "Updated description",
            Files = new Dictionary<string, GistFileUpdate>
            {
                ["test.cs"] = new GistFileUpdate
                {
                    Content = "Console.WriteLine(\"Updated\");"
                }
            }
        };
        var updatedGist = TestDataFactory.CreateTestGist("abc123");
        updatedGist.Description = "Updated description";
        var json = JsonSerializer.Serialize(updatedGist, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.UpdateGistAsync("abc123", updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Description.Should().Be("Updated description");
    }

    #endregion

    #region DeleteGistAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DeleteGistAsync_ValidGistId_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _gistsClient.DeleteGistAsync("abc123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    #endregion

    #region Fork Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ForkGistAsync_ValidGistId_ReturnsForkedGist()
    {
        // Arrange
        var forkedGist = TestDataFactory.CreateTestGist("forked123");
        var json = JsonSerializer.Serialize(forkedGist, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _gistsClient.ForkGistAsync("original123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListForksAsync_ValidGistId_ReturnsForkList()
    {
        // Arrange
        var forks = new List<Gist>
        {
            TestDataFactory.CreateTestGist("fork1"),
            TestDataFactory.CreateTestGist("fork2")
        };
        var json = JsonSerializer.Serialize(forks, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.ListForksAsync("original123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region Star Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task StarGistAsync_ValidGistId_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _gistsClient.StarGistAsync("abc123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task UnstarGistAsync_ValidGistId_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _gistsClient.UnstarGistAsync("abc123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task IsStarredAsync_StarredGist_Returns204()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _gistsClient.IsStarredAsync("abc123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task IsStarredAsync_NotStarredGist_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound, string.Empty);

        // Act
        var result = await _gistsClient.IsStarredAsync("abc123");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    #endregion

    #region Comments Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListCommentsAsync_ValidGistId_ReturnsCommentList()
    {
        // Arrange
        var comments = new List<GistComment>
        {
            new GistComment { Id = 1, Body = "Great gist!" },
            new GistComment { Id = 2, Body = "Thanks for sharing" }
        };
        var json = JsonSerializer.Serialize(comments, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _gistsClient.ListCommentsAsync("abc123");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateCommentAsync_ValidRequest_ReturnsCreatedComment()
    {
        // Arrange
        var commentRequest = new CreateGistCommentRequest
        {
            Body = "This is a test comment"
        };
        var createdComment = new GistComment
        {
            Id = 123,
            Body = "This is a test comment"
        };
        var json = JsonSerializer.Serialize(createdComment, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _gistsClient.CreateCommentAsync("abc123", commentRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Body.Should().Be("This is a test comment");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DeleteCommentAsync_ValidCommentId_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _gistsClient.DeleteCommentAsync("abc123", 123);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    #endregion
}
