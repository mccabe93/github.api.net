using System.Net;
using System.Text.Json;
using github.api.net.Clients;
using github.api.net.Models.Search;
using github.api.net.tests.Base;
using github.api.net.tests.TestData;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace github.api.net.tests.Clients;

[TestClass]
public class SearchClientTests : GitHubApiTestFixture
{
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;
    private SearchClient _searchClient = null!;
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
        _searchClient = new SearchClient(_httpClient, _jsonOptions);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _httpClient?.Dispose();
    }

    #region SearchRepositoriesAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchRepositoriesAsync_ValidQuery_ReturnsRepositorySearchResult()
    {
        // Arrange
        var expectedResult = TestDataFactory.CreateTestRepositorySearchResult();
        var json = JsonSerializer.Serialize(expectedResult, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _searchClient.SearchRepositoriesAsync("test query");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(100);
        result.Data.IncompleteResults.Should().BeFalse();
        result.Data.Items.Should().HaveCount(3);
        result.RateLimit.Should().NotBeNull();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchRepositoriesAsync_WithPagination_IncludesQueryParameters()
    {
        // Arrange
        var expectedResult = TestDataFactory.CreateTestRepositorySearchResult();
        var json = JsonSerializer.Serialize(expectedResult, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _searchClient.SearchRepositoriesAsync(
            "test",
            sort: "stars",
            order: "desc",
            perPage: 50,
            page: 2);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify the request was made with correct parameters
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("q=test") &&
                req.RequestUri!.ToString().Contains("sort=stars") &&
                req.RequestUri!.ToString().Contains("order=desc") &&
                req.RequestUri!.ToString().Contains("per_page=50") &&
                req.RequestUri!.ToString().Contains("page=2")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchRepositoriesAsync_ApiError_ReturnsErrorResponse()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.Unauthorized,
            "{\"message\":\"Bad credentials\"}");

        // Act
        var result = await _searchClient.SearchRepositoriesAsync("test");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.ErrorMessage.Should().Contain("Bad credentials");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchRepositoriesAsync_RateLimitExceeded_Returns403()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.Forbidden,
            "{\"message\":\"API rate limit exceeded\"}");

        // Act
        var result = await _searchClient.SearchRepositoriesAsync("test");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(403);
        result.ErrorMessage.Should().Contain("rate limit");
    }

    #endregion

    #region SearchCodeAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchCodeAsync_ValidQuery_ReturnsCodeSearchResult()
    {
        // Arrange
        var expectedResult = new CodeSearchResult
        {
            TotalCount = 10,
            IncompleteResults = false,
            Items = new List<CodeItem>()
        };
        var json = JsonSerializer.Serialize(expectedResult, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _searchClient.SearchCodeAsync("HttpClient");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(10);
    }

    #endregion

    #region SearchIssuesAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchIssuesAsync_ValidQuery_ReturnsIssueSearchResult()
    {
        // Arrange
        var expectedResult = new IssueSearchResult
        {
            TotalCount = 25,
            IncompleteResults = false,
            Items = new List<IssueItem>()
        };
        var json = JsonSerializer.Serialize(expectedResult, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _searchClient.SearchIssuesAsync("is:issue is:open");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(25);
    }

    #endregion

    #region SearchUsersAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchUsersAsync_ValidQuery_ReturnsUserSearchResult()
    {
        // Arrange
        var expectedResult = new UserSearchResult
        {
            TotalCount = 5,
            IncompleteResults = false,
            Items = new List<Models.Common.User>
            {
                TestDataFactory.CreateTestUser("user1"),
                TestDataFactory.CreateTestUser("user2")
            }
        };
        var json = JsonSerializer.Serialize(expectedResult, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _searchClient.SearchUsersAsync("location:Seattle");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(5);
        result.Data.Items.Should().HaveCount(2);
    }

    #endregion

    #region SearchCommitsAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SearchCommitsAsync_ValidQuery_ReturnsCommitSearchResult()
    {
        // Arrange
        var expectedResult = new CommitSearchResult
        {
            TotalCount = 15,
            IncompleteResults = false,
            Items = new List<CommitItem>()
        };
        var json = JsonSerializer.Serialize(expectedResult, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _searchClient.SearchCommitsAsync("fix bug");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(15);
    }

    #endregion
}
