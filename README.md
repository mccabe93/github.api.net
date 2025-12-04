# github.api.net

.NET 10 library for interacting with the GitHub REST API.

## Features

### Search API
- Search repositories, code, issues, users, and commits
- Full support for pagination and sorting

### Repositories API
- Get, create, update, and delete repositories
- List user repositories
- Manage branches, tags, and releases
- View contributors and repository content
- 13 endpoints implemented

### Users API
- Get user information (authenticated and public)
- Manage followers
- Email addresses management
- SSH and GPG keys management
- 13 endpoints implemented

### Issues API
- List, create, update, and manage issues
- Lock/unlock issues
- Comment management
- Labels and milestones support
- 14 endpoints implemented

### Pull Requests API
- List, create, update pull requests
- Merge pull requests
- Review management
- Review comments
- File changes tracking
- 14 endpoints implemented

### Gists API
- Create, update, delete gists
- Fork and star gists
- Comment management
- List commits and forks
- 20 endpoints implemented

## Installation

```bash
# Add reference to your project
dotnet add reference path/to/github.api.net.csproj
```

## Usage

### Basic Setup

```csharp
using github.api.net;
using github.api.net.Configuration;

// Anonymous access
var client = new GitHubClient();

// Authenticated access
var client = new GitHubClient(new GitHubClientOptions
{
    Token = "your-github-token"
});
```

### Search Examples

```csharp
// Search repositories
var repoSearch = await client.Search.SearchRepositoriesAsync(
    query: "language:csharp stars:>1000",
    sort: "stars",
    order: "desc"
);

if (repoSearch.IsSuccess)
{
    foreach (var repo in repoSearch.Data.Items)
    {
        Console.WriteLine($"{repo.FullName} - ? {repo.StargazersCount}");
    }
}

// Search code
var codeSearch = await client.Search.SearchCodeAsync(
    query: "HttpClient in:file language:csharp repo:dotnet/runtime"
);

// Search issues
var issueSearch = await client.Search.SearchIssuesAsync(
    query: "is:open is:issue label:bug repo:dotnet/runtime"
);

// Search users
var userSearch = await client.Search.SearchUsersAsync(
    query: "location:Seattle followers:>100"
);

// Search commits
var commitSearch = await client.Search.SearchCommitsAsync(
    query: "fix bug repo:dotnet/runtime"
);
```

### Repository Examples

```csharp
// Get a repository
var repo = await client.Repositories.GetRepositoryAsync("dotnet", "runtime");

// List user repositories
var repos = await client.Repositories.ListUserRepositoriesAsync("octocat");

// Create a repository
var newRepo = await client.Repositories.CreateRepositoryAsync(
    new CreateRepositoryRequest
    {
        Name = "my-new-repo",
        Description = "A test repository",
        Private = false,
        AutoInit = true
    }
);

// List branches
var branches = await client.Repositories.ListBranchesAsync("owner", "repo");

// Get latest release
var release = await client.Repositories.GetLatestReleaseAsync("owner", "repo");

// List contributors
var contributors = await client.Repositories.ListContributorsAsync("dotnet", "runtime");
```

### User Examples

```csharp
// Get authenticated user
var me = await client.Users.GetAuthenticatedUserAsync();

// Get a specific user
var user = await client.Users.GetUserAsync("octocat");

// List followers
var followers = await client.Users.ListFollowersAsync("octocat");

// List user's SSH keys
var keys = await client.Users.ListSshKeysForUserAsync("octocat");
```

### Issues Examples

```csharp
// List repository issues
var issues = await client.Issues.ListRepositoryIssuesAsync(
    "owner", 
    "repo",
    state: "open",
    labels: "bug,enhancement"
);

// Create an issue
var newIssue = await client.Issues.CreateIssueAsync(
    "owner",
    "repo",
    new CreateIssueRequest
    {
        Title = "Bug Report",
        Body = "Description of the bug",
        Labels = new List<string> { "bug" }
    }
);

// Add a comment
var comment = await client.Issues.CreateCommentAsync(
    "owner",
    "repo",
    issueNumber: 1,
    new CreateCommentRequest { Body = "Great idea!" }
);

// Update an issue
var updated = await client.Issues.UpdateIssueAsync(
    "owner",
    "repo",
    issueNumber: 1,
    new UpdateIssueRequest { State = "closed" }
);
```

