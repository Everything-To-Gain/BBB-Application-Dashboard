using System;

namespace BBB_ApplicationDashboard.Application.DTOs.User;

public class AdminDashboardUserResponse
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsCsvSync { get; set; }
}