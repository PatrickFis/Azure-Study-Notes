using TestWebApp.Models;

namespace TestWebApp.Services
{
    public interface ICourseService
    {
        IEnumerable<Course> GetCourses();

        Task<IEnumerable<Course>> GetCoursesFromFunction();
    }
}