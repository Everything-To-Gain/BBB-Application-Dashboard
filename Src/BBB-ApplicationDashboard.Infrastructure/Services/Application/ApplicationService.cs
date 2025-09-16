using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Application;

public class ApplicationService(ApplicationDbContext context) : IApplicationService
{
    public async Task<AccreditationResponse> CreateApplicationAsync(SubmittedDataRequest request)
    {
        var existingApplication = await context.Accreditations.FirstOrDefaultAsync(a =>
            a.SubmittedEmail == request.TrackingEmail
        );
        if (existingApplication != null)
        {
            var duplicateResponse = existingApplication.Adapt<AccreditationResponse>();
            duplicateResponse.IsDuplicate = true;
            return duplicateResponse;
        }
        Accreditation accreditation = new()
        {
            ApplicationId = Guid.NewGuid(),
            SubmittedEmail = request.PrimaryBusinessEmail,
            TrackingLink = $"https://bbb.org/track/{Guid.NewGuid()}",
            ApplicationStatus = ApplicationStatus.Submitted,
        };

        context.Accreditations.Add(accreditation);
        await context.SaveChangesAsync();

        AccreditationResponse accreditationResponse = accreditation.Adapt<AccreditationResponse>();

        var applicationsThisYear = await context.Accreditations.CountAsync(a =>
            a.SubmittedEmail != null
        );
        accreditationResponse.ApplicationNumber =
            $"BBB-{DateTime.Now.Year}-ACC-{applicationsThisYear + 1:D4}";
        return accreditationResponse;
    }
}
