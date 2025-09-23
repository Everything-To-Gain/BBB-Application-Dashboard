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
        accreditation.ApplicationStatus = ApplicationStatus.Submitted;
        accreditation.ApplicationNumber = applicationNumber;
        // Accreditation accreditation = new()
        // {
        //     ApplicationId = Guid.NewGuid(),
        //     TrackingLink =
        //         $"https://bbb-partners.playdough.co/track-application/{applicationNumber}",
        //     ApplicationStatus = ApplicationStatus.Submitted,
        //     ApplicationNumber = applicationNumber,
        //     BusinessName = request.BusinessName,
        //     DoingBusinessAs = request.DoingBusinessAs,
        //     BusinessAddress = request.BusinessAddress,
        //     BusinessAptSuite = request.BusinessAptSuite,
        //     BusinessState = request.BusinessState,
        //     BusinessCity = request.BusinessCity,
        //     BusinessZip = request.BusinessZip,
        //     MailingAddress = request.MailingAddress,
        //     MailingCity = request.MailingCity,
        //     MailingState = request.MailingState,
        //     MailingZip = request.MailingZip,
        //     NumberOfLocations = request.NumberOfLocations,
        //     PrimaryBusinessPhone = request.PrimaryBusinessPhone,
        //     PrimaryBusinessEmail = request.PrimaryBusinessEmail,
        //     EmailToReceiveQuoteRequestsFromCustomers =
        //         request.EmailToReceiveQuoteRequestsFromCustomers,
        //     Website = request.Website,
        //     SocialMediaLinks = request.SocialMediaLinks,
        //     PrimaryFirstName = request.PrimaryFirstName,
        //     PrimaryLastName = request.PrimaryLastName,
        //     PrimaryTitle = request.PrimaryTitle,
        //     PrimaryDateOfBirth = request.PrimaryDateOfBirth,
        //     PrimaryContactEmail = request.PrimaryContactEmail,
        //     PrimaryContactNumber = request.PrimaryContactNumber,
        //     PreferredContactMethod = request.PreferredContactMethod,
        //     PrimaryContactTypes = request.PrimaryContactTypes,
        //     SecondaryFirstName = request.SecondaryFirstName,
        //     SecondaryLastName = request.SecondaryLastName,
        //     SecondaryTitle = request.SecondaryTitle,
        //     SecondaryEmail = request.SecondaryEmail,
        //     SecondaryContactTypes = request.SecondaryContactTypes,
        //     SecondaryPhone = request.SecondaryPhone,
        //     SecondaryPreferredContactMethod = request.SecondaryPreferredContactMethod,
        //     BusinessDescription = request.BusinessDescription,
        //     BusinessServiceArea = request.BusinessServiceArea,
        //     EIN = request.EIN,
        //     BusinessType = request.BusinessType,
        //     BusinessEntityType = request.BusinessEntityType,
        //     BusinessStartDate = request.BusinessStartDate,
        //     Licenses = request.Licenses,
        //     NumberOfFullTimeEmployees = request.NumberOfFullTimeEmployees,
        //     NumberOfPartTimeEmployees = request.NumberOfPartTimeEmployees,
        //     GrossAnnualRevenue = request.GrossAnnualRevenue,
        //     AvgCustomersPerYear = request.AvgCustomersPerYear,
        //     AdditionalBusinessInformation = request.AdditionalBusinessInformation,
        //     SubmittedByName = request.SubmittedByName,
        //     SubmittedByTitle = request.SubmittedByTitle,
        //     SubmittedByEmail = request.SubmittedByEmail,
        // };

        context.Accreditations.Add(accreditation);
        await context.SaveChangesAsync();

        AccreditationResponse accreditationResponse = accreditation.Adapt<AccreditationResponse>();

        return accreditationResponse;
    }
}
