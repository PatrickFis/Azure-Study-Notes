using TestWebApp.Services;

namespace TestWebApp
{
    public class TestWebApp
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            // Add the course service so it can be resolved with dependency injection
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IConfiguration>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();

        }
    }
}