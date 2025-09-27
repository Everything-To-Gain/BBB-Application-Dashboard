using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Audit
{
    public class AuditService(ApplicationDbContext context) : IAuditService
    {
        public async Task LogActivityEvent(ActivityEvent activityEvent)
        {
            await context.ActivityEvents.AddAsync(activityEvent);
            await context.SaveChangesAsync();
        }

        public async Task<List<ActivityEvent>> GetActivityEvents(int page = 1, int pageSize = 10)
        {
            return await context.ActivityEvents
                .OrderByDescending(ae => ae.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalActivityEventCount()
        {
            return await context.ActivityEvents.CountAsync();
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
        public async Task<List<string>> GetActions()
        {
            var actions = await context.ActivityEvents
                .Select(ae => ae.Action)
                .Distinct()
                .ToListAsync();
            return actions;
        }

    }
}