### Pull Requests Examples

```csharp
// List pull requests
var prs = await client.PullRequests.ListPullRequestsAsync(
    "owner",
    "repo",
    state: "open"
);

// Create a pull request
var newPr = await client.PullRequests.CreatePullRequestAsync(
    "owner",
    "repo",
    new CreatePullRequestRequest
    {
        Title = "Add new feature",
        Head = "feature-branch",
        Base = "main",
        Body = "This PR adds..."
    }
);

// Merge a pull request
var merged = await client.PullRequests.MergePullRequestAsync(
    "owner",
    "repo",
    pullNumber: 42,
    mergeMethod: "squash"
);

// List reviews
var reviews = await client.PullRequests.ListReviewsAsync("owner", "repo", 42);

// Create a review
var review = await client.PullRequests.CreateReviewAsync(
    "owner",
    "repo",
    pullNumber: 42,
    new CreateReviewRequest
    {
        Event = "APPROVE",
        Body = "LGTM!"
    }
);
```

### Gists Examples

```csharp
// List public gists
var gists = await client.Gists.ListPublicGistsAsync();

// Create a gist
var newGist = await client.Gists.CreateGistAsync(
    new CreateGistRequest
    {
        Description = "Example gist",
        Public = true,
        Files = new Dictionary<string, GistFileContent>
        {
            ["example.cs"] = new GistFileContent
            {
                Content = "Console.WriteLine(\"Hello, World!\");"
            }
        }
    }
);

// Star a gist
await client.Gists.StarGistAsync(gistId);

// Fork a gist
var forked = await client.Gists.ForkGistAsync(gistId);
```

## Response Handling

All API methods return a `GitHubResponse<T>` object that includes:

```csharp
var response = await client.Repositories.GetRepositoryAsync("owner", "repo");

if (response.IsSuccess)
{
    var repo = response.Data;
    Console.WriteLine($"Repository: {repo.FullName}");
    
    // Rate limit information
    if (response.RateLimit != null)
    {
        Console.WriteLine($"Rate Limit: {response.RateLimit.Remaining}/{response.RateLimit.Limit}");
        Console.WriteLine($"Resets at: {response.RateLimit.Reset}");
    }
}
else
{
    Console.WriteLine($"Error {response.StatusCode}: {response.ErrorMessage}");
}
```

## Configuration Options

```csharp
var options = new GitHubClientOptions
{
    BaseUrl = "https://api.github.com",  // Default
    Token = "your-token",                // Optional authentication
    UserAgent = "github.api.net",        // Default user agent
    Timeout = TimeSpan.FromSeconds(30),  // Request timeout
    MaxRetries = 3                       // Retry count (not yet implemented)
};

var client = new GitHubClient(options);
```

## Authentication

For authenticated requests, create a Personal Access Token in GitHub:

1. Go to GitHub Settings ? Developer settings ? Personal access tokens
2. Generate a new token with appropriate scopes
3. Pass the token to the `GitHubClientOptions`

```csharp
var client = new GitHubClient(new GitHubClientOptions
{
    Token = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
});
```

## Rate Limiting

GitHub API has rate limits:
- **Unauthenticated**: 60 requests/hour
- **Authenticated**: 5,000 requests/hour

Check rate limit information in the response:

```csharp
if (response.RateLimit != null)
{
    Console.WriteLine($"Remaining: {response.RateLimit.Remaining}");
    Console.WriteLine($"Resets: {response.RateLimit.Reset}");
}
```

## License

This project is open source under the MIT License. See the [LICENSE](LICENSE) file for details.
## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

## Additional Resources

- [GitHub REST API Documentation](https://docs.github.com/en/rest)
- [GitHub API Best Practices](https://docs.github.com/en/rest/guides/best-practices-for-integrators)
