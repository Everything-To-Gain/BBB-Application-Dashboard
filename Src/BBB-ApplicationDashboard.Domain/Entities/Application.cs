using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Domain.Entities;

public class Accreditation
{
    [Key]
    public Guid ApplicationId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string? BlueId { get; set; }
    public string SubmittedEmail { get; set; } = null!;
    public string TrackingLink { get; set; } = null!;
    public ApplicationStatus ApplicationStatus { get; set; }
}
