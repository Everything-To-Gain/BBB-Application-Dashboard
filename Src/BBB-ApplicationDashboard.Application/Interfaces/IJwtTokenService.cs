using System;
using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateJwtToken(int expirationDays, User user);
    string ExtractEmailFromToken(string token);
}
