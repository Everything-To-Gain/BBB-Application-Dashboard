using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs;

public class GenerateApiKeyRequest
{
    public string? Description { get; set; }
    public Source Source { get; set; }
}
