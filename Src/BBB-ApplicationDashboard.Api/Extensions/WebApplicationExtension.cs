using System.Text.Json.Serialization;
using BBB_ApplicationDashboard.Infrastructure.Services.Clients;

namespace BBB_ApplicationDashboard.Api.Extensions;

public static class WebApplicationExtension
{
    //!1️⃣ Secrets & Config
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

    //!2️⃣ Logging config
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        var isDevelopment = builder.Environment.IsDevelopment();

        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", "BBBAppDashboard")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Debug
            );

        if (isDevelopment)
        {
            loggerConfig
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information);
        }
        else
        {
            loggerConfig
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
        }

        Log.Logger = loggerConfig.CreateLogger();
        builder.Host.UseSerilog();
    }

    //!3️⃣ Database config
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        // PostgreSQL Configuration
        services.AddDbContext<ApplicationDbContext>(
            (serviceProvider, options) =>
            {
                var secretService = serviceProvider.GetRequiredService<ISecretService>();
                var connectionString = secretService.GetSecret(
                    ProjectSecrets.ApplicationConnectionString,
                    Folders.ConnectionStrings
                );
                options.UseNpgsql(connectionString);
            }
        );

        //? MongoDB Configuration
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

    //!4️⃣ Authentication & authorization
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        var secretService = services.BuildServiceProvider().GetRequiredService<ISecretService>();
        string secretKey =
            secretService.GetSecret(ProjectSecrets.AuthSecretKey, Folders.Auth)
            ?? throw new NotFoundException("Secret key could not be found!");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)
                        ),
                        ClockSkew = TimeSpan.Zero,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (
                                context.Request.Cookies.TryGetValue(
                                    "BBBPartnersAuth",
                                    out var cookieToken
                                )
                            )
                                context.Token = cookieToken;
                            return Task.CompletedTask;
                        },
                    };

                    options.Events.OnMessageReceived = context =>
                    {
                        // if (
                        //     context.Request.Cookies.TryGetValue(
                        //         "BBBPartnersAuth",
                        //         out var cookieToken
                        //     )
                        // )
                        //     context.Token = cookieToken;
                        return Task.CompletedTask;
                    };
                }
            );

        services.AddAuthorization(options =>
        {
            // Policy for Admin only
            options.AddPolicy("Internal", policy => policy.RequireRole(Source.Internal.ToString()));
        });
        return services;
    }

    //!5️⃣ Services DI
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //? Core Services
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddOpenApi();
        services.AddHttpClient();

        //? Email Services
        services.AddOptions<EmailOptions>().BindConfiguration("Email");
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailOptions>>().Value);
        services.AddScoped<IEmailService, EmailService>();

        //? Business Services
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<ITobService, TobService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        //? Client Services
        services.AddScoped<IMainServerClient, MainServerClient>();

        //? Object Mapping
        ConfigureMapster();

        return services;
    }

    //!6️⃣ Cors configuration
    public static IServiceCollection AddApplicationCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "Angular Cors",
                policy =>
                    policy
                        .WithOrigins(
                            "http://localhost:4201",
                            "https://localhost:4201",
                            "https://joinbbb.org"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            );
        });
        return services;
    }

    //!7️⃣ Application build setup
    public static void AddApplication(this WebApplicationBuilder builder)
    {
        builder.ConfigureSerilog();
        builder
            .Services.AddSecretManager()
            .AddDatabase()
            .AddApplicationServices()
            .AddApplicationCors()
            .AddAuth();
    }

    //!8️⃣ middleware pipeline
    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                var elapsedMs = elapsed;
                if (elapsedMs > 240000)
                    return LogEventLevel.Fatal;
                if (elapsedMs > 120000)
                    return LogEventLevel.Warning;
                if (ex != null || httpContext.Response.StatusCode >= 500)
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
        return app;
    }

    public static WebApplication UseHttpsAndErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();
        return app;
    }

    public static WebApplication UseAppCors(this WebApplication app)
    {
        app.UseCors("Angular Cors");
        return app;
    }

    public static WebApplication UseRoutingAndEndpoints(this WebApplication app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }

    //!9️⃣ Api docs
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

    //!🔟 Database migration
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

    //!1️⃣1️⃣ Pipeline Orchestration
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        return app.UseRequestLogging()
            .UseHttpsAndErrorHandling()
            .UseAppCors()
            .UseRoutingAndEndpoints()
            .UseApiDocs();
    }

    //!1️⃣2️⃣ Mapster config
    private static void ConfigureMapster()
    {
        TypeAdapterConfig<Accreditation, AccreditationResponse>.NewConfig();
    }
}
