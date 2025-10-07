using System.ComponentModel.DataAnnotations;

namespace BBB_ApplicationDashboard.Domain;

public class Session
{
    [Key]
    [Required]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}
