# GitHub API .NET Tests

This directory contains the comprehensive test suite for the GitHub API .NET library.

## Test Organization

```
github.api.net.tests/
??? Base/                           # Base test infrastructure
?   ??? TestBase.cs                # Base test class with configuration
?   ??? GitHubApiTestFixture.cs    # Mocking and helper methods
??? Clients/                        # Unit tests for API clients
?   ??? SearchClientTests.cs       # Search API tests (8 tests)
?   ??? RepositoriesClientTests.cs # Repositories API tests (10 tests)
?   ??? UsersClientTests.cs        # Users API tests (14 tests)
?   ??? IssuesClientTests.cs       # Issues API tests (14 tests)
?   ??? PullRequestsClientTests.cs # Pull Requests API tests (14 tests)
?   ??? GistsClientTests.cs        # Gists API tests (20 tests)
??? Integration/                    # Integration tests
?   ??? GitHubApiIntegrationTests.cs # End-to-end tests (8 tests)
??? TestData/                       # Test data and sample responses
?   ??? TestDataFactory.cs         # Factory for creating test objects
?   ??? JsonResponseSamples/       # Sample JSON responses from GitHub API
??? Configuration files
    ??? appsettings.json           # Test configuration
    ??? github.api.net.tests.runsettings # Test runner settings
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Only Unit Tests
```bash
dotnet test --filter:"TestCategory=Unit"
```

### Run Only Integration Tests
```bash
dotnet test --filter:"TestCategory=Integration"
```

### Run with Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run with Settings File
```bash
dotnet test --settings github.api.net.tests.runsettings
```

## Test Categories

- **Unit**: Tests that use mocked HTTP responses (default, fast)
- **Integration**: Tests that call the real GitHub API (require token, marked with [Ignore])

## Configuration

### appsettings.json
```json
{
  "GitHub": {
    "Token": null,           // Set via environment variable GITHUB_TOKEN
    "UseRealApi": false      // Set to true to enable integration tests
  }
}
```

### Environment Variables
- `GITHUB_TOKEN`: Your GitHub Personal Access Token for integration tests

## Integration Tests

Integration tests are marked with `[Ignore]` by default. To run them:

1. Set your GitHub token:
   ```bash
   # Windows (PowerShell)
   $env:GITHUB_TOKEN="your_token_here"
   
   # Linux/macOS
   export GITHUB_TOKEN="your_token_here"
   ```

2. Enable integration tests in `appsettings.json`:
   ```json
   "GitHub": {
     "UseRealApi": true
   }
   ```

3. Remove the `[Ignore]` attribute from tests you want to run

## Test Statistics

- **Total Test Methods**: 88
- **Unit Tests**: 80
- **Integration Tests**: 8
- **Test Coverage**: Targets all 6 API client areas

## Best Practices Implemented

? **Arrange-Act-Assert** pattern for all tests  
? **Mocking** with Moq for HTTP requests  
? **FluentAssertions** for readable test assertions  
? **Test categories** for filtering  
? **Proper cleanup** with TestCleanup  
? **Test data factories** for consistent test data  
? **Sample JSON responses** for realistic testing  
? **Configuration management** for flexible testing  
? **Code coverage** setup and tracking  

## Writing New Tests

### Unit Test Template
```csharp
[TestMethod]
[TestCategory("Unit")]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var expectedData = TestDataFactory.CreateTest...();
    var json = JsonSerializer.Serialize(expectedData, _jsonOptions);
    SetupMockResponse(_mockHandler, HttpStatusCode.OK, json);

    // Act
    var result = await _client.MethodAsync(parameters);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Data.Should()...
}
```

### Integration Test Template
```csharp
[TestMethod]
[TestCategory("Integration")]
[Ignore("Requires real GitHub token")]
public async Task Scenario_WorksCorrectly()
{
    // Skip if not configured
    if (!UseRealApi)
    {
        Assert.Inconclusive("Real API testing not enabled");
    }

    // Arrange
    var client = CreateClient();

    // Act
    var result = await client...

    // Assert
    result.IsSuccess.Should().BeTrue();
    ...
}
```

## Continuous Integration

For CI/CD pipelines, use these commands:

```bash
# Build
dotnet build

# Run unit tests only (fast)
dotnet test --filter:"TestCategory=Unit" --logger "trx"

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## Troubleshooting

### Tests fail with "File not found" for JSON samples
Ensure the `.csproj` includes:
```xml
<None Update="TestData\JsonResponseSamples\**\*.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

### Integration tests always inconclusive
1. Check `GITHUB_TOKEN` environment variable is set
2. Verify `appsettings.json` has `UseRealApi: true`
3. Remove `[Ignore]` attribute from the test

### Rate limit errors in integration tests
GitHub API has rate limits:
- Unauthenticated: 60 requests/hour
- Authenticated: 5,000 requests/hour

Ensure you're using a valid GitHub token for integration tests.
