using System.Text;
using System.Text.Json;
using github.api.net.Models.Common;
using github.api.net.Models.Repositories;

namespace github.api.net.Clients;

public class RepositoriesClient : BaseClient
{
    public RepositoriesClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        : base(httpClient, jsonOptions) { }

    /// <summary>
    /// Get a repository
    /// </summary>
    public async Task<GitHubResponse<Repository>> GetRepositoryAsync(
        string owner,
        string repo,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/repos/{owner}/{repo}");
        return await SendAsync<Repository>(request, cancellationToken);
    }

    /// <summary>
    /// List repositories for a user
    /// </summary>
    public async Task<GitHubResponse<List<Repository>>> ListUserRepositoriesAsync(
        string username,
        string? type = null,
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
                ["type"] = type,
                ["sort"] = sort,
                ["direction"] = direction,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/users/{username}/repos?{queryParams}"
        );
        return await SendAsync<List<Repository>>(request, cancellationToken);
    }

    /// <summary>
    /// List repositories for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<List<Repository>>> ListMyRepositoriesAsync(
        string? visibility = null,
        string? affiliation = null,
        string? type = null,
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
                ["visibility"] = visibility,
                ["affiliation"] = affiliation,
                ["type"] = type,
                ["sort"] = sort,
                ["direction"] = direction,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(HttpMethod.Get, $"/user/repos?{queryParams}");
        return await SendAsync<List<Repository>>(request, cancellationToken);
    }

    /// <summary>
    /// Create a repository for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<Repository>> CreateRepositoryAsync(
        CreateRepositoryRequest repositoryRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(repositoryRequest, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, "/user/repos")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<Repository>(request, cancellationToken);
    }

    /// <summary>
    /// Delete a repository
    /// </summary>
    public async Task<GitHubResponse<string>> DeleteRepositoryAsync(
        string owner,
        string repo,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/repos/{owner}/{repo}");
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List branches
    /// </summary>
    public async Task<GitHubResponse<List<Branch>>> ListBranchesAsync(
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
            $"/repos/{owner}/{repo}/branches?{queryParams}"
        );
        return await SendAsync<List<Branch>>(request, cancellationToken);
    }

    /// <summary>
    /// Get a branch
    /// </summary>
    public async Task<GitHubResponse<Branch>> GetBranchAsync(
        string owner,
        string repo,
        string branch,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/branches/{branch}"
        );
        return await SendAsync<Branch>(request, cancellationToken);
    }

    /// <summary>
    /// List tags
    /// </summary>
    public async Task<GitHubResponse<List<Tag>>> ListTagsAsync(
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
            $"/repos/{owner}/{repo}/tags?{queryParams}"
        );
        return await SendAsync<List<Tag>>(request, cancellationToken);
    }

    /// <summary>
    /// List releases
    /// </summary>
    public async Task<GitHubResponse<List<Release>>> ListReleasesAsync(
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
            $"/repos/{owner}/{repo}/releases?{queryParams}"
        );
        return await SendAsync<List<Release>>(request, cancellationToken);
    }

    /// <summary>
    /// Get the latest release
    /// </summary>
    public async Task<GitHubResponse<Release>> GetLatestReleaseAsync(
        string owner,
        string repo,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/repos/{owner}/{repo}/releases/latest"
        );
        return await SendAsync<Release>(request, cancellationToken);
    }

    /// <summary>
    /// List contributors
    /// </summary>
    public async Task<GitHubResponse<List<Contributor>>> ListContributorsAsync(
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
            $"/repos/{owner}/{repo}/contributors?{queryParams}"
        );
        return await SendAsync<List<Contributor>>(request, cancellationToken);
    }

    /// <summary>
    /// Get repository content
    /// </summary>
    public async Task<GitHubResponse<RepositoryContent>> GetContentAsync(
        string owner,
        string repo,
        string path,
        string? reference = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(new Dictionary<string, string?> { ["ref"] = reference });

        var url = $"/repos/{owner}/{repo}/contents/{path}";
        if (!string.IsNullOrEmpty(queryParams))
        {
            url += $"?{queryParams}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<RepositoryContent>(request, cancellationToken);
    }

    /// <summary>
    /// List repository contents (directory)
    /// </summary>
    public async Task<GitHubResponse<List<RepositoryContent>>> ListContentsAsync(
        string owner,
        string repo,
        string path = "",
        string? reference = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(new Dictionary<string, string?> { ["ref"] = reference });

        var url = $"/repos/{owner}/{repo}/contents/{path}";
        if (!string.IsNullOrEmpty(queryParams))
        {
            url += $"?{queryParams}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<List<RepositoryContent>>(request, cancellationToken);
    }
}
