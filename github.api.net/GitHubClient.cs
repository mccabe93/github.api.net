using System.Net.Http.Headers;
using System.Text.Json;
using github.api.net.Clients;
using github.api.net.Configuration;

namespace github.api.net;

public sealed class GitHubClient : IDisposable
{
    private readonly HttpClient _httpClient;
    public SearchClient Search { get; }
    public RepositoriesClient Repositories { get; }
    public UsersClient Users { get; }
    public IssuesClient Issues { get; }
    public PullRequestsClient PullRequests { get; }
    public GistsClient Gists { get; }

    public GitHubClient(GitHubClientOptions? githubClientOptions = null)
    {
        GitHubClientOptions options = githubClientOptions ?? new GitHubClientOptions();
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
            Timeout = options.Timeout,
        };

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github+json")
        );
        _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

        if (!string.IsNullOrEmpty(options.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                options.Token
            );
        }

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System
                .Text
                .Json
                .Serialization
                .JsonIgnoreCondition
                .WhenWritingNull,
        };

        Search = new SearchClient(_httpClient, jsonOptions);
        Repositories = new RepositoriesClient(_httpClient, jsonOptions);
        Users = new UsersClient(_httpClient, jsonOptions);
        Issues = new IssuesClient(_httpClient, jsonOptions);
        PullRequests = new PullRequestsClient(_httpClient, jsonOptions);
        Gists = new GistsClient(_httpClient, jsonOptions);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
