using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace az204_auth
{
    public class Program
    {
        // Replace these values with values from AAD
        private const string _clientId = "Replace me";
        private const string _tenantId = "Replace me";
        public static async Task Main(string[] args)
        {
            var app = PublicClientApplicationBuilder
            .Create(_clientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
            .WithRedirectUri("http://localhost")
            .Build();

            string[] scopes = { "user.read" };

            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

            Console.WriteLine($"Token:\t{result.AccessToken}");
        }
    }
}