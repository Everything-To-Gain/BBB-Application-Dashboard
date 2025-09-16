using System.Collections.Concurrent;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using Infisical.Sdk;
using Env = System.Environment;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Security;

internal class GetSecret : GetSecretOptions
{
    public GetSecret()
    {
        Environment = Env.GetEnvironmentVariable("ENV") ?? "dev";
        ProjectId =
            Env.GetEnvironmentVariable("ProjectId")
            ?? throw new ArgumentNullException("Infiscal Error: Project id is missing");
    }
}

public class SecretService(InfisicalClient infisicalClient) : ISecretService
{
    private readonly ConcurrentDictionary<string, string> _cache = new();

    private static GetSecret CreateSecret(ProjectSecrets name, Folders folder)
    {
        return new GetSecret { SecretName = name.ToString(), Path = $"/{folder}" };
    }

    public string GetSecret(ProjectSecrets name, Folders folder)
    {
        try
        {
            return _cache.GetOrAdd(
                name.ToString(),
                n =>
                    infisicalClient
                        .GetSecret(CreateSecret(Enum.Parse<ProjectSecrets>(n), folder))
                        .SecretValue
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 Error retrieving secret: {ex.Message}");
            throw;
        }
    }
}
