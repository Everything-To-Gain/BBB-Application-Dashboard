using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class AdminExternalPaginationRequest : BasePaginationRequest
{
    public ApplicationStatusExternal? ExternalStatus { get; set; } = null;
    public Source? PartnershipSource { get; set; } = null;
}
