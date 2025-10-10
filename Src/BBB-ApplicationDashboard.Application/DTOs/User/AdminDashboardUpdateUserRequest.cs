namespace BBB_ApplicationDashboard.Application.DTOs.User;

public class AdminDashboardUpdateUserRequest
{
    public string? Email { get; set; }
    public bool? IsCsvSync { get; set; }
    public bool? IsActive { get; set; }
}