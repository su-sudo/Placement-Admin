using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using PlacementAdmin.DAL;
using PlacementAdmin.services;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddControllersWithViews();
    //builder.Services.AddSingleton<DatabaseHelper>();
    //builder.Services.AddSingleton<UserDAL>();
    builder.Services.AddScoped<DatabaseHelper>();
    builder.Services.AddScoped<UserDAL>();
    builder.Services.AddScoped<AdminDAL>();
    builder.Services.AddSingleton<UtilityService>();

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
    });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.Use(async (context, next) => { context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate"; 
                                        context.Response.Headers["Pragma"] = "no-cache"; 
                                        context.Response.Headers["Expires"] = "0"; await next(); 
                                        });
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped  program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
