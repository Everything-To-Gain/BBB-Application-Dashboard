using System.Security.Claims;
using BBB_ApplicationDashboard.Application.DTOs.Application;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

public class ApplicationController(
    IApplicationService applicationService,
    IMainServerClient mainServerClient
) : CustomControllerBase
{
    [Authorize]
    [HttpPost("update-application-info")]
    public async Task<IActionResult> UpdateApplicationInfo(ApplicationInfo applicationInfo)
    {
        await applicationService.UpdateApplicationAsync(applicationInfo);
        return SucessResponseWithData("Application updated successfully");
    }

    [HttpPost("submit-form")]
    public async Task<IActionResult> SubmitApplicationForm(SubmittedDataRequest request)
    {
        //!1)Create application in database
        var accreditationResponse = await applicationService.CreateApplicationAsync(request);
        //!2)send to main server
        await mainServerClient.SendFormData(
            request,
            accreditationResponse.ApplicationId.ToString()
        );
        return SucessResponseWithData(
            data: new { applicationId = accreditationResponse.ApplicationId },
            message: accreditationResponse.IsDuplicate
                ? "Duplicate application detected. Email sent with existing application details."
                : "Application submitted successfully and confirmation email sent"
        );
    }

    [Authorize(Policy = "Internal")]
    [HttpGet("internal-data")]
    public async Task<IActionResult> GetInternalData([FromQuery] InternalPaginationRequest request)
    {
        var applications = await applicationService.GetInternalData(request);
        return SucessResponseWithData(applications);
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

        return SucessResponseWithData(applications);
    }

    [Authorize]
    [HttpPatch("update-application-status")]
    public async Task<IActionResult> UpdateApplicationStatus(
        UpdateApplicationStatusRequest updateApplicationStatusRequest
    )
    {
        var result = await applicationService.UpdateApplicationStatus(
            updateApplicationStatusRequest
        );
        return result
            ? SucessResponse("Application status updated successfully")
            : ErrorResponse("Application not found");
    }
}
