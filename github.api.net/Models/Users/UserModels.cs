using System.Text.Json.Serialization;

namespace github.api.net.Models.Users;

public class UserEmail
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("visibility")]
    public string? Visibility { get; set; }
}

public class SshKey
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("read_only")]
    public bool ReadOnly { get; set; }
}

public class GpgKey
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("primary_key_id")]
    public long? PrimaryKeyId { get; set; }

    [JsonPropertyName("key_id")]
    public string KeyId { get; set; } = string.Empty;

    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; } = string.Empty;

    [JsonPropertyName("emails")]
    public List<GpgEmail> Emails { get; set; } = new();

    [JsonPropertyName("can_sign")]
    public bool CanSign { get; set; }

    [JsonPropertyName("can_encrypt_comms")]
    public bool CanEncryptComms { get; set; }

    [JsonPropertyName("can_encrypt_storage")]
    public bool CanEncryptStorage { get; set; }

    [JsonPropertyName("can_certify")]
    public bool CanCertify { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }
}

public class GpgEmail
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }
}

public class CreateSshKeyRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
}
