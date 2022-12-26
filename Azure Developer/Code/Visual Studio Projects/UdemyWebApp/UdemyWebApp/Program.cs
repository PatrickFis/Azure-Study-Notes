using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.FeatureManagement;
using UdemyWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Note: This requires the application to have this connection string configured for it to run.
var appConfigConnectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration");

builder.Host.ConfigureAppConfiguration(builder =>
{
    builder.AddAzureAppConfiguration(options => options.Connect(appConfigConnectionString).UseFeatureFlags());
});

builder.Services.AddTransient<IProductService, ProductService>();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddFeatureManagement();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
