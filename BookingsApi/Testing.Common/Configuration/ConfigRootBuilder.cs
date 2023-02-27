using Microsoft.Extensions.Configuration;

namespace Testing.Common.Configuration;

public static class ConfigRootBuilder
{
    private const string UserSecretId = "D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F";
    public static IConfigurationRoot Build(string userSecretId = UserSecretId)
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Production.json", true) // CI write variables in the pipeline to this file
            .AddUserSecrets(userSecretId)
            .AddEnvironmentVariables()
            .Build();
    }
}
