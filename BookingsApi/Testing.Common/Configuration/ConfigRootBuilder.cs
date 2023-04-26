using Microsoft.Extensions.Configuration;

namespace Testing.Common.Configuration;

public static class ConfigRootBuilder
{
    private const string UserSecretId = "D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F";
    public static IConfigurationRoot Build(string userSecretId = UserSecretId, bool useSecrets = true)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", true)
            .AddJsonFile("appsettings.Production.json", true); // CI write variables in the pipeline to this file

        if (useSecrets)
        {
            builder = builder.AddUserSecrets(userSecretId);
        }

        return builder.AddEnvironmentVariables()
            .Build();
    }
}
