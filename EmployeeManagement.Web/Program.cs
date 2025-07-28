using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/web-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Logging.ClearProviders();

// Configure HTTP Clients
builder.Services.AddHttpClient("HttpApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5038/");
});

builder.Services.AddHttpClient("HttpsApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7071/");
});

// Add Controllers with Views
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/Employee/HandleStatusCode", "?code={0}");
    app.UseExceptionHandler("/Employee/Error");
    app.UseHsts();
}
else
{
    app.UseStatusCodePagesWithReExecute("/Employee/HandleStatusCode", "?code={0}");
    app.UseExceptionHandler("/Employee/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// No authentication, so no need for UseAuthentication
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=GetEmployeeList}/{id?}");

// Removed app.MapRazorPages(); since you're not using Razor Pages

app.Run();
