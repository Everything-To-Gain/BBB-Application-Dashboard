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

    private readonly string _googleRedirectUri = "https://localhost:7100/api/auth/google-callback";

    private readonly string _googleClientId = secretService.GetSecret(
        ProjectSecrets.PartnersGoogleClientId,
        Folders.Auth
    );

    private readonly string _googleClientSecret = secretService.GetSecret(
        ProjectSecrets.PartnersGoogleClientSecret,
        Folders.Auth
    );

    private readonly string _microsoftRedirectUri =
        $"{secretService.GetSecret(ProjectSecrets.PartnersRedirectBaseUrl, Folders.Auth)}/api/Auth/microsoft-callback";

    private readonly string _microsoftClientId = secretService.GetSecret(
        ProjectSecrets.MicrosoftClientId,
        Folders.Auth
    );

    private readonly string _microsoftClientSecret = secretService.GetSecret(
        ProjectSecrets.MicrosoftClientSecret,
        Folders.Auth
    );

    private readonly string _microsoftTenantId = secretService.GetSecret(
        ProjectSecrets.MicrosoftTenantId,
        Folders.Auth
    );

    public Uri GetGoogleLoginUri()
    {
        return new(
            $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={_googleClientId}&redirect_uri={_googleRedirectUri}&scope=openid email profile&state={Guid.NewGuid()}"
        );
    }

    public Uri GetMicrosoftLoginUri() =>
        new Uri(
            $"https://login.microsoftonline.com/{_microsoftTenantId}/oauth2/v2.0/authorize"
            + $"?client_id={Uri.EscapeDataString(_microsoftClientId)}"
            + $"&response_type=code"
            + $"&redirect_uri={Uri.EscapeDataString(_microsoftRedirectUri)}"
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
                    ClientId = _googleClientId,
                    ClientSecret = _googleClientSecret,
                },

                Scopes = ["openid", "profile", "email"],
            }
        );

        logger.LogInformation(flow.ProjectId);

        var tokens =
            await flow.ExchangeCodeForTokenAsync(
                userId: null,
                code,
                redirectUrl ?? _googleRedirectUri,
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
            $"https://login.microsoftonline.com/{_microsoftTenantId}/oauth2/v2.0/token";

        var body = new Dictionary<string, string>
        {
            ["client_id"] = _microsoftClientId,
            ["scope"] = "openid profile email offline_access User.Read",
            ["code"] = code,
            ["redirect_uri"] = redirectUrl ?? _microsoftRedirectUri,
            ["grant_type"] = "authorization_code",
            ["client_secret"] = _microsoftClientSecret,
        };

        HttpResponseMessage tokenResponse;
        try
        {
            tokenResponse = await new HttpClient().PostAsync(
                tokenEndpoint,
                new FormUrlEncodedContent(body)
            );
        }
        catch (Exception ex)
        {
            throw new UserBadRequestException($"Token endpoint request failed: {ex.Message}");
        }

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var err = await tokenResponse.Content.ReadAsStringAsync();
            throw new UserUnauthorizedException($"Failed to exchange code for tokens: {err}");
        }

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
        var tokenData =
            JsonSerializer.Deserialize<MicrosoftTokenResponse>(tokenJson)
            ?? throw new UserUnauthorizedException("Failed to deserialize token response");

        return tokenData;
    }

    public async Task<MicrosoftUserInfo?> GetMicrosoftUserInfoAsync(string accessToken)
    {
        var graphReq = new HttpRequestMessage(
            HttpMethod.Get,
            "https://graph.microsoft.com/v1.0/me"
        );
        graphReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );

        var graphRes = await new HttpClient().SendAsync(graphReq);
        if (!graphRes.IsSuccessStatusCode)
        {
            var err = await graphRes.Content.ReadAsStringAsync();
            logger.LogError("Failed to call Graph API: {Error}", err);
            return null;
        }

        var graphJson = await graphRes.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<MicrosoftUserInfo>(graphJson);
        return userInfo;
    }
}