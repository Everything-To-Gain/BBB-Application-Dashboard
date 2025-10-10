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
        return SuccessResponseWithData("Application updated successfully");
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
        return SuccessResponseWithData(
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
        return SuccessResponseWithData(applications);
    }

    [Authorize(Policy = "Internal")]
    [HttpGet("application-internal-status")]
    public IActionResult GetApplicationInternalStatus()
    {
        var statuses = Enum.GetValues<ApplicationStatusInternal>()
            .Cast<ApplicationStatusInternal>()
            .Select(e => new { Id = (int)e, Name = e.ToString() })
            .ToList();
        return SuccessResponseWithData(statuses);
    }

    [Authorize(Policy = "Internal")]
    [HttpGet("internal-data/{id}")]
    public async Task<IActionResult> GetApplicationDetails(Guid id)
    {
        var applicationDetails = await applicationService.GetApplicationById(id);
        return SuccessResponseWithData(applicationDetails);
    }

    [Authorize(Policy = "Internal")]
    [HttpPost("{applicationId}/send-form-data")]
    public async Task<IActionResult> SendCsvFormData(Guid applicationId)
    {
        var applicationDetails = await applicationService.GetApplicationById(applicationId);
        var request = applicationDetails.Adapt<SubmittedDataRequest>();
        await mainServerClient.SendFormData(request, applicationDetails.ApplicationId.ToString());
        return SuccessResponse();
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

        return SuccessResponseWithData(applications);
    }

    [Authorize(Policy = "Internal")]
    [HttpGet("external-data/admins")]
    public async Task<IActionResult> GetExternalDataForAdmins(
        [FromQuery] AdminExternalPaginationRequest request
    )
    {
        var applications = await applicationService.GetExternalDataForAdmins(request);

        return SuccessResponseWithData(applications);
    }

    [Authorize]
    [HttpGet("application-external-status")]
    public IActionResult GetApplicationExternalStatus()
    {
        var statuses = Enum.GetValues<ApplicationStatusExternal>()
            .Cast<ApplicationStatusExternal>()
            .Select(e => new { Id = (int)e, Name = e.ToString() })
            .ToList();
        return SuccessResponseWithData(statuses);
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
            ? SuccessResponse("Application status updated successfully")
            : ErrorResponse("Application not found");
    }
}