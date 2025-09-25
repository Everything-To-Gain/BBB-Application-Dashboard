using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class InternalPaginationRequest : BasePaginationRequest
{
    public ApplicationStatusInternal? InternalStatus { get; set; } = null;
    public ApplicationStatusExternal? ExternalStatus { get; set; } = null;
}
