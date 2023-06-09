using Microsoft.Extensions.Configuration;

internal static class IConfigurationRootExtensionsHelpers
{
    public static IConfigurationBuilder AddBasePath(this IConfigurationBuilder builder)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var startupProjectPath = Path.Combine(currentDirectory, "../CryptoDataCollector");
        var basePathConfiguration = Directory.Exists(startupProjectPath) ? startupProjectPath : currentDirectory;

        return builder.SetBasePath(basePathConfiguration);
    }
}