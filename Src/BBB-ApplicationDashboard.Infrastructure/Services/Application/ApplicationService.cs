using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
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
            a.SubmittedByEmail == request.SubmittedByEmail
        );
        if (existingApplication != null)
        {
            var duplicateResponse = existingApplication.Adapt<AccreditationResponse>();
            duplicateResponse.IsDuplicate = true;
            return duplicateResponse;
        }

        var applicationsThisYear = await context.Accreditations.CountAsync(a =>
            a.SubmittedByEmail != null
        );
        var applicationNumber = $"BBB-{DateTime.Now.Year}-ACC-{applicationsThisYear + 1:D4}";

        Accreditation accreditation = request.Adapt<Accreditation>();
        accreditation.ApplicationId = Guid.NewGuid();
        accreditation.TrackingLink =
            $"https://bbb-partners.playdough.co/track-application/{applicationNumber}";
        accreditation.ApplicationStatusExternal = ApplicationStatusExternal.Submitted;
        accreditation.ApplicationStatusInternal =
            ApplicationStatusInternal.Accreditation_Services_Review;
        accreditation.ApplicationNumber = applicationNumber;

        context.Accreditations.Add(accreditation);
        await context.SaveChangesAsync();

        AccreditationResponse accreditationResponse = accreditation.Adapt<AccreditationResponse>();

        return accreditationResponse;
    }

    public async Task UpdateApplicationAsync(ApplicationInfo applicationInfo)
    {
        if (!Guid.TryParse(applicationInfo.ApplicationID, out var applicationId))
            throw new ArgumentException(
                "Invalid ApplicationID format",
                nameof(applicationInfo.ApplicationID)
            );

        var accreditation =
            await context.Accreditations.FirstOrDefaultAsync(a => a.ApplicationId == applicationId)
            ?? throw new KeyNotFoundException("Accreditation not found");

        accreditation.BlueApplicationID = applicationInfo.BlueAppID;
        accreditation.HubSpotApplicationID = applicationInfo.HubSpotAppID;
        accreditation.BID = applicationInfo.BID;
        accreditation.CompanyRecordID = applicationInfo.CompanyRecordID;

        await context.SaveChangesAsync();
    }

    // public async Task GetInternalData(InternalPaginationRequest) {

    //  }
}
