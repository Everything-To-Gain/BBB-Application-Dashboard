using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.DTOs.Application;
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

    public async Task<PaginatedResponse<InternalApplicationResponse>> GetInternalData(
        InternalPaginationRequest request
    )
    {
        //! 1) Filter by source internal
        var query = context
            .Accreditations.AsNoTracking()
            .Where(a => a.PartnershipSource == Source.Internal);

        //! 2) Smart search for filter by submitted by email
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(a => EF.Functions.ILike(a.SubmittedByEmail, $"%{searchTerm}%"));
        }

        //! 3) Get total count of result
        int total = await query.CountAsync();

        //! 4) Apply pagination
        int pageIndex = request.PageNumber - 1;
        int pageSize = Math.Max(1, Math.Min(100, request.PageSize));

        //! 5) Execute query
        IEnumerable<InternalApplicationResponse> applications = await query
            .OrderBy(a => a.SubmittedByEmail)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(a => new InternalApplicationResponse
            {
                ApplicationId = a.ApplicationId,
                BlueApplicationID = a.BlueApplicationID,
                HubSpotApplicationID = a.HubSpotApplicationID,
                BID = a.BID,
                CompanyRecordID = a.CompanyRecordID,
                SubmittedByEmail = a.SubmittedByEmail,
                ApplicationStatusInternal = a.ApplicationStatusInternal.ToString(),
            })
            .ToListAsync();

        //! 6) Return result
        return new PaginatedResponse<InternalApplicationResponse>(
            pageIndex,
            pageSize,
            total,
            applications
        );
    }

    public async Task<PaginatedResponse<ExternalApplicationResponse>> GetExternalData(
        ExternalPaginationRequest request,
        Source source
    )
    {
        //! 1) Filter by source
        var query = context.Accreditations.AsNoTracking().Where(a => a.PartnershipSource == source);

        //! 2) Smart search for filter by submitted by email
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(a => EF.Functions.ILike(a.SubmittedByEmail, $"%{searchTerm}%"));
        }

        //! 3) Get total count of result
        int total = await query.CountAsync();

        //! 4) Apply pagination
        int pageIndex = request.PageNumber - 1;
        int pageSize = Math.Max(1, Math.Min(100, request.PageSize));

        //! 5) Execute query
        IEnumerable<ExternalApplicationResponse> applications = await query
            .OrderBy(a => a.SubmittedByEmail)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(a => new ExternalApplicationResponse
            {
                ApplicationId = a.ApplicationId,
                CompanyName = a.BusinessName,
                SubmittedByEmail = a.SubmittedByEmail,
                ApplicationStatusExternal = a.ApplicationStatusExternal.ToString(),
            })
            .ToListAsync();

        //! 6) Return result
        return new PaginatedResponse<ExternalApplicationResponse>(
            pageIndex,
            pageSize,
            total,
            applications
        );
    }
}
