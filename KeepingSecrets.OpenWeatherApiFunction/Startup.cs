using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

[assembly: FunctionsStartup(typeof(Startup))]

namespace KeepingSecrets.OpenWeatherApiFunction
{

    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var azureAppConfigurationEndpointUri = Environment.GetEnvironmentVariable("AzureAppConfiguration:Endpoint");

            if (!string.IsNullOrWhiteSpace(azureAppConfigurationEndpointUri))
            {
                builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(azureAppConfigurationEndpointUri), new DefaultAzureCredential());
                });
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            
        }
    }
}
