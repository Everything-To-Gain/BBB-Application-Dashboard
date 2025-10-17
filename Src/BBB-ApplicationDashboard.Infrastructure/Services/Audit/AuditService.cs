using BBB_ApplicationDashboard.Application.DTOs.Audit;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Audit
{
    public class AuditService(ApplicationDbContext context) : IAuditService
    {
        public async Task LogActivityEvent(ActivityEvent activityEvent)
        {
            await context.ActivityEvents.AddAsync(activityEvent);
            await context.SaveChangesAsync();
        }

        public async Task<ActivityEvent?> GetActivityEventById(Guid id)
        {
            return await context.ActivityEvents.FirstOrDefaultAsync(ae => ae.Id == id);
        }

        public async Task DeleteActivityEvent(Guid id)
        {
            var activityEvent = await context.ActivityEvents.FirstOrDefaultAsync(ae => ae.Id == id);
            if (activityEvent != null)
            {
                context.ActivityEvents.Remove(activityEvent);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllActivityEvents()
        {
            var allEvents = context.ActivityEvents;
            context.ActivityEvents.RemoveRange(allEvents);
            await context.SaveChangesAsync();
        }

        public async Task<PaginatedResponse<SimpleAuditResponse>> GetAllFilteredActivityEvents(
            AuditPaginationRequest request
        )
        {
            var query = context.ActivityEvents.AsQueryable();

            // Apply requests dynamically
            if (!string.IsNullOrEmpty(request.User))
                query = query.Where(ae => ae.User == request.User);

            if (!string.IsNullOrEmpty(request.Action))
                query = query.Where(ae => ae.Action == request.Action);

            if (!string.IsNullOrEmpty(request.Entity))
                query = query.Where(ae => ae.Entity == request.Entity);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(ae => ae.Status == request.Status);

            if (request.FromDate.HasValue)
                query = query.Where(ae => ae.Timestamp >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(ae => ae.Timestamp <= request.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();

                query = query.Where(a => (a.EntityIdentifier ?? string.Empty).Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(request.UserVersion))
                query = query.Where(ae => ae.UserVersion == request.UserVersion);

            var count = await query.CountAsync();
            int pageIndex = request.PageNumber - 1;
            int pageSize = Math.Max(1, Math.Min(100, request.PageSize));

            var audits = await query
                .OrderByDescending(ae => ae.Timestamp)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(ae => new SimpleAuditResponse
                {
                    Id = ae.Id,
                    User = ae.User,
                    Action = ae.Action,
                    Timestamp = ae.Timestamp,
                    Entity = ae.Entity,
                    EntityIdentifier = ae.EntityIdentifier,
                    Status = ae.Status,
                    UserVersion = ae.UserVersion,

                })
                .ToListAsync();
            return new PaginatedResponse<SimpleAuditResponse>(pageIndex, pageSize, count, audits);
        }

        public async Task<List<string>> GetActions()
        {
            var actions = await context
                .ActivityEvents.Select(ae => ae.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
            return actions;
        }

        public async Task<List<string>> GetUsers()
        {
            var users = await context
                .ActivityEvents.Select(ae => ae.User)
                .Distinct()
                .OrderBy(u => u)
                .ToListAsync();
            return users;
        }

        public async Task<List<string>> GetEntities()
        {
            var entities = await context
                .ActivityEvents.Select(ae => ae.Entity)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
            return entities;
        }

        public async Task<List<string?>> GetStatuses()
        {
            var statuses = await context
                .ActivityEvents.Select(ae => ae.Status)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
            return statuses;
        }

        public async Task<List<string?>> GetUserVersions()
        {
            var userVersions = await context
                .ActivityEvents.Select(ae => ae.UserVersion)
                .Distinct()
                .OrderByDescending(v => v)
                .ToListAsync();
            return userVersions;
        }
    }
}
