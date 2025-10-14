using System;
using System.Text.Json;
using Amazon.Runtime.Internal.Util;
using BBB_ApplicationDashboard.Application.DTOs.MicrosoftOAuth;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.User;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Logging;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Auth;

public class AuthService(ISecretService secretService, ILogger<AuthService> logger) : IAuthService
{
    // private readonly string googleRedirectUri =
    //     $"{secretService.GetSecret(ProjectSecrets.PartnersRedirectBaseUrl, Folders.Auth)}/api/auth/google-callback";

    private readonly string googleRedirectUri = "https://localhost:7100/api/auth/google-callback";

    private readonly string googleClientId = secretService.GetSecret(
        ProjectSecrets.PartnersGoogleClientId,
        Folders.Auth
    );

    private readonly string googleClientSecret = secretService.GetSecret(
        ProjectSecrets.PartnersGoogleClientSecret,
        Folders.Auth
    );
    private readonly string microsoftRedirectUri =
        $"{secretService.GetSecret(ProjectSecrets.PartnersRedirectBaseUrl, Folders.Auth)}/api/Auth/microsoft-callback";

    private readonly string microsoftClientId = secretService.GetSecret(
        ProjectSecrets.MicrosoftClientId,
        Folders.Auth
    );

    private readonly string microsoftClientSecret = secretService.GetSecret(
        ProjectSecrets.MicrosoftClientSecret,
        Folders.Auth
    );

    private readonly string microsoftTenantId = secretService.GetSecret(
        ProjectSecrets.MicrosoftTenantId,
        Folders.Auth
    );

    public Uri GetGoogleLoginURI()
    {
        return new(
            $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={googleClientId}&redirect_uri={googleRedirectUri}&scope=openid email profile&state={Guid.NewGuid()}"
        );
    }

    public Uri GetMicrosoftLoginURI() =>
        new Uri(
            $"https://login.microsoftonline.com/{microsoftTenantId}/oauth2/v2.0/authorize"
                + $"?client_id={Uri.EscapeDataString(microsoftClientId)}"
                + $"&response_type=code"
                + $"&redirect_uri={Uri.EscapeDataString(microsoftRedirectUri)}"
                + $"&response_mode=query"
                + $"&scope={Uri.EscapeDataString("openid profile email offline_access User.Read")}"
                + $"&state={Uri.EscapeDataString(Guid.NewGuid().ToString("N"))}"
        );

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string? redirectUrl)
    {
        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = googleClientId,
                    ClientSecret = googleClientSecret,
                },

                Scopes = ["openid", "profile", "email"],
            }
        );

        logger.LogInformation(flow.ProjectId);

        var tokens =
            await flow.ExchangeCodeForTokenAsync(
                userId: null,
                code,
                redirectUrl ?? googleRedirectUri,
                CancellationToken.None
            )
            ?? throw new UserUnauthorizedException(
                "Authentication error, cannot exchange authorization code with google"
            );

        return tokens;
    }

    public async Task<GoogleJsonWebSignature.Payload?> GetPayload(TokenResponse tokenData)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(
            tokenData.IdToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience =
                [
                    secretService.GetSecret(ProjectSecrets.PartnersGoogleClientId, Folders.Auth),
                ],
            }
        );

        if (payload == null || !payload.EmailVerified)
            return null;
        return payload;
    }

    public async Task<MicrosoftTokenResponse> ExchangeMicrosoftCodeForTokenAsync(
        string code,
        string? redirectUrl
    )
    {
        var tokenEndpoint =
            $"https://login.microsoftonline.com/{microsoftTenantId}/oauth2/v2.0/token";

        logger.LogInformation(
            "Starting Microsoft token exchange. Redirect URL: {RedirectUrl}",
            redirectUrl
        );

        var body = new Dictionary<string, string>
        {
            ["client_id"] = microsoftClientId,
            ["scope"] = "openid profile email offline_access User.Read",
            ["code"] = code,
            ["redirect_uri"] = redirectUrl ?? microsoftRedirectUri,
            ["grant_type"] = "authorization_code",
            ["client_secret"] = microsoftClientSecret,
        };

        logger.LogDebug(
            "Sending POST request to Microsoft token endpoint: {Endpoint}",
            tokenEndpoint
        );
        var tokenResponse = await new HttpClient().PostAsync(
            tokenEndpoint,
            new FormUrlEncodedContent(body)
        );

        logger.LogDebug(
            "Received response with status code: {StatusCode}",
            tokenResponse.StatusCode
        );

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var err = await tokenResponse.Content.ReadAsStringAsync();
            logger.LogWarning(
                "Token exchange failed with status code {StatusCode}. Response: {Error}",
                tokenResponse.StatusCode,
                err
            );
            throw new UserUnauthorizedException($"Failed to exchange code for tokens: {err}");
        }

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
        logger.LogDebug("Token response received. Deserializing...");

        var tokenData = JsonSerializer.Deserialize<MicrosoftTokenResponse>(tokenJson);
        if (tokenData is null)
        {
            logger.LogError(
                "Failed to deserialize Microsoft token response. Raw JSON: {Json}",
                tokenJson
            );
            throw new UserUnauthorizedException("Failed to deserialize token response");
        }

        logger.LogInformation("Microsoft token exchange completed successfully.");
        return tokenData;
    }

    public async Task<MicrosoftUserInfo?> GetMicrosoftUserInfoAsync(string accessToken)
    {
        logger.LogInformation("Fetching Microsoft user info from Graph API...");

        var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );

        logger.LogDebug(
            "Sending Graph API request with access token starting with: {TokenPrefix}",
            accessToken[..Math.Min(10, accessToken.Length)]
        );

        var response = await new HttpClient().SendAsync(request);

        logger.LogDebug(
            "Received Graph API response with status code: {StatusCode}",
            response.StatusCode
        );

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            logger.LogError(
                "Failed to call Microsoft Graph API. Status: {StatusCode}, Response: {Error}",
                response.StatusCode,
                err
            );
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        logger.LogDebug("Graph API response received. Deserializing user info...");

        var userInfo = JsonSerializer.Deserialize<MicrosoftUserInfo>(json);
        if (userInfo is null)
        {
            logger.LogError(
                "Failed to deserialize Microsoft Graph API user info. Raw JSON: {Json}",
                json
            );
            return null;
        }

        logger.LogInformation(
            "Successfully retrieved Microsoft user info for {User}",
            userInfo.DisplayName ?? "Unknown User"
        );
        return userInfo;
    }
}
