using System;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class BasePaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
