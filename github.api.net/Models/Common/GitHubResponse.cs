namespace github.api.net.Models.Common;

public class GitHubResponse<T>
{
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;
    public string? ErrorMessage { get; set; }
    public RateLimit? RateLimit { get; set; }
}

public class RateLimit
{
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public DateTimeOffset Reset { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public bool IncompleteResults { get; set; }
}
