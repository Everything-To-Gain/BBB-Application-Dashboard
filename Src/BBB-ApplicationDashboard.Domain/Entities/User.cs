using System;
using System.ComponentModel.DataAnnotations;

namespace BBB_ApplicationDashboard.Domain.Entities;

public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [EmailAddress]
    [Required(ErrorMessage = "A user cannot exist without email address!")]
    public required string Email { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsBasic { get; set; } = false;
}
