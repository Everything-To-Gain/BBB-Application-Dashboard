using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Application;

public class ExternalApplicationResponse
{
    public Guid ApplicationId { get; set; }
    public string? CompanyName { get; set; }
    public string? SubmittedByEmail { get; set; }
    public string? ApplicationStatusExternal { get; set; }
}
