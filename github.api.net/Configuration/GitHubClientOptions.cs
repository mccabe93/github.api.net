namespace github.api.net.Configuration;

public class GitHubClientOptions
{
    public string BaseUrl { get; set; } = "https://api.github.com";
    public string? Token { get; set; }
    public string? UserAgent { get; set; } = "github.api.net";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; set; } = 3;
}
