using github.api.net.Models.Common;
using github.api.net.Models.Search;
using github.api.net.Models.Issues;
using github.api.net.Models.PullRequests;
using github.api.net.Models.Gists;

namespace github.api.net.tests.TestData;

public static class TestDataFactory
{
    public static Repository CreateTestRepository(string owner = "testowner", string name = "testrepo")
    {
        return new Repository
        {
            Id = 12345,
            NodeId = "MDEwOlJlcG9zaXRvcnkxMjM0NQ==",
            Name = name,
            FullName = $"{owner}/{name}",
            Owner = CreateTestUser(owner),
            Private = false,
            HtmlUrl = $"https://github.com/{owner}/{name}",
            Description = "Test repository description",
            Fork = false,
            Url = $"https://api.github.com/repos/{owner}/{name}",
            CreatedAt = DateTimeOffset.Now.AddDays(-30),
            UpdatedAt = DateTimeOffset.Now.AddHours(-1),
            PushedAt = DateTimeOffset.Now.AddHours(-2),
            Size = 1024,
            StargazersCount = 42,
            WatchersCount = 42,
            Language = "C#",
            ForksCount = 5,
            OpenIssuesCount = 3,
            DefaultBranch = "main",
            Topics = new List<string> { "csharp", "dotnet" },
            HasIssues = true,
            HasProjects = true,
            HasWiki = true,
            HasPages = false,
            HasDownloads = true,
            Archived = false,
            Disabled = false,
            Visibility = "public"
        };
    }

    public static User CreateTestUser(string login = "testuser")
    {
        return new User
        {
            Login = login,
            Id = 67890,
            NodeId = "MDQ6VXNlcjY3ODkw",
            AvatarUrl = $"https://avatars.githubusercontent.com/u/67890?v=4",
            Url = $"https://api.github.com/users/{login}",
            HtmlUrl = $"https://github.com/{login}",
            Type = "User",
            SiteAdmin = false,
            Name = $"{char.ToUpper(login[0])}{login[1..]} User",
            Company = "Test Company",
            Location = "Test City",
            Email = $"{login}@example.com",
            Bio = "Test user bio",
            TwitterUsername = login,
            PublicRepos = 10,
            PublicGists = 5,
            Followers = 25,
            Following = 30,
            CreatedAt = DateTimeOffset.Now.AddYears(-2),
            UpdatedAt = DateTimeOffset.Now.AddDays(-1)
        };
    }

    public static RepositorySearchResult CreateTestRepositorySearchResult()
    {
        return new RepositorySearchResult
        {
            TotalCount = 100,
            IncompleteResults = false,
            Items = new List<Repository>
            {
                CreateTestRepository("microsoft", "vscode"),
                CreateTestRepository("dotnet", "runtime"),
                CreateTestRepository("github", "copilot")
            }
        };
    }

    public static Issue CreateTestIssue(int number = 1, string state = "open")
    {
        return new Issue
        {
            Id = 1000000 + number,
            NodeId = $"MDU6SXNzdWUxMDAwMDAw{number}",
            Number = number,
            Title = $"Test Issue #{number}",
            User = CreateTestUser(),
            Labels = new List<IssueLabel>
            {
                new IssueLabel { Id = 1, Name = "bug", Color = "d73a4a" }
            },
            State = state,
            Locked = false,
            Comments = 5,
            CreatedAt = DateTimeOffset.Now.AddDays(-7),
            UpdatedAt = DateTimeOffset.Now.AddHours(-2),
            ClosedAt = state == "closed" ? DateTimeOffset.Now.AddHours(-1) : null,
            Body = "This is a test issue body",
            HtmlUrl = $"https://github.com/owner/repo/issues/{number}",
            Url = $"https://api.github.com/repos/owner/repo/issues/{number}",
            RepositoryUrl = "https://api.github.com/repos/owner/repo"
        };
    }

    public static PullRequest CreateTestPullRequest(int number = 1, string state = "open")
    {
        return new PullRequest
        {
            Id = 2000000 + number,
            NodeId = $"MDExOlB1bGxSZXF1ZXN0MjAwMDAwMA=={number}",
            Number = number,
            State = state,
            Locked = false,
            Title = $"Test PR #{number}",
            User = CreateTestUser(),
            Body = "Test pull request body",
            Labels = new List<IssueLabel>(),
            CreatedAt = DateTimeOffset.Now.AddDays(-3),
            UpdatedAt = DateTimeOffset.Now.AddHours(-1),
            ClosedAt = state == "closed" ? DateTimeOffset.Now.AddHours(-1) : null,
            MergedAt = state == "closed" ? DateTimeOffset.Now.AddHours(-1) : null,
            Head = new PullRequestBranch
            {
                Label = "testuser:feature-branch",
                Ref = "feature-branch",
                Sha = "abc123def456",
                User = CreateTestUser()
            },
            Base = new PullRequestBranch
            {
                Label = "owner:main",
                Ref = "main",
                Sha = "def456abc123",
                User = CreateTestUser("owner")
            },
            Draft = false,
            Merged = state == "closed",
            Mergeable = true,
            MergeableState = "clean",
            Comments = 3,
            ReviewComments = 2,
            Commits = 5,
            Additions = 150,
            Deletions = 50,
            ChangedFiles = 10,
            HtmlUrl = $"https://github.com/owner/repo/pull/{number}",
            DiffUrl = $"https://github.com/owner/repo/pull/{number}.diff",
            PatchUrl = $"https://github.com/owner/repo/pull/{number}.patch",
            Url = $"https://api.github.com/repos/owner/repo/pulls/{number}"
        };
    }

    public static Gist CreateTestGist(string id = "abc123def456")
    {
        return new Gist
        {
            Id = id,
            NodeId = "MDQ6R2lzdGFiYzEyM2RlZjQ1Ng==",
            Url = $"https://api.github.com/gists/{id}",
            ForksUrl = $"https://api.github.com/gists/{id}/forks",
            CommitsUrl = $"https://api.github.com/gists/{id}/commits",
            GitPullUrl = $"https://gist.github.com/{id}.git",
            GitPushUrl = $"https://gist.github.com/{id}.git",
            HtmlUrl = $"https://gist.github.com/{id}",
            Files = new Dictionary<string, GistFile>
            {
                ["example.cs"] = new GistFile
                {
                    Filename = "example.cs",
                    Type = "application/x-csharp",
                    Language = "C#",
                    RawUrl = $"https://gist.githubusercontent.com/testuser/{id}/raw/example.cs",
                    Size = 256,
                    Content = "Console.WriteLine(\"Hello, World!\");"
                }
            },
            Public = true,
            CreatedAt = DateTimeOffset.Now.AddDays(-10),
            UpdatedAt = DateTimeOffset.Now.AddHours(-3),
            Description = "Test gist",
            Comments = 2,
            User = CreateTestUser(),
            Owner = CreateTestUser(),
            CommentsUrl = $"https://api.github.com/gists/{id}/comments",
            Truncated = false
        };
    }
}
