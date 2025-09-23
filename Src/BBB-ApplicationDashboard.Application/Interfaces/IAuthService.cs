using System;
using BBB_ApplicationDashboard.Application.DTOs.MicrosoftOAuth;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Responses;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IAuthService
{
    Uri GetGoogleLoginURI();

    Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string? redirectUrl);

    Task<GoogleJsonWebSignature.Payload?> GetPayload(TokenResponse tokenData);

    Uri GetMicrosoftLoginURI();

    Task<MicrosoftTokenResponse> ExchangeMicrosoftCodeForTokenAsync(
        string code,
        string? redirectUrl
    );

    Task<MicrosoftUserInfo?> GetMicrosoftUserInfoAsync(string accessToken);
}
