using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs;

public class AccreditationResponse
{
    public Guid ApplicationId { get; set; }
    public string SubmittedEmail { get; set; } = null!;
    public string TrackingLink { get; set; } = null!;
    public ApplicationStatus ApplicationStatus { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string ApplicationNumber { get; set; } = null!;
    public bool IsDuplicate { get; set; } = false;
}
