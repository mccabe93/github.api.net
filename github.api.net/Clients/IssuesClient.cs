using System.Text;
using System.Text.Json;
using github.api.net.Models.Common;
using github.api.net.Models.Issues;

namespace github.api.net.Clients;

public sealed class IssuesClient : BaseClient
{
    public IssuesClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        : base(httpClient, jsonOptions) { }

    /// <summary>
    /// List issues for a repository
    /// </summary>
    public async Task<GitHubResponse<List<Issue>>> ListRepositoryIssuesAsync(
        string owner,
        string repo,
        string? state = null,
        string? labels = null,
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
                ["state"] = state,
                ["labels"] = labels,
                ["sort"] = sort,
                ["direction"] = direction,
                ["since"] = since?.ToString("o"),
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/issues?{queryParams}"
        );
        return await SendAsync<List<Issue>>(request, cancellationToken);
    }

    /// <summary>
    /// List all issues assigned to the authenticated user
    /// </summary>
    public async Task<GitHubResponse<List<Issue>>> ListMyIssuesAsync(
        string? filter = null,
        string? state = null,
        string? labels = null,
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
                ["filter"] = filter,
                ["state"] = state,
                ["labels"] = labels,
                ["sort"] = sort,
                ["direction"] = direction,
                ["since"] = since?.ToString("o"),
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(HttpMethod.Get, $"/issues?{queryParams}");
        return await SendAsync<List<Issue>>(request, cancellationToken);
    }

    /// <summary>
    /// Get an issue
    /// </summary>
    public async Task<GitHubResponse<Issue>> GetIssueAsync(
        string owner,
        string repo,
        int issueNumber,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/issues/{issueNumber}"
        );
        return await SendAsync<Issue>(request, cancellationToken);
    }

    /// <summary>
    /// Create an issue
    /// </summary>
    public async Task<GitHubResponse<Issue>> CreateIssueAsync(
        string owner,
        string repo,
        CreateIssueRequest issueRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(issueRequest, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, $"/repos/{owner}/{repo}/issues")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<Issue>(request, cancellationToken);
    }

    /// <summary>
    /// Update an issue
    /// </summary>
    public async Task<GitHubResponse<Issue>> UpdateIssueAsync(
        string owner,
        string repo,
        int issueNumber,
        UpdateIssueRequest updateRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(updateRequest, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/repos/{owner}/{repo}/issues/{issueNumber}"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<Issue>(request, cancellationToken);
    }

    /// <summary>
    /// Lock an issue
    /// </summary>
    public async Task<GitHubResponse<string>> LockIssueAsync(
        string owner,
        string repo,
        int issueNumber,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/repos/{owner}/{repo}/issues/{issueNumber}/lock"
        );
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Unlock an issue
    /// </summary>
    public async Task<GitHubResponse<string>> UnlockIssueAsync(
        string owner,
        string repo,
        int issueNumber,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/repos/{owner}/{repo}/issues/{issueNumber}/lock"
        );
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List comments on an issue
    /// </summary>
    public async Task<GitHubResponse<List<IssueComment>>> ListCommentsAsync(
        string owner,
        string repo,
        int issueNumber,
        DateTimeOffset? since = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(
            since?.ToString("0"),
            perPage.ToString(),
            page.ToString()
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/issues/{issueNumber}/comments?{queryParams}"
        );
        return await SendAsync<List<IssueComment>>(request, cancellationToken);
    }

    /// <summary>
    /// Create a comment on an issue
    /// </summary>
    public async Task<GitHubResponse<IssueComment>> CreateCommentAsync(
        string owner,
        string repo,
        int issueNumber,
        CreateCommentRequest commentRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(commentRequest, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/repos/{owner}/{repo}/issues/{issueNumber}/comments"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<IssueComment>(request, cancellationToken);
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    public async Task<GitHubResponse<IssueComment>> UpdateCommentAsync(
        string owner,
        string repo,
        long commentId,
        CreateCommentRequest commentRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(commentRequest, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/repos/{owner}/{repo}/issues/comments/{commentId}"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<IssueComment>(request, cancellationToken);
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    public async Task<GitHubResponse<string>> DeleteCommentAsync(
        string owner,
        string repo,
        long commentId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/repos/{owner}/{repo}/issues/comments/{commentId}"
        );
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List labels for a repository
    /// </summary>
    public async Task<GitHubResponse<List<IssueLabel>>> ListLabelsAsync(
        string owner,
        string repo,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/labels?{queryParams}"
        );
        return await SendAsync<List<IssueLabel>>(request, cancellationToken);
    }

    /// <summary>
    /// List milestones for a repository
    /// </summary>
    public async Task<GitHubResponse<List<Milestone>>> ListMilestonesAsync(
        string owner,
        string repo,
        string? state = null,
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
                ["sort"] = sort,
                ["direction"] = direction,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/milestones?{queryParams}"
        );
        return await SendAsync<List<Milestone>>(request, cancellationToken);
    }

    /// <summary>
    /// Get a milestone
    /// </summary>
    public async Task<GitHubResponse<Milestone>> GetMilestoneAsync(
        string owner,
        string repo,
        int milestoneNumber,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/milestones/{milestoneNumber}"
        );
        return await SendAsync<Milestone>(request, cancellationToken);
    }
}
