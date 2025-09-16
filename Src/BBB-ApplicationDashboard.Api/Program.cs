using BBB_ApplicationDashboard.Api.Extensions;
using DotNetEnv;
using Serilog;

namespace BBB_ApplicationDashboard.Api;

public class Program
{
    public static async Task Main()
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder();
        builder.ConfigureSerilog();

        try
        {
            Log.Information("🚀 Starting BBB Application Dashboard API");

            builder.Services.AddSecretManager().AddDatabase().AddApplicationServices();

            var app = builder.Build();

            app.UseRequestLogging().UseHttpsAndErrorHandling();

            await app.UseMigrationAsync();

            app.UseRoutingAndEndpoints().UseApiDocs();

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
