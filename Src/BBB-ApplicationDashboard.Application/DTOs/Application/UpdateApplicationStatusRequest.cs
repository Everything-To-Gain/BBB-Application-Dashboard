using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.Application;

public class UpdateApplicationStatusRequest
{
    [Required(ErrorMessage = "ApplicationId is required")]
    public required string BlueApplicationId { get; set; }

    [Required(ErrorMessage = "Application status is required")]
    public required ApplicationStatusInternal ApplicationStatusInternal { get; set; }
}