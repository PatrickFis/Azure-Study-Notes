using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestWebApp.Services;
using TestWebApp.Models;

namespace TestWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICourseService _courseService;

    public IndexModel(ILogger<IndexModel> logger, ICourseService courseService)
    {
        _logger = logger;
        _courseService = courseService;
    }

    public void OnGet()
    {

    }

    public IEnumerable<Course> GetCourses()
    {
        return _courseService.GetCourses();
    }
}
