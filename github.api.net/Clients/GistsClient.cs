using System.Text;
using System.Text.Json;
using github.api.net.Models.Common;
using github.api.net.Models.Gists;

namespace github.api.net.Clients;

public sealed class GistsClient : BaseClient
{
    public GistsClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        : base(httpClient, jsonOptions) { }

    /// <summary>
    /// List gists for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<List<Gist>>> ListMyGistsAsync(
        DateTimeOffset? since = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(
            since?.ToString("o"),
            perPage.ToString(),
            page.ToString()
        );
        var request = new HttpRequestMessage(HttpMethod.Get, $"/gists?{queryParams}");
        return await SendAsync<List<Gist>>(request, cancellationToken);
    }

    /// <summary>
    /// List public gists
    /// </summary>
    public async Task<GitHubResponse<List<Gist>>> ListPublicGistsAsync(
        DateTimeOffset? since = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(
            since?.ToString("o"),
            perPage.ToString(),
            page.ToString()
        );
        var request = new HttpRequestMessage(HttpMethod.Get, $"/gists/public?{queryParams}");
        return await SendAsync<List<Gist>>(request, cancellationToken);
    }

    /// <summary>
    /// List starred gists
    /// </summary>
    public async Task<GitHubResponse<List<Gist>>> ListStarredGistsAsync(
        DateTimeOffset? since = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(
            since?.ToString("o"),
            perPage.ToString(),
            page.ToString()
        );
        var request = new HttpRequestMessage(HttpMethod.Get, $"/gists/starred?{queryParams}");
        return await SendAsync<List<Gist>>(request, cancellationToken);
    }

    /// <summary>
    /// List gists for a user
    /// </summary>
    public async Task<GitHubResponse<List<Gist>>> ListUserGistsAsync(
        string username,
        DateTimeOffset? since = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(
            since?.ToString("o"),
            perPage.ToString(),
            page.ToString()
        );
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/users/{username}/gists?{queryParams}"
        );
        return await SendAsync<List<Gist>>(request, cancellationToken);
    }

    /// <summary>
    /// Get a gist
    /// </summary>
    public async Task<GitHubResponse<Gist>> GetGistAsync(
        string gistId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/gists/{gistId}");
        return await SendAsync<Gist>(request, cancellationToken);
    }

    /// <summary>
    /// Create a gist
    /// </summary>
    public async Task<GitHubResponse<Gist>> CreateGistAsync(
        CreateGistRequest gistRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(gistRequest, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, "/gists")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<Gist>(request, cancellationToken);
    }

    /// <summary>
    /// Update a gist
    /// </summary>
    public async Task<GitHubResponse<Gist>> UpdateGistAsync(
        string gistId,
        UpdateGistRequest updateRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(updateRequest, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Patch, $"/gists/{gistId}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<Gist>(request, cancellationToken);
    }

    /// <summary>
    /// Delete a gist
    /// </summary>
    public async Task<GitHubResponse<string>> DeleteGistAsync(
        string gistId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/gists/{gistId}");
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List commits for a gist
    /// </summary>
    public async Task<GitHubResponse<List<GistCommit>>> ListCommitsAsync(
        string gistId,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/gists/{gistId}/commits?{queryParams}"
        );
        return await SendAsync<List<GistCommit>>(request, cancellationToken);
    }

    /// <summary>
    /// Fork a gist
    /// </summary>
    public async Task<GitHubResponse<Gist>> ForkGistAsync(
        string gistId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/gists/{gistId}/forks");
        return await SendAsync<Gist>(request, cancellationToken);
    }

    /// <summary>
    /// List forks for a gist
    /// </summary>
    public async Task<GitHubResponse<List<Gist>>> ListForksAsync(
        string gistId,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/gists/{gistId}/forks?{queryParams}"
        );
        return await SendAsync<List<Gist>>(request, cancellationToken);
    }

    /// <summary>
    /// Star a gist
    /// </summary>
    public async Task<GitHubResponse<string>> StarGistAsync(
        string gistId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"/gists/{gistId}/star");
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Unstar a gist
    /// </summary>
    public async Task<GitHubResponse<string>> UnstarGistAsync(
        string gistId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/gists/{gistId}/star");
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Check if a gist is starred
    /// </summary>
    public async Task<GitHubResponse<string>> IsStarredAsync(
        string gistId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/gists/{gistId}/star");
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List comments on a gist
    /// </summary>
    public async Task<GitHubResponse<List<GistComment>>> ListCommentsAsync(
        string gistId,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/gists/{gistId}/comments?{queryParams}"
        );
        return await SendAsync<List<GistComment>>(request, cancellationToken);
    }

    /// <summary>
    /// Get a comment on a gist
    /// </summary>
    public async Task<GitHubResponse<GistComment>> GetCommentAsync(
        string gistId,
        long commentId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/gists/{gistId}/comments/{commentId}"
        );
        return await SendAsync<GistComment>(request, cancellationToken);
    }

    /// <summary>
    /// Create a comment on a gist
    /// </summary>
    public async Task<GitHubResponse<GistComment>> CreateCommentAsync(
        string gistId,
        CreateGistCommentRequest commentRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(commentRequest, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, $"/gists/{gistId}/comments")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<GistComment>(request, cancellationToken);
    }

    /// <summary>
    /// Update a comment on a gist
    /// </summary>
    public async Task<GitHubResponse<GistComment>> UpdateCommentAsync(
        string gistId,
        long commentId,
        CreateGistCommentRequest commentRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(commentRequest, JsonOptions);
        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/gists/{gistId}/comments/{commentId}"
        )
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<GistComment>(request, cancellationToken);
    }

    /// <summary>
    /// Delete a comment on a gist
    /// </summary>
    public async Task<GitHubResponse<string>> DeleteCommentAsync(
        string gistId,
        long commentId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/gists/{gistId}/comments/{commentId}"
        );
        return await SendAsync(request, cancellationToken);
    }
}
