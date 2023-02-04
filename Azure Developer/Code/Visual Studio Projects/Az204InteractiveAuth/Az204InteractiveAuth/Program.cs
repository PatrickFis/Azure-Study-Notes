using Microsoft.Identity.Client;

namespace Az204InteractiveAuth
{
    public class Program
    {
        private const string clientId = "client ID from app registration";
        private const string tenantId = "tenant ID from app registration";

        public static async Task Main(string[] args)
        {
            var app = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                .WithRedirectUri("http://localhost")
                .Build();
            string[] scopes = { "user.read" };
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes)
                .ExecuteAsync();

            Console.WriteLine($"Token: {result.AccessToken}, Username: {result.Account.Username}," +
                $" Correlation ID: {result.CorrelationId}, Tenant ID: {result.TenantId}, Claims Principal: {result.ClaimsPrincipal}");
        }
    }
}