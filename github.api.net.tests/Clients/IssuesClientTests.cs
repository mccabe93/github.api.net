using System.Net;
using System.Text.Json;
using github.api.net.Clients;
using github.api.net.Models.Issues;
using github.api.net.tests.Base;
using github.api.net.tests.TestData;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace github.api.net.tests.Clients;

[TestClass]
public class IssuesClientTests : GitHubApiTestFixture
{
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;
    private IssuesClient _issuesClient = null!;
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
        _issuesClient = new IssuesClient(_httpClient, _jsonOptions);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _httpClient?.Dispose();
    }

    #region ListRepositoryIssuesAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListRepositoryIssuesAsync_ValidRepository_ReturnsIssueList()
    {
        // Arrange
        var issues = new List<Issue>
        {
            TestDataFactory.CreateTestIssue(1, "open"),
            TestDataFactory.CreateTestIssue(2, "open"),
            TestDataFactory.CreateTestIssue(3, "closed")
        };
        var json = JsonSerializer.Serialize(issues, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _issuesClient.ListRepositoryIssuesAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
        result.Data.Should().Contain(i => i.Number == 1);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListRepositoryIssuesAsync_WithFilters_IncludesQueryParameters()
    {
        // Arrange
        var issues = new List<Issue>();
        var json = JsonSerializer.Serialize(issues, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        await _issuesClient.ListRepositoryIssuesAsync(
            "owner",
            "repo",
            state: "open",
            labels: "bug,enhancement",
            sort: "created",
            direction: "desc");

        // Assert
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("state=open") &&
                req.RequestUri!.ToString().Contains("labels=bug") &&
                req.RequestUri!.ToString().Contains("sort=created")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    #endregion

    #region GetIssueAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetIssueAsync_ValidIssueNumber_ReturnsIssue()
    {
        // Arrange
        var issue = TestDataFactory.CreateTestIssue(42, "open");
        var json = JsonSerializer.Serialize(issue, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _issuesClient.GetIssueAsync("owner", "repo", 42);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Number.Should().Be(42);
        result.Data.State.Should().Be("open");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetIssueAsync_IssueNotFound_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound,
            "{\"message\":\"Not Found\"}");

        // Act
        var result = await _issuesClient.GetIssueAsync("owner", "repo", 999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    #endregion

    #region CreateIssueAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateIssueAsync_ValidRequest_ReturnsCreatedIssue()
    {
        // Arrange
        var createRequest = new CreateIssueRequest
        {
            Title = "New Bug Report",
            Body = "Description of the bug",
            Labels = new List<string> { "bug" }
        };
        var createdIssue = TestDataFactory.CreateTestIssue(1, "open");
        createdIssue.Title = "New Bug Report";
        var json = JsonSerializer.Serialize(createdIssue, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _issuesClient.CreateIssueAsync("owner", "repo", createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Title.Should().Be("New Bug Report");
    }

    #endregion

    #region UpdateIssueAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task UpdateIssueAsync_ValidRequest_ReturnsUpdatedIssue()
    {
        // Arrange
        var updateRequest = new UpdateIssueRequest
        {
            State = "closed",
            Title = "Updated Title"
        };
        var updatedIssue = TestDataFactory.CreateTestIssue(1, "closed");
        updatedIssue.Title = "Updated Title";
        var json = JsonSerializer.Serialize(updatedIssue, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _issuesClient.UpdateIssueAsync("owner", "repo", 1, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.State.Should().Be("closed");
        result.Data.Title.Should().Be("Updated Title");
    }

    #endregion

    #region LockIssueAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task LockIssueAsync_ValidIssue_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _issuesClient.LockIssueAsync("owner", "repo", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    #endregion

    #region UnlockIssueAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task UnlockIssueAsync_ValidIssue_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _issuesClient.UnlockIssueAsync("owner", "repo", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    #endregion

    #region Comments Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListCommentsAsync_ValidIssue_ReturnsCommentList()
    {
        // Arrange
        var comments = new List<IssueComment>
        {
            new IssueComment { Id = 1, Body = "First comment" },
            new IssueComment { Id = 2, Body = "Second comment" }
        };
        var json = JsonSerializer.Serialize(comments, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _issuesClient.ListCommentsAsync("owner", "repo", 1);

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
        var commentRequest = new CreateCommentRequest
        {
            Body = "This is a test comment"
        };
        var createdComment = new IssueComment
        {
            Id = 123,
            Body = "This is a test comment"
        };
        var json = JsonSerializer.Serialize(createdComment, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _issuesClient.CreateCommentAsync("owner", "repo", 1, commentRequest);

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
        var result = await _issuesClient.DeleteCommentAsync("owner", "repo", 123);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    #endregion

    #region Labels and Milestones Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListLabelsAsync_ValidRepository_ReturnsLabelList()
    {
        // Arrange
        var labels = new List<IssueLabel>
        {
            new IssueLabel { Id = 1, Name = "bug", Color = "d73a4a" },
            new IssueLabel { Id = 2, Name = "enhancement", Color = "a2eeef" }
        };
        var json = JsonSerializer.Serialize(labels, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _issuesClient.ListLabelsAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(l => l.Name == "bug");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListMilestonesAsync_ValidRepository_ReturnsMilestoneList()
    {
        // Arrange
        var milestones = new List<Milestone>
        {
            new Milestone { Id = 1, Number = 1, Title = "v1.0", State = "open" },
            new Milestone { Id = 2, Number = 2, Title = "v2.0", State = "open" }
        };
        var json = JsonSerializer.Serialize(milestones, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _issuesClient.ListMilestonesAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion
}
