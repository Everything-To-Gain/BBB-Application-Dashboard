using System;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class PaginatedResponse<TEntity>(
    int pageIndex,
    int pageSize,
    long count,
    IEnumerable<TEntity> items
)
    where TEntity : class
{
    public int PageIndex { get; } = pageIndex + 1;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
    public IEnumerable<TEntity> Items { get; } = items;
}
