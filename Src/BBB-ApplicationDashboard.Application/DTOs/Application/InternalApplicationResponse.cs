using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Application;

public class InternalApplicationResponse
{
    public Guid ApplicationId { get; set; }
    public string? BlueApplicationID { get; set; }
    public string? HubSpotApplicationID { get; set; }
    public string? BID { get; set; }
    public string? CompanyRecordID { get; set; }
    public string? SubmittedByEmail { get; set; }
    public string? ApplicationStatusInternal { get; set; }
    public string? ApplicationStatusExternal { get; set; }
}
