using System.Text.Json.Serialization;
using github.api.net.Models.Common;

namespace github.api.net.Models.Gists;

public class Gist
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("forks_url")]
    public string ForksUrl { get; set; } = string.Empty;

    [JsonPropertyName("commits_url")]
    public string CommitsUrl { get; set; } = string.Empty;

    [JsonPropertyName("git_pull_url")]
    public string GitPullUrl { get; set; } = string.Empty;

    [JsonPropertyName("git_push_url")]
    public string GitPushUrl { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("files")]
    public Dictionary<string, GistFile> Files { get; set; } = new();

    [JsonPropertyName("public")]
    public bool Public { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("comments")]
    public int Comments { get; set; }

    [JsonPropertyName("user")]
    public User? User { get; set; }

    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public User? Owner { get; set; }

    [JsonPropertyName("truncated")]
    public bool Truncated { get; set; }
}

public class GistFile
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("raw_url")]
    public string RawUrl { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("truncated")]
    public bool? Truncated { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class GistComment
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}

public class GistCommit
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("change_status")]
    public GistChangeStatus ChangeStatus { get; set; } = new();

    [JsonPropertyName("committed_at")]
    public DateTimeOffset CommittedAt { get; set; }
}

public class GistChangeStatus
{
    [JsonPropertyName("deletions")]
    public int Deletions { get; set; }

    [JsonPropertyName("additions")]
    public int Additions { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class CreateGistRequest
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("public")]
    public bool Public { get; set; }

    [JsonPropertyName("files")]
    public Dictionary<string, GistFileContent> Files { get; set; } = new();
}

public class GistFileContent
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class UpdateGistRequest
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("files")]
    public Dictionary<string, GistFileUpdate>? Files { get; set; }
}

public class GistFileUpdate
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("filename")]
    public string? Filename { get; set; }
}

public class CreateGistCommentRequest
{
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}
