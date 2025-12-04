using FluentAssertions;
using github.api.net.tests.Base;

namespace github.api.net.tests.Integration;

[TestClass]
[TestCategory("Integration")]
public class GitHubApiIntegrationTests : GitHubApiTestFixture
{
    [TestMethod]
    public async Task EndToEnd_SearchAndGetRepository_WorksCorrectly()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive(
                "Real API testing not enabled. Set GitHub:UseRealApi to true in appsettings.json"
            );
        }

        // Arrange
        var client = CreateClient();

        // Act - Search for repositories
        var searchResult = await client.Search.SearchRepositoriesAsync(
            "language:csharp",
            sort: "stars",
            order: "desc",
            perPage: 5
        );

        // Assert - Search succeeded
        searchResult.Should().NotBeNull();
        searchResult.IsSuccess.Should().BeTrue("search should succeed");
        searchResult.Data.Should().NotBeNull();
        searchResult.Data!.Items.Should().NotBeEmpty("should return at least one repository");

        // Act - Get first repository details
        var firstRepo = searchResult.Data.Items.First();
        var repoResult = await client.Repositories.GetRepositoryAsync(
            firstRepo.Owner.Login,
            firstRepo.Name
        );

        // Assert - Repository retrieval succeeded
        repoResult.IsSuccess.Should().BeTrue("repository get should succeed");
        repoResult.Data.Should().NotBeNull();
        repoResult.Data!.FullName.Should().Be(firstRepo.FullName);
        repoResult.Data.Language.Should().Be("C#");
    }

    [TestMethod]
    public async Task RateLimit_IsTrackedCorrectly()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive("Real API testing not enabled");
        }

        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.Search.SearchRepositoriesAsync("test");

        // Assert
        result.Should().NotBeNull();
        result.RateLimit.Should().NotBeNull("rate limit information should be present");
        result
            .RateLimit!.Limit.Should()
            .BeGreaterThan(0, "rate limit should have a positive limit");
        result
            .RateLimit.Remaining.Should()
            .BeGreaterThanOrEqualTo(0, "remaining calls should be non-negative");
        result
            .RateLimit.Reset.Should()
            .BeAfter(
                DateTimeOffset.Now.AddHours(-1),
                "reset time should be in the future or recent past"
            );

        // Output rate limit info for diagnostics
        Console.WriteLine($"Rate Limit: {result.RateLimit.Remaining}/{result.RateLimit.Limit}");
        Console.WriteLine($"Resets at: {result.RateLimit.Reset}");
    }

    [TestMethod]
    public async Task GetUser_PublicProfile_WorksCorrectly()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive("Real API testing not enabled");
        }

        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.Users.GetUserAsync("octocat");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Login.Should().Be("octocat");
        result.Data.Type.Should().Be("User");
    }

    [TestMethod]
    public async Task ListIssues_OpenSourceRepository_WorksCorrectly()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive("Real API testing not enabled");
        }

        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.Issues.ListRepositoryIssuesAsync(
            "dotnet",
            "runtime",
            state: "open",
            perPage: 5
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Issues list may be empty if there are no open issues, so we just check it's not null
        result.Data.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetLatestRelease_PopularRepository_WorksCorrectly()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive("Real API testing not enabled");
        }

        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.Repositories.GetLatestReleaseAsync("dotnet", "runtime");

        // Assert
        result.Should().NotBeNull();
        if (result.IsSuccess)
        {
            result.Data.Should().NotBeNull();
            result.Data!.TagName.Should().NotBeNullOrEmpty();
            Console.WriteLine($"Latest release: {result.Data.TagName}");
        }
        else
        {
            // Some repos might not have releases
            Console.WriteLine($"No releases found or error: {result.ErrorMessage}");
        }
    }

    [TestMethod]
    public async Task SearchCode_SpecificPattern_WorksCorrectly()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive("Real API testing not enabled");
        }

        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.Search.SearchCodeAsync(
            "HttpClient in:file language:csharp repo:dotnet/runtime",
            perPage: 5
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetAuthenticatedUser_WithValidToken_ReturnsUserProfile()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive("Real API testing not enabled");
        }

        var token = GitHubToken;
        if (string.IsNullOrEmpty(token))
        {
            Assert.Inconclusive("GitHub token not provided. Set GITHUB_TOKEN environment variable");
        }

        // Arrange
        var client = CreateClient(token);

        // Act
        var result = await client.Users.GetAuthenticatedUserAsync();

        // Assert
        result.Should().NotBeNull();
        result
            .IsSuccess.Should()
            .BeTrue("authenticated user request should succeed with valid token");
        result.Data.Should().NotBeNull();
        result.Data!.Login.Should().NotBeNullOrEmpty();

        Console.WriteLine($"Authenticated as: {result.Data.Login}");
    }

    [TestMethod]
    public async Task MultipleSequentialCalls_DontExceedRateLimit()
    {
        // Skip if no real API testing is configured
        if (!UseRealApi)
        {
            Assert.Inconclusive("Real API testing not enabled");
        }

        // Arrange
        var client = CreateClient();
        var callCount = 3;
        int? initialRemaining = null;

        // Act & Assert
        for (int i = 0; i < callCount; i++)
        {
            var result = await client.Users.GetUserAsync("octocat");

            result.IsSuccess.Should().BeTrue($"call {i + 1} should succeed");
            result.RateLimit.Should().NotBeNull();

            if (i == 0)
            {
                initialRemaining = result.RateLimit!.Remaining;
            }
            else
            {
                // Each call should decrease the remaining count
                result
                    .RateLimit!.Remaining.Should()
                    .BeLessThan(
                        initialRemaining!.Value,
                        "subsequent calls should decrease rate limit"
                    );
            }

            Console.WriteLine($"Call {i + 1}: Remaining = {result.RateLimit!.Remaining}");
        }
    }
}
