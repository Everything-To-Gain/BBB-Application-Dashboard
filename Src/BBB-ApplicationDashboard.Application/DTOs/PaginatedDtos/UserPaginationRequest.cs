using System;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class UserPaginationRequest : BasePaginationRequest
{
    public bool? IsAdmin { get; set; }
    public bool? IsCSVSync { get; set; }
    public bool? IsActive { get; set; }
}
