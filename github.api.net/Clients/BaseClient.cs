using System.Net.Http.Headers;
using System.Text.Json;
using github.api.net.Models.Common;

namespace github.api.net.Clients;

public abstract class BaseClient
{
    protected readonly HttpClient HttpClient;
    protected readonly JsonSerializerOptions JsonOptions;

    protected BaseClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
    {
        HttpClient = httpClient;
        JsonOptions = jsonOptions;
    }

    protected async Task<GitHubResponse<T>> SendAsync<T>(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = new GitHubResponse<T>
            {
                StatusCode = (int)response.StatusCode,
                RateLimit = ParseRateLimit(response.Headers),
            };

            if (response.IsSuccessStatusCode)
            {
                result.Data = JsonSerializer.Deserialize<T>(content, JsonOptions);
            }
            else
            {
                result.ErrorMessage = content;
            }

            return result;
        }
        catch (Exception ex)
        {
            return new GitHubResponse<T> { StatusCode = 0, ErrorMessage = ex.Message };
        }
    }

    protected async Task<GitHubResponse<string>> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await HttpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            return new GitHubResponse<string>
            {
                StatusCode = (int)response.StatusCode,
                Data = content,
                RateLimit = ParseRateLimit(response.Headers),
                ErrorMessage = response.IsSuccessStatusCode ? null : content,
            };
        }
        catch (Exception ex)
        {
            return new GitHubResponse<string> { StatusCode = 0, ErrorMessage = ex.Message };
        }
    }

    private RateLimit? ParseRateLimit(HttpResponseHeaders headers)
    {
        if (
            headers.TryGetValues("X-RateLimit-Limit", out var limitValues)
            && headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues)
            && headers.TryGetValues("X-RateLimit-Reset", out var resetValues)
        )
        {
            return new RateLimit
            {
                Limit = int.Parse(limitValues.First()),
                Remaining = int.Parse(remainingValues.First()),
                Reset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(resetValues.First())),
            };
        }

        return null;
    }

    protected static string BuildQueryString(Dictionary<string, string?> parameters)
    {
        var queryParams = parameters
            .Where(p => !string.IsNullOrEmpty(p.Value))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}");

        return string.Join("&", queryParams);
    }

    protected static string GetPaginationQueryString(string? since, string? perPage, string? page)
    {
        Dictionary<string, string?> queryParams = new Dictionary<string, string?>();
        if (since != null)
            queryParams["since"] = since;
        if (perPage != null)
            queryParams["per_page"] = perPage;
        if (page != null)
            queryParams["page"] = page;

        return BuildQueryString(queryParams);
    }
}
