using BBB_ApplicationDashboard.Api.Extensions;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace BBB_ApplicationDashboard.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", "BBBAppDashboard")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .CreateLogger();

        try
        {
            Log.Information("🚀 Starting BBB Application Dashboard API");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            Env.Load();
            builder.Services.AddSecretManager().AddDatabase().AddApplicationServices();

            var app = builder.Build();

            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (httpContext, elapsed, ex) =>
                {
                    var path = httpContext.Request.Path.Value ?? "";
                    if (path.Equals("/api/Application/health", StringComparison.OrdinalIgnoreCase))
                    {
                        return LogEventLevel.Debug;
                    }

                    var elapsedMs = elapsed;
                    if (elapsedMs > 240000) // 4 minutes
                    {
                        return LogEventLevel.Fatal;
                    }
                    if (elapsedMs > 120000) // 2 minutes
                    {
                        return LogEventLevel.Warning;
                    }

                    if (ex != null)
                        return LogEventLevel.Error;
                    if (httpContext.Response.StatusCode >= 500)
                        return LogEventLevel.Error;
                    return LogEventLevel.Information;
                };

                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                    diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                    diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
                };
            });

            app.UseHttpsAndErrorHandling();
            await app.UseMigrationAsync();
            app.UseRoutingAndEndpoints();
            app.UseApiDocs();

            Log.Information("✅ BBB Application Dashboard API started successfully");

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "💥 Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
