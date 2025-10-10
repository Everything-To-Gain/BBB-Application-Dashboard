using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Domain.Entities;

public class Session
{
    [Key] [Required] public string Token { get; set; } = string.Empty;
    public required Source SessionSource { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}