using System.Security.Claims;
using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.DTOs.Application;
using BBB_ApplicationDashboard.Application.DTOs.Common;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

public class ApplicationController(IApplicationService applicationService) : CustomControllerBase
{
    [Authorize]
    [HttpPost("update-application-info")]
    public async Task<IActionResult> UpdateApplicationInfo(ApplicationInfo applicationInfo)
    {
        await applicationService.UpdateApplicationAsync(applicationInfo);
        return SucessResponse("Application updated successfully");
    }

    [HttpPost("submit-form")]
    public async Task<IActionResult> SubmitApplicationForm(SubmittedDataRequest request)
    {
        //! Create application in database
        var accreditationResponse = await applicationService.CreateApplicationAsync(request);
        return SucessResponse(
            data: new { applicationId = accreditationResponse.ApplicationId },
            message: accreditationResponse.IsDuplicate
                ? "Duplicate application detected. Email sent with existing application details."
                : "Application submitted successfully and confirmation email sent"
        );

        //TODO SEND TO SCV SERVER WITH SHAWKI-CHAN DATA
    }

    [Authorize(Policy = "Internal")]
    [HttpGet("internal-data")]
    public async Task<IActionResult> GetInternalData([FromQuery] InternalPaginationRequest request)
    {
        var applications = await applicationService.GetInternalData(request);
        return SucessResponse(applications);
    }

    [Authorize]
    [HttpGet("external-data")]
    public async Task<IActionResult> GetExternalData([FromQuery] ExternalPaginationRequest request)
    {
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleClaim))
            return Unauthorized();

        if (!Enum.TryParse<Source>(roleClaim, ignoreCase: true, out var source))
            return BadRequest();

        var applications = await applicationService.GetExternalData(request, source);

        return SucessResponse(applications);
    }
}
