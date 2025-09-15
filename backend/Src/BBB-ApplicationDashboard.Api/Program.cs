using BBB_ApplicationDashboard.Api.Extensions;
using DotNetEnv;

namespace BBB_ApplicationDashboard.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Env.Load();

        builder.Services.AddSecretManager().AddDatabase().AddApplicationServices();

        var app = builder.Build();

        app.UseHttpsAndErrorHandling();
        await app.UseMigrationAsync();
        app.UseRoutingAndEndpoints();
        app.UseApiDocs();

        await app.RunAsync();
    }
}
