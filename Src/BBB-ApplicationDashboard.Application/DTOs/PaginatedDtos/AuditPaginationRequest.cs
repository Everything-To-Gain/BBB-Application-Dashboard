using System;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class AuditPaginationRequest : BasePaginationRequest
{
    public string? User { get; set; }
    public string? Action { get; set; }
    public string? Entity { get; set; }
    public string? Status { get; set; }

    public string? UserVersion { get; set; }

    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}
