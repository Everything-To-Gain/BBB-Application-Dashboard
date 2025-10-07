using System.Security.Claims;
using System.Text.Encodings.Web;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Auth;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApplicationDbContext _context;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApplicationDbContext context
    )
        : base(options, logger, encoder)
    {
        _context = context;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (
            !Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeader)
            || string.IsNullOrEmpty(apiKeyHeader)
        )
        {
            return AuthenticateResult.NoResult();
        }

        var apiKey = apiKeyHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
            return AuthenticateResult.NoResult();

        // Look up API key in database
        var session = await _context.Sessions.FirstOrDefaultAsync(s =>
            s.Token == apiKey && s.IsActive && s.ExpiresAt > DateTime.UtcNow
        );

        if (session == null)
            return AuthenticateResult.Fail("Invalid or expired API key");

        // Create simple claims for API key authentication
        var claims = new List<Claim> { new(ClaimTypes.Name, "ApiKeyUser") };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
