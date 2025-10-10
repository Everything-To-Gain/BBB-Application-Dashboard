using System;
using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Domain.Entities;

public class User
{
    [Key] public Guid UserId { get; set; } = Guid.NewGuid();

    [EmailAddress]
    [Required(ErrorMessage = "A user cannot exist without email address!")]
    public required string Email { get; set; }

    public required Source UserSource { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; } = false;
    public bool IsCsvSync { get; set; } = false;
}