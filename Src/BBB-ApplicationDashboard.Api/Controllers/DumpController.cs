using BBB_ApplicationDashboard.Api.Controllers;
using BBB_ApplicationDashboard.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api;

[Authorize(AuthenticationSchemes = "ApiKey")]
public class DumpController(IDumpService dumpService) : CustomControllerBase
{
    [HttpPost("partnership-form")]
    public async Task<IActionResult> DumpPartnershipData(
        [FromBody] Dictionary<string, object> payload
    )
    {
        var key =
            HttpContext.Request.Headers["X-API-KEY"].ToString() ?? throw new UnauthorizedException(
                "❌ X-API-KEY didn't exist and this is impossible, Authentication handler sucks and doesnt work"
            );

        await dumpService.DumpIntoMongo(payload, key);
        return SuccessResponse("✅ Dumped data successfully!");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await dumpService.GetAllAsync();
        return SuccessResponseWithData(items);
    }
}
