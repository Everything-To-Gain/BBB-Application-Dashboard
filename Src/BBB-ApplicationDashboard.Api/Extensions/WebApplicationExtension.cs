using BBB_ApplicationDashboard.Api.Middlewares;
using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using BBB_ApplicationDashboard.Infrastructure.Configuration;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using BBB_ApplicationDashboard.Infrastructure.Services.Application;
using BBB_ApplicationDashboard.Infrastructure.Services.Email;
using BBB_ApplicationDashboard.Infrastructure.Services.Security;
using BBB_ApplicationDashboard.Infrastructure.Services.Tob;
using Infisical.Sdk;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;

namespace BBB_ApplicationDashboard.Api.Extensions;

public static class WebApplicationExtension
{
    public static IServiceCollection AddSecretManager(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
        {
            var clientId = Environment.GetEnvironmentVariable("ClientId");
            var clientSecret = Environment.GetEnvironmentVariable("ClientSecret");

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new InvalidOperationException(
                    "💥 Infisical credentials not configured in environment variables."
                );
            var settings = new ClientSettings
            {
                Auth = new AuthenticationOptions
                {
                    UniversalAuth = new UniversalAuthMethod
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                    },
                },
            };

            return new InfisicalClient(settings);
        });
        services.AddSingleton<ISecretService, SecretService>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        //! configure the postgre database

        services.AddDbContext<ApplicationDbContext>(
            (serviceProvider, options) =>
            {
                var SecretService = serviceProvider.GetRequiredService<ISecretService>();
                var connectionString = SecretService.GetSecret(
                    ProjectSecrets.ApplicationConnectionString,
                    Folders.ConnectionStrings
                );
                options.UseNpgsql(connectionString);
            }
        );
        //! configure the monogdb database
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var secrets = serviceProvider.GetRequiredService<ISecretService>();
            var mongo = secrets.GetSecret(
                ProjectSecrets.MongoDBConnection,
                Folders.ConnectionStrings
            );
            return new MongoClient(mongo);
        });
        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("bbb-cluster-1")
        );

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddOpenApi();

        //? Email services
        services.AddOptions<EmailOptions>().BindConfiguration("Email");
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailOptions>>().Value);
        services.AddScoped<IEmailService, EmailService>();

        //? Application services
        services.AddScoped<IApplicationService, ApplicationService>();

        //? tob service
        services.AddScoped<ITobService, TobService>();

        //? Configure Mapster
        ConfigureMapster();

        return services;
    }

    public static IApplicationBuilder UseHttpsAndErrorHandling(this IApplicationBuilder app)
    {
        // Only use HTTPS redirection in development
        var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        if (environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }
        app.UseExceptionHandler();
        return app;
    }

    public static async Task<IApplicationBuilder> UseMigrationAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.EnsureCreatedAsync();

            await dbContext.Database.MigrateAsync();
        }
        catch (Exception)
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
            }
            catch (Exception createEx)
            {
                Console.WriteLine($"❌ Database creation failed: {createEx.Message}");
                throw;
            }
        }

        return app;
    }

    public static WebApplication UseRoutingAndEndpoints(this WebApplication app)
    {
        app.UseRouting();
        app.MapControllers();
        return app;
    }

    public static WebApplication UseApiDocs(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(
            "/docs",
            options =>
            {
                options
                    .WithTitle("BBB Application Dashboard API")
                    .WithTheme(ScalarTheme.DeepSpace)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            }
        );
        app.MapGet(
                "/",
                context =>
                {
                    context.Response.Redirect("/docs");
                    return Task.CompletedTask;
                }
            )
            .ExcludeFromDescription();
        return app;
    }

    private static void ConfigureMapster()
    {
        TypeAdapterConfig<Accreditation, AccreditationResponse>.NewConfig();
    }
}
