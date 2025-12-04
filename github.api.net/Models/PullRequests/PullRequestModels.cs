using System.Text.Json.Serialization;
using github.api.net.Models.Common;
using github.api.net.Models.Issues;

namespace github.api.net.Models.PullRequests;

public class PullRequest
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("labels")]
    public List<IssueLabel> Labels { get; set; } = new();

    [JsonPropertyName("milestone")]
    public Milestone? Milestone { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("closed_at")]
    public DateTimeOffset? ClosedAt { get; set; }

    [JsonPropertyName("merged_at")]
    public DateTimeOffset? MergedAt { get; set; }

    [JsonPropertyName("merge_commit_sha")]
    public string? MergeCommitSha { get; set; }

    [JsonPropertyName("assignee")]
    public User? Assignee { get; set; }

    [JsonPropertyName("assignees")]
    public List<User> Assignees { get; set; } = new();

    [JsonPropertyName("requested_reviewers")]
    public List<User> RequestedReviewers { get; set; } = new();

    [JsonPropertyName("head")]
    public PullRequestBranch Head { get; set; } = new();

    [JsonPropertyName("base")]
    public PullRequestBranch Base { get; set; } = new();

    [JsonPropertyName("draft")]
    public bool Draft { get; set; }

    [JsonPropertyName("merged")]
    public bool Merged { get; set; }

    [JsonPropertyName("mergeable")]
    public bool? Mergeable { get; set; }

    [JsonPropertyName("mergeable_state")]
    public string MergeableState { get; set; } = string.Empty;

    [JsonPropertyName("merged_by")]
    public User? MergedBy { get; set; }

    [JsonPropertyName("comments")]
    public int Comments { get; set; }

    [JsonPropertyName("review_comments")]
    public int ReviewComments { get; set; }

    [JsonPropertyName("commits")]
    public int Commits { get; set; }

    [JsonPropertyName("additions")]
    public int Additions { get; set; }

    [JsonPropertyName("deletions")]
    public int Deletions { get; set; }

    [JsonPropertyName("changed_files")]
    public int ChangedFiles { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("diff_url")]
    public string DiffUrl { get; set; } = string.Empty;

    [JsonPropertyName("patch_url")]
    public string PatchUrl { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class PullRequestBranch
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("ref")]
    public string Ref { get; set; } = string.Empty;

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("repo")]
    public Repository? Repo { get; set; }
}

public class PullRequestReview
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("pull_request_url")]
    public string PullRequestUrl { get; set; } = string.Empty;

    [JsonPropertyName("submitted_at")]
    public DateTimeOffset? SubmittedAt { get; set; }

    [JsonPropertyName("commit_id")]
    public string CommitId { get; set; } = string.Empty;
}

public class ReviewComment
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("pull_request_review_id")]
    public long? PullRequestReviewId { get; set; }

    [JsonPropertyName("diff_hunk")]
    public string DiffHunk { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("original_position")]
    public int OriginalPosition { get; set; }

    [JsonPropertyName("commit_id")]
    public string CommitId { get; set; } = string.Empty;

    [JsonPropertyName("original_commit_id")]
    public string OriginalCommitId { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public User User { get; set; } = new();

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
}

public class PullRequestFile
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("additions")]
    public int Additions { get; set; }

    [JsonPropertyName("deletions")]
    public int Deletions { get; set; }

    [JsonPropertyName("changes")]
    public int Changes { get; set; }

    [JsonPropertyName("blob_url")]
    public string BlobUrl { get; set; } = string.Empty;

    [JsonPropertyName("raw_url")]
    public string RawUrl { get; set; } = string.Empty;

    [JsonPropertyName("contents_url")]
    public string ContentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("patch")]
    public string? Patch { get; set; }
}

public class CreatePullRequestRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("head")]
    public string Head { get; set; } = string.Empty;

    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("maintainer_can_modify")]
    public bool? MaintainerCanModify { get; set; }

    [JsonPropertyName("draft")]
    public bool? Draft { get; set; }
}

public class UpdatePullRequestRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("base")]
    public string? Base { get; set; }

    [JsonPropertyName("maintainer_can_modify")]
    public bool? MaintainerCanModify { get; set; }
}

public class CreateReviewRequest
{
    [JsonPropertyName("commit_id")]
    public string? CommitId { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    [JsonPropertyName("comments")]
    public List<ReviewCommentDraft>? Comments { get; set; }
}

public class ReviewCommentDraft
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("line")]
    public int? Line { get; set; }

    [JsonPropertyName("side")]
    public string? Side { get; set; }

    [JsonPropertyName("start_line")]
    public int? StartLine { get; set; }

    [JsonPropertyName("start_side")]
    public string? StartSide { get; set; }
}
