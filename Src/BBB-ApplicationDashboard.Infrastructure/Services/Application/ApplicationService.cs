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

        var applicationsThisYear = await context.Accreditations.CountAsync(a =>
            a.SubmittedEmail != null
        );
        var applicationNumber = $"BBB-{DateTime.Now.Year}-ACC-{applicationsThisYear + 1:D4}";
        Accreditation accreditation = new()
        {
            ApplicationId = Guid.NewGuid(),
            SubmittedEmail = request.TrackingEmail!,
            TrackingLink = $"https://bbb.org/track/{applicationNumber}",
            ApplicationStatus = ApplicationStatus.Submitted,
            ApplicationNumber = applicationNumber,
        };

        context.Accreditations.Add(accreditation);
        await context.SaveChangesAsync();

        AccreditationResponse accreditationResponse = accreditation.Adapt<AccreditationResponse>();

        return accreditationResponse;
    }
}
