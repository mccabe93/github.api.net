using System.Text.Json;
using github.api.net.Models.Common;
using github.api.net.Models.Search;

namespace github.api.net.Clients;

public class SearchClient : BaseClient
{
    public SearchClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        : base(httpClient, jsonOptions) { }

    /// <summary>
    /// Search for repositories
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="sort">Sort by stars, forks, help-wanted-issues, or updated</param>
    /// <param name="order">Sort order: asc or desc</param>
    /// <param name="perPage">Results per page (max 100)</param>
    /// <param name="page">Page number</param>
    public async Task<GitHubResponse<RepositorySearchResult>> SearchRepositoriesAsync(
        string query,
        string? sort = null,
        string? order = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["q"] = query,
                ["sort"] = sort,
                ["order"] = order,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(HttpMethod.Get, $"/search/repositories?{queryParams}");
        return await SendAsync<RepositorySearchResult>(request, cancellationToken);
    }

    /// <summary>
    /// Search for code
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="sort">Sort by indexed</param>
    /// <param name="order">Sort order: asc or desc</param>
    /// <param name="perPage">Results per page (max 100)</param>
    /// <param name="page">Page number</param>
    public async Task<GitHubResponse<CodeSearchResult>> SearchCodeAsync(
        string query,
        string? sort = null,
        string? order = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["q"] = query,
                ["sort"] = sort,
                ["order"] = order,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(HttpMethod.Get, $"/search/code?{queryParams}");
        return await SendAsync<CodeSearchResult>(request, cancellationToken);
    }

    /// <summary>
    /// Search for issues and pull requests
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="sort">Sort by comments, reactions, created, or updated</param>
    /// <param name="order">Sort order: asc or desc</param>
    /// <param name="perPage">Results per page (max 100)</param>
    /// <param name="page">Page number</param>
    public async Task<GitHubResponse<IssueSearchResult>> SearchIssuesAsync(
        string query,
        string? sort = null,
        string? order = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["q"] = query,
                ["sort"] = sort,
                ["order"] = order,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(HttpMethod.Get, $"/search/issues?{queryParams}");
        return await SendAsync<IssueSearchResult>(request, cancellationToken);
    }

    /// <summary>
    /// Search for users
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="sort">Sort by followers, repositories, or joined</param>
    /// <param name="order">Sort order: asc or desc</param>
    /// <param name="perPage">Results per page (max 100)</param>
    /// <param name="page">Page number</param>
    public async Task<GitHubResponse<UserSearchResult>> SearchUsersAsync(
        string query,
        string? sort = null,
        string? order = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["q"] = query,
                ["sort"] = sort,
                ["order"] = order,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(HttpMethod.Get, $"/search/users?{queryParams}");
        return await SendAsync<UserSearchResult>(request, cancellationToken);
    }

    /// <summary>
    /// Search for commits
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="sort">Sort by author-date or committer-date</param>
    /// <param name="order">Sort order: asc or desc</param>
    /// <param name="perPage">Results per page (max 100)</param>
    /// <param name="page">Page number</param>
    public async Task<GitHubResponse<CommitSearchResult>> SearchCommitsAsync(
        string query,
        string? sort = null,
        string? order = null,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = BuildQueryString(
            new Dictionary<string, string?>
            {
                ["q"] = query,
                ["sort"] = sort,
                ["order"] = order,
                ["per_page"] = perPage.ToString(),
                ["page"] = page.ToString(),
            }
        );

        var request = new HttpRequestMessage(HttpMethod.Get, $"/search/commits?{queryParams}");
        return await SendAsync<CommitSearchResult>(request, cancellationToken);
    }
}
