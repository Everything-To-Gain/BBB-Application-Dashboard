using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

public class ApplicationController(
    IEmailService emailService,
    IApplicationService applicationService
) : CustomControllerBase
{
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

        //! Create email template with dynamic data
        /* var emailHtml = CreateEmailTemplate(
            request,
            accreditationResponse.ApplicationNumber,
            accreditationResponse.TrackingLink,
            accreditationResponse.IsDuplicate
        ); */

        //! Send email
        /* var emailMessage = new EmailMessage
        {
            To = request.SubmittedByEmail!,
            Subject = accreditationResponse.IsDuplicate
                ? "BBB Accreditation Application - Duplicate Submission Detected"
                : "BBB Accreditation Application Submitted Successfully",
            HtmlBody = emailHtml,
        };

        var emailSent = await emailService.SendAsync(emailMessage, CancellationToken.None);

        return emailSent
            ? SucessResponse(
                data: new { applicationId = accreditationResponse.ApplicationId },
                message: accreditationResponse.IsDuplicate
                    ? "Duplicate application detected. Email sent with existing application details."
                    : "Application submitted successfully and confirmation email sent"
            )
            : ErrorResponse(data: "Application received but failed to send confirmation email"); */
        //TODO SEND TO SCV SERVER WITH SHAWKI-CHAN DATA
        return SucessResponse(data: new { applicationId = accreditationResponse.ApplicationId });
    }
}
