using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestWebApp.Services;
using TestWebApp.Models;

namespace TestWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICourseService _courseService;
    private readonly IAppConfigService _appConfigService;

    public IndexModel(ILogger<IndexModel> logger, ICourseService courseService, IAppConfigService appConfigService)
    {
        _logger = logger;
        _courseService = courseService;
        _appConfigService = appConfigService;
    }

    public void OnGet()
    {

    }

    public IEnumerable<Course> GetCourses()
    {
        return _courseService.GetCourses();
    }

    public string GetAppConfigValue()
    {
        return _appConfigService.RetrieveDemoAppConfigSetting();
    }

    public string ShowDemoValue()
    {
        // Show a value based on a feature flag
        Task<bool> flag = _appConfigService.IsDemoFlagEnabled();
        flag.Wait();

        return flag.Result ? "Demo flag enabled!" : "Demo flag disabled!";
    }
}
