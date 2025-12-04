using System.Net;
using System.Text.Json;
using github.api.net.Clients;
using github.api.net.Models.Common;
using github.api.net.Models.Repositories;
using github.api.net.tests.Base;
using github.api.net.tests.TestData;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace github.api.net.tests.Clients;

[TestClass]
public class RepositoriesClientTests : GitHubApiTestFixture
{
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;
    private RepositoriesClient _repositoriesClient = null!;
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
        _repositoriesClient = new RepositoriesClient(_httpClient, _jsonOptions);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _httpClient?.Dispose();
    }

    #region GetRepositoryAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetRepositoryAsync_ValidOwnerAndRepo_ReturnsRepository()
    {
        // Arrange
        var expectedRepo = TestDataFactory.CreateTestRepository("dotnet", "runtime");
        var json = JsonSerializer.Serialize(expectedRepo, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _repositoriesClient.GetRepositoryAsync("dotnet", "runtime");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("runtime");
        result.Data.Owner.Login.Should().Be("dotnet");
        result.Data.FullName.Should().Be("dotnet/runtime");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetRepositoryAsync_RepositoryNotFound_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound,
            "{\"message\":\"Not Found\"}");

        // Act
        var result = await _repositoriesClient.GetRepositoryAsync("owner", "nonexistent");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Data.Should().BeNull();
    }

    #endregion

    #region ListUserRepositoriesAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListUserRepositoriesAsync_ValidUsername_ReturnsRepositoryList()
    {
        // Arrange
        var repos = new List<Repository>
        {
            TestDataFactory.CreateTestRepository("octocat", "repo1"),
            TestDataFactory.CreateTestRepository("octocat", "repo2")
        };
        var json = JsonSerializer.Serialize(repos, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _repositoriesClient.ListUserRepositoriesAsync("octocat");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.All(r => r.Owner.Login == "octocat").Should().BeTrue();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListUserRepositoriesAsync_WithFilters_IncludesQueryParameters()
    {
        // Arrange
        var repos = new List<Repository>();
        var json = JsonSerializer.Serialize(repos, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        await _repositoriesClient.ListUserRepositoriesAsync(
            "octocat",
            type: "owner",
            sort: "updated",
            direction: "desc",
            perPage: 50,
            page: 1);

        // Assert
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("type=owner") &&
                req.RequestUri!.ToString().Contains("sort=updated") &&
                req.RequestUri!.ToString().Contains("direction=desc")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    #endregion

    #region CreateRepositoryAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateRepositoryAsync_ValidRequest_ReturnsCreatedRepository()
    {
        // Arrange
        var createRequest = new CreateRepositoryRequest
        {
            Name = "new-repo",
            Description = "A test repository",
            Private = false,
            AutoInit = true
        };
        var expectedRepo = TestDataFactory.CreateTestRepository("testuser", "new-repo");
        var json = JsonSerializer.Serialize(expectedRepo, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _repositoriesClient.CreateRepositoryAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Name.Should().Be("new-repo");
    }

    #endregion

    #region DeleteRepositoryAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DeleteRepositoryAsync_ValidRepository_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _repositoriesClient.DeleteRepositoryAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    #endregion

    #region ListBranchesAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListBranchesAsync_ValidRepository_ReturnsBranchList()
    {
        // Arrange
        var branches = new List<Branch>
        {
            new Branch { Name = "main", Protected = true },
            new Branch { Name = "develop", Protected = false }
        };
        var json = JsonSerializer.Serialize(branches, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _repositoriesClient.ListBranchesAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(b => b.Name == "main");
    }

    #endregion

    #region ListReleasesAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListReleasesAsync_ValidRepository_ReturnsReleaseList()
    {
        // Arrange
        var releases = new List<Release>();
        var json = JsonSerializer.Serialize(releases, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _repositoriesClient.ListReleasesAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    #endregion

    #region GetLatestReleaseAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetLatestReleaseAsync_ValidRepository_ReturnsLatestRelease()
    {
        // Arrange
        var release = new Release
        {
            Id = 1,
            TagName = "v1.0.0",
            Name = "Version 1.0.0",
            Prerelease = false,
            Draft = false
        };
        var json = JsonSerializer.Serialize(release, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _repositoriesClient.GetLatestReleaseAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.TagName.Should().Be("v1.0.0");
    }

    #endregion

    #region ListContributorsAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListContributorsAsync_ValidRepository_ReturnsContributorList()
    {
        // Arrange
        var contributors = new List<Contributor>
        {
            new Contributor { Login = "user1", Contributions = 100 },
            new Contributor { Login = "user2", Contributions = 50 }
        };
        var json = JsonSerializer.Serialize(contributors, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _repositoriesClient.ListContributorsAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(c => c.Login == "user1");
    }

    #endregion
}
