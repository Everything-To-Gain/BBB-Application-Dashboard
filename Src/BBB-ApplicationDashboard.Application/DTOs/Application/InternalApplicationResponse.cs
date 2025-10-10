using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Application;

public class InternalApplicationResponse
{
    public Guid ApplicationId { get; set; }
    public string? BlueApplicationId { get; set; }
    public string? HubSpotApplicationId { get; set; }
    public string? Bid { get; set; }
    public string? CompanyRecordId { get; set; }
    public string? SubmittedByEmail { get; set; }
    public string? ApplicationStatusInternal { get; set; }
    public string? ApplicationStatusExternal { get; set; }
}