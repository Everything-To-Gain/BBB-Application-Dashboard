using BBB_ApplicationDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

public class VisualDataController(ITobService tobService) : CustomControllerBase
{
    [HttpGet("type-of-business")]
    public async Task<IActionResult> GetTobs(string? searchTerm)
    {
        return SucessResponse(await tobService.GetTOBs(searchTerm));
    }
}
