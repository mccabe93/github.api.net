using System.Net;
using System.Text.Json;
using github.api.net.Clients;
using github.api.net.Models.Common;
using github.api.net.Models.Users;
using github.api.net.tests.Base;
using github.api.net.tests.TestData;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace github.api.net.tests.Clients;

[TestClass]
public class UsersClientTests : GitHubApiTestFixture
{
    private Mock<HttpMessageHandler> _mockHandler = null!;
    private HttpClient _httpClient = null!;
    private UsersClient _usersClient = null!;
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
        _usersClient = new UsersClient(_httpClient, _jsonOptions);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _httpClient?.Dispose();
    }

    #region GetAuthenticatedUserAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetAuthenticatedUserAsync_ValidToken_ReturnsUser()
    {
        // Arrange
        var expectedUser = TestDataFactory.CreateTestUser("authenticateduser");
        var json = JsonSerializer.Serialize(expectedUser, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _usersClient.GetAuthenticatedUserAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Login.Should().Be("authenticateduser");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetAuthenticatedUserAsync_InvalidToken_Returns401()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.Unauthorized,
            "{\"message\":\"Bad credentials\"}");

        // Act
        var result = await _usersClient.GetAuthenticatedUserAsync();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
    }

    #endregion

    #region GetUserAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetUserAsync_ValidUsername_ReturnsUser()
    {
        // Arrange
        var expectedUser = TestDataFactory.CreateTestUser("octocat");
        var json = JsonSerializer.Serialize(expectedUser, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _usersClient.GetUserAsync("octocat");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Login.Should().Be("octocat");
        result.Data.Name.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task GetUserAsync_UserNotFound_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound,
            "{\"message\":\"Not Found\"}");

        // Act
        var result = await _usersClient.GetUserAsync("nonexistentuser");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    #endregion

    #region ListUsersAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListUsersAsync_DefaultParameters_ReturnsUserList()
    {
        // Arrange
        var users = new List<User>
        {
            TestDataFactory.CreateTestUser("user1"),
            TestDataFactory.CreateTestUser("user2"),
            TestDataFactory.CreateTestUser("user3")
        };
        var json = JsonSerializer.Serialize(users, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _usersClient.ListUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListUsersAsync_WithSinceParameter_IncludesInQuery()
    {
        // Arrange
        var users = new List<User>();
        var json = JsonSerializer.Serialize(users, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        await _usersClient.ListUsersAsync(since: 12345);

        // Assert
        _mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("since=12345")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    #endregion

    #region ListFollowersAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListFollowersAsync_ValidUsername_ReturnsFollowerList()
    {
        // Arrange
        var followers = new List<User>
        {
            TestDataFactory.CreateTestUser("follower1"),
            TestDataFactory.CreateTestUser("follower2")
        };
        var json = JsonSerializer.Serialize(followers, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _usersClient.ListFollowersAsync("octocat");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region ListFollowingAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListFollowingAsync_ValidUsername_ReturnsFollowingList()
    {
        // Arrange
        var following = new List<User>
        {
            TestDataFactory.CreateTestUser("following1"),
            TestDataFactory.CreateTestUser("following2")
        };
        var json = JsonSerializer.Serialize(following, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _usersClient.ListFollowingAsync("octocat");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    #endregion

    #region CheckFollowingAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CheckFollowingAsync_UserFollowsTarget_Returns204()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _usersClient.CheckFollowingAsync("user1", "user2");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CheckFollowingAsync_UserDoesNotFollow_Returns404()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NotFound, string.Empty);

        // Act
        var result = await _usersClient.CheckFollowingAsync("user1", "user2");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    #endregion

    #region ListEmailsAsync Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListEmailsAsync_ValidToken_ReturnsEmailList()
    {
        // Arrange
        var emails = new List<UserEmail>
        {
            new UserEmail { Email = "primary@example.com", Primary = true, Verified = true },
            new UserEmail { Email = "secondary@example.com", Primary = false, Verified = true }
        };
        var json = JsonSerializer.Serialize(emails, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _usersClient.ListEmailsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(e => e.Primary);
    }

    #endregion

    #region SSH Keys Tests

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ListSshKeysForUserAsync_ValidUsername_ReturnsSshKeyList()
    {
        // Arrange
        var keys = new List<SshKey>
        {
            new SshKey { Id = 1, Title = "Work Laptop", Verified = true },
            new SshKey { Id = 2, Title = "Personal Desktop", Verified = true }
        };
        var json = JsonSerializer.Serialize(keys, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

        // Act
        var result = await _usersClient.ListSshKeysForUserAsync("octocat");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateSshKeyAsync_ValidRequest_ReturnsCreatedKey()
    {
        // Arrange
        var keyRequest = new CreateSshKeyRequest
        {
            Title = "New Key",
            Key = "ssh-rsa AAAAB3NzaC1yc2E..."
        };
        var createdKey = new SshKey { Id = 123, Title = "New Key", Verified = false };
        var json = JsonSerializer.Serialize(createdKey, _jsonOptions);
        SetupMockResponse(_mockHandler, HttpStatusCode.Created, json);

        // Act
        var result = await _usersClient.CreateSshKeyAsync(keyRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Title.Should().Be("New Key");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DeleteSshKeyAsync_ValidKeyId_ReturnsSuccess()
    {
        // Arrange
        SetupMockResponse(_mockHandler, HttpStatusCode.NoContent, string.Empty);

        // Act
        var result = await _usersClient.DeleteSshKeyAsync(123);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    #endregion
}
