using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Audit;

public class SimpleAuditResponse
{
    public Guid Id { get; set; }

    public string? User { get; set; }

    public string? Action { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string? Entity { get; set; }

    public string? EntityIdentifier { get; set; }

    public string? Status { get; set; }

    public string? UserVersion { get; set; }
}
