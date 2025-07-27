using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/web-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Logging.ClearProviders();

builder.Services.AddHttpClient("HttpApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5038/");
});

builder.Services.AddHttpClient("HttpsApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7071/");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/Employee/HandleStatusCode", "?code={0}");
    app.UseExceptionHandler("/Employee/Error");
    app.UseHsts();
    //app.UseDeveloperExceptionPage();
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=GetEmployeeList}/{id?}");

app.MapRazorPages();

app.Run();
