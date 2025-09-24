using System.Text;
using System.Text.Json;
using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Clients;

public class MainServerClient(
    HttpClient httpClient,
    ISecretService secretService,
    ILogger<MainServerClient> logger
) : IMainServerClient
{
    private static StringContent ConstructHttpContent(object? payload)
    {
        if (payload is null)
            return new StringContent("{}", Encoding.UTF8, "application/json");

        return new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );
    }

    public async Task<string> SendFormData(SubmittedDataRequest submittedData, string internalAppId)
    {
        httpClient.DefaultRequestHeaders.Add(
            "X-API-KEY",
            secretService.GetSecret(ProjectSecrets.APIAcessToken, Folders.Auth)
        );

        var response = await httpClient.PostAsync(
            $"http://bbb-sync.playdough.co/api/Partner/accreditation-form/{internalAppId}",
            ConstructHttpContent(submittedData)
        );
        logger.LogInformation(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        var id = await response.Content.ReadAsStringAsync();
        logger.LogInformation("✅ server answered with id: {id}", id);
        return id;
    }
}
