using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System.Text.Json;
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

    /// <summary>
    /// Retrives courses from Redis if the courses are cached, otherwise retrieves them from the database or a function
    /// </summary>
    public IEnumerable<Course> GetCourses()
    {
        using (var db = ConnectionMultiplexer.Connect(_appConfigService.RetrieveConnectionUrl("RedisConnectionString")))
        {
            IDatabase cache = db.GetDatabase();

            if (cache.KeyExists("Courses"))
            {
                _logger.LogInformation("Retrieved courses from Redis");
                return JsonSerializer.Deserialize<IEnumerable<Course>>(cache.StringGet("Courses"));
            }
            else
            {
                _logger.LogInformation("Retrieved courses from Azure and stored them in Redis");
                IEnumerable<Course> result = GetCoursesFromAzure();
                cache.StringSet("Courses", JsonSerializer.Serialize<IEnumerable<Course>>(result));
                cache.KeyExpire("Courses", TimeSpan.FromMinutes(15));
                return result;
            }
        }
    }

    private IEnumerable<Course> GetCoursesFromAzure()
    {
        if (_appConfigService.IsFlagEnabled("getCoursesFromFunction").GetAwaiter().GetResult())
        {
            _logger.LogInformation("Retrieved courses from function");
            return _courseService.GetCoursesFromFunction().GetAwaiter().GetResult();
        }
        else
        {
            _logger.LogInformation("Retrieved courses from database");
            return _courseService.GetCourses();
        }
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
