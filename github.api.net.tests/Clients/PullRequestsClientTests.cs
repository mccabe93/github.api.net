using System.Net;
using System.Text.Json;
using github.api.net.Clients;
using github.api.net.Models.PullRequests;
using github.api.net.tests.Base;
using github.api.net.tests.TestData;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace github.api.net.tests.Clients;

[TestClass]
public class PullRequestsClientTests : GitHubApiTestFixture
{
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;
    private PullRequestsClient _pullRequestsClient = null!;
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
        _pullRequestsClient = new PullRequestsClient(_httpClient, _jsonOptions);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _httpClient?.Dispose();
    }

    #region ListPullRequestsAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListPullRequestsAsync_ValidRepository_ReturnsPullRequestList()
    {
        // Arrange
        var prs = new List<PullRequest>
        {
            TestDataFactory.CreateTestPullRequest(1, "open"),
            TestDataFactory.CreateTestPullRequest(2, "open"),
            TestDataFactory.CreateTestPullRequest(3, "closed")
        };
        var json = JsonSerializer.Serialize(prs, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _pullRequestsClient.ListPullRequestsAsync("owner", "repo");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
        result.Data.Should().Contain(pr => pr.Number == 1);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListPullRequestsAsync_WithFilters_IncludesQueryParameters()
    {
        // Arrange
        var prs = new List<PullRequest>();
        var json = JsonSerializer.Serialize(prs, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        await _pullRequestsClient.ListPullRequestsAsync(
            "owner",
            "repo",
            state: "open",
            head: "feature-branch",
            baseRef: "main",
            sort: "created",
            direction: "desc");

        // Assert
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("state=open") &&
                req.RequestUri!.ToString().Contains("head=feature-branch") &&
                req.RequestUri!.ToString().Contains("base=main")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    #endregion

    #region GetPullRequestAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetPullRequestAsync_ValidPullNumber_ReturnsPullRequest()
    {
        // Arrange
        var pr = TestDataFactory.CreateTestPullRequest(42, "open");
        var json = JsonSerializer.Serialize(pr, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _pullRequestsClient.GetPullRequestAsync("owner", "repo", 42);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Number.Should().Be(42);
        result.Data.State.Should().Be("open");
        result.Data.Head.Should().NotBeNull();
        result.Data.Base.Should().NotBeNull();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetPullRequestAsync_PullRequestNotFound_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound,
            "{\"message\":\"Not Found\"}");

        // Act
        var result = await _pullRequestsClient.GetPullRequestAsync("owner", "repo", 999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    #endregion

    #region CreatePullRequestAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreatePullRequestAsync_ValidRequest_ReturnsCreatedPullRequest()
    {
        // Arrange
        var createRequest = new CreatePullRequestRequest
        {
            Title = "Add new feature",
            Head = "feature-branch",
            Base = "main",
            Body = "This PR adds a new feature",
            Draft = false
        };
        var createdPr = TestDataFactory.CreateTestPullRequest(1, "open");
        createdPr.Title = "Add new feature";
        var json = JsonSerializer.Serialize(createdPr, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _pullRequestsClient.CreatePullRequestAsync("owner", "repo", createRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Title.Should().Be("Add new feature");
    }

    #endregion

    #region UpdatePullRequestAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task UpdatePullRequestAsync_ValidRequest_ReturnsUpdatedPullRequest()
    {
        // Arrange
        var updateRequest = new UpdatePullRequestRequest
        {
            State = "closed",
            Title = "Updated Title"
        };
        var updatedPr = TestDataFactory.CreateTestPullRequest(1, "closed");
        updatedPr.Title = "Updated Title";
        var json = JsonSerializer.Serialize(updatedPr, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _pullRequestsClient.UpdatePullRequestAsync("owner", "repo", 1, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.State.Should().Be("closed");
        result.Data.Title.Should().Be("Updated Title");
    }

    #endregion

    #region ListFilesAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListFilesAsync_ValidPullRequest_ReturnsFileList()
    {
        // Arrange
        var files = new List<PullRequestFile>
        {
            new PullRequestFile
            {
                Filename = "src/file1.cs",
                Status = "modified",
                Additions = 10,
                Deletions = 5,
                Changes = 15
            },
            new PullRequestFile
            {
                Filename = "src/file2.cs",
                Status = "added",
                Additions = 50,
                Deletions = 0,
                Changes = 50
            }
        };
        var json = JsonSerializer.Serialize(files, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _pullRequestsClient.ListFilesAsync("owner", "repo", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(f => f.Status == "added");
    }

    #endregion

    #region MergePullRequestAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task MergePullRequestAsync_ValidPullRequest_ReturnsSuccess()
    {
        // Arrange
        var mergeResponse = "{\"sha\":\"abc123\",\"merged\":true,\"message\":\"Pull Request successfully merged\"}";
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, mergeResponse);

        // Act
        var result = await _pullRequestsClient.MergePullRequestAsync(
            "owner",
            "repo",
            1,
            commitTitle: "Merge PR #1",
            mergeMethod: "squash");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Contain("merged");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task MergePullRequestAsync_Conflict_Returns405()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.MethodNotAllowed,
            "{\"message\":\"Pull Request is not mergeable\"}");

        // Act
        var result = await _pullRequestsClient.MergePullRequestAsync("owner", "repo", 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(405);
    }

    #endregion

    #region IsMergedAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task IsMergedAsync_MergedPullRequest_Returns204()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _pullRequestsClient.IsMergedAsync("owner", "repo", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task IsMergedAsync_NotMergedPullRequest_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound, string.Empty);

        // Act
        var result = await _pullRequestsClient.IsMergedAsync("owner", "repo", 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    #endregion

    #region Reviews Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListReviewsAsync_ValidPullRequest_ReturnsReviewList()
    {
        // Arrange
        var reviews = new List<PullRequestReview>
        {
            new PullRequestReview { Id = 1, State = "APPROVED" },
            new PullRequestReview { Id = 2, State = "CHANGES_REQUESTED" }
        };
        var json = JsonSerializer.Serialize(reviews, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _pullRequestsClient.ListReviewsAsync("owner", "repo", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(r => r.State == "APPROVED");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateReviewAsync_ValidRequest_ReturnsCreatedReview()
    {
        // Arrange
        var reviewRequest = new CreateReviewRequest
        {
            Event = "APPROVE",
            Body = "LGTM!"
        };
        var createdReview = new PullRequestReview
        {
            Id = 123,
            State = "APPROVED",
            Body = "LGTM!"
        };
        var json = JsonSerializer.Serialize(createdReview, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _pullRequestsClient.CreateReviewAsync("owner", "repo", 1, reviewRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.State.Should().Be("APPROVED");
    }

    #endregion

    #region Review Comments Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListReviewCommentsAsync_ValidPullRequest_ReturnsCommentList()
    {
        // Arrange
        var comments = new List<ReviewComment>
        {
            new ReviewComment { Id = 1, Body = "Good catch!" },
            new ReviewComment { Id = 2, Body = "Please fix this" }
        };
        var json = JsonSerializer.Serialize(comments, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _pullRequestsClient.ListReviewCommentsAsync("owner", "repo", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion
}
