using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class ExternalPaginationRequest : BasePaginationRequest
{
    public ApplicationStatusExternal? ExternalStatus { get; set; } = null;
}
