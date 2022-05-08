using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace TestWebApp.Services
{
    public class AppConfigService : IAppConfigService
    {
        private readonly ILogger<AppConfigService> _logger;

        public AppConfigService(ILogger<AppConfigService> logger)
        {
            _logger = logger;
        }

        public string RetrieveDemoAppConfigSetting()
        {
            // Create a client to connect to the Key Vault. DefaultAzureCredential will use my Azure account locally and a managed identity in Azure when deployed.
            var client = new SecretClient(new Uri("https://az204appservicekeyvault.vault.azure.net/"), new DefaultAzureCredential());
            // Retrieve the connection string from the Key Vault and use it to retrieve a value from Azure App Configuration
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(client.GetSecret("AppConfigurationConnectionString").Value.Value);

            var appConfig = builder.Build();
            string value = appConfig["demokey"];
            _logger.LogInformation(value);

            return value;
        }

        public async Task<bool> IsDemoFlagEnabled()
        {
            // Create a client to connect to the Key Vault. DefaultAzureCredential will use my Azure account locally and a managed identity in Azure when deployed.
            var client = new SecretClient(new Uri("https://az204appservicekeyvault.vault.azure.net/"), new DefaultAzureCredential());
            // Retrieve the connection string from the Key Vault and use it to retrieve a value from Azure App Configuration. Also tell it to use feature flags.
            IConfigurationRoot config = new ConfigurationBuilder().AddAzureAppConfiguration(options =>
            {
                options.Connect(client.GetSecret("AppConfigurationConnectionString").Value.Value).UseFeatureFlags();
            }).Build();

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config).AddFeatureManagement();
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                IFeatureManager featureManager = serviceProvider.GetRequiredService<IFeatureManager>();
                return await featureManager.IsEnabledAsync("demoflag");
            }
        }

        public async Task<bool> IsFlagEnabled(string flagName)
        {
            // Create a client to connect to the Key Vault. DefaultAzureCredential will use my Azure account locally and a managed identity in Azure when deployed.
            var client = new SecretClient(new Uri("https://az204appservicekeyvault.vault.azure.net/"), new DefaultAzureCredential());
            // Retrieve the connection string from the Key Vault and use it to retrieve a value from Azure App Configuration. Also tell it to use feature flags.
            IConfigurationRoot config = new ConfigurationBuilder().AddAzureAppConfiguration(options =>
            {
                options.Connect(client.GetSecret("AppConfigurationConnectionString").Value.Value).UseFeatureFlags();
            }).Build();

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config).AddFeatureManagement();
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                IFeatureManager featureManager = serviceProvider.GetRequiredService<IFeatureManager>();
                return await featureManager.IsEnabledAsync(flagName);
            }
        }
    }
}