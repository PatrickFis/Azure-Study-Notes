using TestWebApp.Models;

namespace TestWebApp.Services
{
    public interface IAppConfigService
    {
        string RetrieveDemoAppConfigSetting();

        string RetrieveAppConfigSetting(string setting);

        string RetrieveConnectionUrl(string connection);

        Task<bool> IsDemoFlagEnabled();

        Task<bool> IsFlagEnabled(string flagName);
    }
}