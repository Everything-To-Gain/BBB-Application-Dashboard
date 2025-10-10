using System;
using System.ComponentModel.DataAnnotations;

namespace BBB_ApplicationDashboard.Application.DTOs.User;

public class AdminDashboardCreateUserRequest
{
    [Required] [EmailAddress] public required string Email { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsCsvSync { get; set; } = false;
}