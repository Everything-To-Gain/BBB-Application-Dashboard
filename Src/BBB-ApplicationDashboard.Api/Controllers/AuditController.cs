using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers
{
    public class AuditController(IAuditService auditService, IN8NAuditService n8n) : CustomControllerBase
    {
        [HttpPost("log")]
        public async Task<IActionResult> LogAudit(ActivityEvent activityEvent)
        {
            await auditService.LogActivityEvent(activityEvent);
            return SuccessResponse("Audit logged successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAudit(Guid id)
        {
            var audit = await auditService.GetActivityEventById(id);
            return SuccessResponseWithData(audit);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFilteredAudits(
            [FromQuery] AuditPaginationRequest request
        )
        {
            var audits = await auditService.GetAllFilteredActivityEvents(request);
            return SuccessResponseWithData(audits);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await auditService.GetUsers();
            return SuccessResponseWithData(users);
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await auditService.GetStatuses();
            return SuccessResponseWithData(statuses);
        }

        [HttpGet("user-versions")]
        public async Task<IActionResult> GetUserVersions()
        {
            var versions = await auditService.GetUserVersions();
            return SuccessResponseWithData(versions);
        }

        [HttpGet("entities")]
        public async Task<IActionResult> GetEntities()
        {
            var entities = await auditService.GetEntities();
            return SuccessResponseWithData(entities);
        }

        [HttpGet("n8n/log")]
        public async Task<IActionResult> LogN8NEvent(
            [FromBody] Dictionary<string, object> payload
        )
        {
            await n8n.Add(payload);
            return SuccessResponse("N8N Audit logged successfully");
        }
    }
}