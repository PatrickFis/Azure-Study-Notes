using TestWebApp.Models;

namespace TestWebApp.Services
{
    public interface IAppConfigService
    {
        string RetrieveDemoAppConfigSetting();

        Task<bool> IsDemoFlagEnabled();

        Task<bool> IsFlagEnabled(string flagName);
    }
}