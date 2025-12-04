using System.Text;
using System.Text.Json;
using github.api.net.Models.Common;
using github.api.net.Models.Users;

namespace github.api.net.Clients;

public class UsersClient : BaseClient
{
    public UsersClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        : base(httpClient, jsonOptions) { }

    /// <summary>
    /// Get the authenticated user
    /// </summary>
    public async Task<GitHubResponse<User>> GetAuthenticatedUserAsync(
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/user");
        return await SendAsync<User>(request, cancellationToken);
    }

    /// <summary>
    /// Get a user
    /// </summary>
    public async Task<GitHubResponse<User>> GetUserAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/users/{username}");
        return await SendAsync<User>(request, cancellationToken);
    }

    /// <summary>
    /// List users
    /// </summary>
    public async Task<GitHubResponse<List<User>>> ListUsersAsync(
        long? since = null,
        int perPage = 30,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(since?.ToString(), perPage.ToString(), null);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/users?{queryParams}");
        return await SendAsync<List<User>>(request, cancellationToken);
    }

    /// <summary>
    /// List followers for a user
    /// </summary>
    public async Task<GitHubResponse<List<User>>> ListFollowersAsync(
        string username,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/users/{username}/followers?{queryParams}"
        );
        return await SendAsync<List<User>>(request, cancellationToken);
    }

    /// <summary>
    /// List users followed by another user
    /// </summary>
    public async Task<GitHubResponse<List<User>>> ListFollowingAsync(
        string username,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/users/{username}/following?{queryParams}"
        );
        return await SendAsync<List<User>>(request, cancellationToken);
    }

    /// <summary>
    /// Check if a user follows another user
    /// </summary>
    public async Task<GitHubResponse<string>> CheckFollowingAsync(
        string username,
        string targetUser,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/users/{username}/following/{targetUser}"
        );
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List email addresses for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<List<UserEmail>>> ListEmailsAsync(
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());
        var request = new HttpRequestMessage(HttpMethod.Get, $"/user/emails?{queryParams}");
        return await SendAsync<List<UserEmail>>(request, cancellationToken);
    }

    /// <summary>
    /// List SSH keys for a user
    /// </summary>
    public async Task<GitHubResponse<List<SshKey>>> ListSshKeysForUserAsync(
        string username,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/users/{username}/keys?{queryParams}"
        );
        return await SendAsync<List<SshKey>>(request, cancellationToken);
    }

    /// <summary>
    /// List SSH keys for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<List<SshKey>>> ListMySshKeysAsync(
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());
        var request = new HttpRequestMessage(HttpMethod.Get, $"/user/keys?{queryParams}");
        return await SendAsync<List<SshKey>>(request, cancellationToken);
    }

    /// <summary>
    /// Create an SSH key for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<SshKey>> CreateSshKeyAsync(
        CreateSshKeyRequest keyRequest,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(keyRequest, JsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, "/user/keys")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

        return await SendAsync<SshKey>(request, cancellationToken);
    }

    /// <summary>
    /// Delete an SSH key for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<string>> DeleteSshKeyAsync(
        long keyId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/user/keys/{keyId}");
        return await SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// List GPG keys for a user
    /// </summary>
    public async Task<GitHubResponse<List<GpgKey>>> ListGpgKeysForUserAsync(
        string username,
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/users/{username}/gpg_keys?{queryParams}"
        );
        return await SendAsync<List<GpgKey>>(request, cancellationToken);
    }

    /// <summary>
    /// List GPG keys for the authenticated user
    /// </summary>
    public async Task<GitHubResponse<List<GpgKey>>> ListMyGpgKeysAsync(
        int perPage = 30,
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = GetPaginationQueryString(null, perPage.ToString(), page.ToString());
        var request = new HttpRequestMessage(HttpMethod.Get, $"/user/gpg_keys?{queryParams}");
        return await SendAsync<List<GpgKey>>(request, cancellationToken);
    }
}
