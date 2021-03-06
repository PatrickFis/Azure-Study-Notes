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
            // Add services so that they can be resolved with dependency injection
            builder.Services.AddScoped<ICourseService, CourseService>()
                .AddScoped<IAppConfigService, AppConfigService>();

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