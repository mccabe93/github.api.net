using System.Net;
using github.api.net;
using github.api.net.Configuration;
using Moq;
using Moq.Protected;

namespace github.api.net.tests.Base;

public class GitHubApiTestFixture : TestBase
{
    protected GitHubClient CreateClient(string? token = null)
    {
        var options = new GitHubClientOptions
        {
            Token = token ?? GitHubToken,
            Timeout = TimeSpan.FromSeconds(30),
        };

        return new GitHubClient(options);
    }

    protected Mock<HttpMessageHandler> CreateMockHttpHandler()
    {
        return new Mock<HttpMessageHandler>();
    }

    protected HttpClient CreateMockHttpClient(Mock<HttpMessageHandler> handler)
    {
        return new HttpClient(handler.Object) { BaseAddress = new Uri("https://api.github.com") };
    }

    protected void SetupMockResponse(
        Mock<HttpMessageHandler> handler,
        HttpStatusCode statusCode,
        string content,
        Dictionary<string, string>? headers = null
    )
    {
        var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content),
        };

        // Add rate limit headers by default
        response.Headers.Add("X-RateLimit-Limit", "5000");
        response.Headers.Add("X-RateLimit-Remaining", "4999");
        response.Headers.Add(
            "X-RateLimit-Reset",
            DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()
        );

        if (headers != null)
        {
            foreach (var header in headers)
            {
                response.Headers.Add(header.Key, header.Value);
            }
        }

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);
    }

    protected string LoadJsonResponse(string fileName)
    {
        var path = Path.Combine("TestData", "JsonResponseSamples", fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"JSON response file not found: {path}");
        }
        return File.ReadAllText(path);
    }
}
