using System.Text;
using System.Text.Json;
using github.api.net.Models.Common;
using github.api.net.Models.PullRequests;

namespace github.api.net.Clients;

public class PullRequestsClient : BaseClient
{
    public PullRequestsClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        : base(httpClient, jsonOptions) { }

    /// <summary>
    /// List pull requests
    /// </summary>
    public async Task<GitHubResponse<List<PullRequest>>> ListPullRequestsAsync(
        string owner,
        string repo,
        string? state = null,
        string? head = null,
        string? baseRef = null,
        string? sort = null,
        string? direction = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["state"] = state,
                ["head"] = head,
                ["base"] = baseRef,
                ["sort"] = sort,
                ["direction"] = direction,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls?{queryParams}"
        );
        return await SendAsync<List<PullRequest>>(request, cancellationToken);
    }

    /// <summary>
    /// Get a pull request
    /// </summary>
    public async Task<GitHubResponse<PullRequest>> GetPullRequestAsync(
        string owner,
        string repo,
        int pullNumber,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}"
        );
        return await SendAsync<PullRequest>(request, cancellationToken);
    }

    /// <summary>
    /// Create a pull request
    /// </summary>
    public async Task<GitHubResponse<PullRequest>> CreatePullRequestAsync(
        string owner,
        string repo,
        CreatePullRequestRequest pullRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(pullRequest, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, $"/repos/{owner}/{repo}/pulls")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<PullRequest>(request, cancellationToken);
    }

    /// <summary>
    /// Update a pull request
    /// </summary>
    public async Task<GitHubResponse<PullRequest>> UpdatePullRequestAsync(
        string owner,
        string repo,
        int pullNumber,
        UpdatePullRequestRequest updateRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(updateRequest, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<PullRequest>(request, cancellationToken);
    }

    /// <summary>
    /// List commits on a pull request
    /// </summary>
    public async Task<GitHubResponse<List<PullRequestFile>>> ListFilesAsync(
        string owner,
        string repo,
        int pullNumber,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/files?{queryParams}"
        );
        return await SendAsync<List<PullRequestFile>>(request, cancellationToken);
    }

    /// <summary>
    /// Check if a pull request has been merged
    /// </summary>
    public async Task<GitHubResponse<string>> IsMergedAsync(
        string owner,
        string repo,
        int pullNumber,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/merge"
        );
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Merge a pull request
    /// </summary>
    public async Task<GitHubResponse<string>> MergePullRequestAsync(
        string owner,
        string repo,
        int pullNumber,
        string? commitTitle = null,
        string? commitMessage = null,
        string? mergeMethod = null,
        CancellationToken cancellationToken = default
    )
    {
        var body = new Dictionary<string, string?>();
        if (!string.IsNullOrEmpty(commitTitle))
            body["commit_title"] = commitTitle;
        if (!string.IsNullOrEmpty(commitMessage))
            body["commit_message"] = commitMessage;
        if (!string.IsNullOrEmpty(mergeMethod))
            body["merge_method"] = mergeMethod;

        var json = JsonSerializer.Serialize(body, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/merge"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List reviews for a pull request
    /// </summary>
    public async Task<GitHubResponse<List<PullRequestReview>>> ListReviewsAsync(
        string owner,
        string repo,
        int pullNumber,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/reviews?{queryParams}"
        );
        return await SendAsync<List<PullRequestReview>>(request, cancellationToken);
    }

    /// <summary>
    /// Get a review for a pull request
    /// </summary>
    public async Task<GitHubResponse<PullRequestReview>> GetReviewAsync(
        string owner,
        string repo,
        int pullNumber,
        long reviewId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/reviews/{reviewId}"
        );
        return await SendAsync<PullRequestReview>(request, cancellationToken);
    }

    /// <summary>
    /// Create a review for a pull request
    /// </summary>
    public async Task<GitHubResponse<PullRequestReview>> CreateReviewAsync(
        string owner,
        string repo,
        int pullNumber,
        CreateReviewRequest reviewRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(reviewRequest, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/reviews"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<PullRequestReview>(request, cancellationToken);
    }

    /// <summary>
    /// List review comments on a pull request
    /// </summary>
    public async Task<GitHubResponse<List<ReviewComment>>> ListReviewCommentsAsync(
        string owner,
        string repo,
        int pullNumber,
        string? sort = null,
        string? direction = null,
        DateTimeOffset? since = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["sort"] = sort,
                ["direction"] = direction,
                ["since"] = since?.ToString("o"),
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/comments?{queryParams}"
        );
        return await SendAsync<List<ReviewComment>>(request, cancellationToken);
    }

    /// <summary>
    /// Get a review comment for a pull request
    /// </summary>
    public async Task<GitHubResponse<ReviewComment>> GetReviewCommentAsync(
        string owner,
        string repo,
        long commentId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/pulls/comments/{commentId}"
        );
        return await SendAsync<ReviewComment>(request, cancellationToken);
    }

    /// <summary>
    /// Create a review comment on a pull request
    /// </summary>
    public async Task<GitHubResponse<ReviewComment>> CreateReviewCommentAsync(
        string owner,
        string repo,
        int pullNumber,
        ReviewCommentDraft comment,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(comment, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/repos/{owner}/{repo}/pulls/{pullNumber}/comments"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<ReviewComment>(request, cancellationToken);
    }

    /// <summary>
    /// Delete a review comment
    /// </summary>
    public async Task<GitHubResponse<string>> DeleteReviewCommentAsync(
        string owner,
        string repo,
        long commentId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/repos/{owner}/{repo}/pulls/comments/{commentId}"
        );
        return await SendAsync(request, cancellationToken);
    }
}
