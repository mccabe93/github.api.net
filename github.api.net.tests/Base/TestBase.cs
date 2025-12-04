using Microsoft.Extensions.Configuration;

namespace github.api.net.tests.Base;

[TestClass]
public abstract class TestBase
{
    protected static IConfiguration Configuration { get; private set; } = null!;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true);

        Configuration = builder.Build();
    }

    protected string? GitHubToken
    {
        get
        {
            if (field == null)
            {
                field =
                    Configuration["GitHub:Token"]
                    ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            }
            return field;
        }
    }

    private bool? _useRealApi = null;
    protected bool UseRealApi
    {
        get
        {
            if (_useRealApi == null)
            {
                if (Configuration.GetValue<bool?>("GitHub:UseRealApi") != null)
                {
                    _useRealApi = Configuration.GetValue<bool?>("GitHub:UseRealApi");
                }
                else
                {
                    _useRealApi =
                        Environment.GetEnvironmentVariable("GITHUB_USEREALAPI")?.ToLower()
                        == "true";
                }
            }
            return _useRealApi ?? false;
        }
    }
}
