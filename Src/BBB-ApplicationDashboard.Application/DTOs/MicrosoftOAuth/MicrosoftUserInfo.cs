using System;
using System.Text.Json.Serialization;

namespace BBB_ApplicationDashboard.Application.DTOs.MicrosoftOAuth;

public class MicrosoftUserInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("givenName")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonPropertyName("userPrincipalName")]
    public string UserPrincipalName { get; set; } = string.Empty;

    [JsonPropertyName("mail")]
    public string Mail { get; set; } = string.Empty;

    [JsonPropertyName("jobTitle")]
    public string? JobTitle { get; set; }

    [JsonPropertyName("officeLocation")]
    public string? OfficeLocation { get; set; }

    [JsonPropertyName("preferredLanguage")]
    public string? PreferredLanguage { get; set; }

    [JsonPropertyName("mobilePhone")]
    public string? MobilePhone { get; set; }

    [JsonPropertyName("businessPhones")]
    public List<string> BusinessPhones { get; set; } = new();
}
