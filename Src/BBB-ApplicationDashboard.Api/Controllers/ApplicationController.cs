using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.Interfaces;
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

    // [HttpGet("internal-data")]
    // public async Task<PaginatedResponse<>> GetInternalData(InternalPaginationRequest request) { }
}
