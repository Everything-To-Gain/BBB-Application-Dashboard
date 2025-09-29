using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController(IAuditService auditService) : CustomControllerBase
    {
        [HttpGet("GetAllAudits")]
        public async Task<IActionResult> GetAllAudits([FromQuery] int page= 1, [FromQuery] int pageSize=10)
        {
            var audits = await auditService.GetActivityEvents(page,pageSize);
            return SucessResponseWithData(audits);
        }
        [HttpPost("log")]
        public async Task<IActionResult> LogAudit(ActivityEvent activityEvent)
        {
            await auditService.LogActivityEvent(activityEvent);
            return SucessResponse("Audit logged successfully");
        }
    }
}
