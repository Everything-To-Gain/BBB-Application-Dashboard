using BBB_ApplicationDashboard.Api.Extensions;
using DotNetEnv;

var app = await CreateWebApplicationAsync(args);
await app.RunAsync();

static async Task<WebApplication> CreateWebApplicationAsync(string[] args)
{
    //! 🧩 load all env variables for local dev only first thing
    Env.Load();
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSecretManager();
    builder.Services.AddDatabase();
    builder.Services.AddApplicationServices();

    var app = builder.Build();
    await app.UseMigrationAsync();
    app.UseStaticFiles();
    app.UseHttpsAndErrorHandling();
    app.UseRoutingAndEndpoints();
    app.UseApiDocs();

    app.MapGet("/", () => Results.Redirect("/docs")).ExcludeFromDescription();

    return app;
}
