using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Domain.Entities;

public class Accreditation
{
    [Key] public Guid ApplicationId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string? BlueApplicationId { get; set; }
    public string? HubSpotApplicationId { get; set; }
    public string? Bid { get; set; }
    public string? CompanyRecordId { get; set; }
    public string TrackingLink { get; set; } = null!;
    public ApplicationStatusInternal ApplicationStatusInternal { get; set; }
    public ApplicationStatusExternal ApplicationStatusExternal { get; set; }
    public required string BusinessName { get; set; }
    public string? DoingBusinessAs { get; set; }
    public required string BusinessAddress { get; set; }
    public string? BusinessAptSuite { get; set; }
    public required string BusinessState { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessZip { get; set; }

    public required string MailingAddress { get; set; }

    public required string MailingCity { get; set; }

    public required string MailingState { get; set; }

    public required string MailingZip { get; set; }

    public int? NumberOfLocations { get; set; }
    public required string PrimaryBusinessPhone { get; set; }

    public required string PrimaryBusinessEmail { get; set; }
    public string? EmailToReceiveQuoteRequestsFromCustomers { get; set; }
    public string? Website { get; set; }

    public List<string> SocialMediaLinks { get; set; } = [];

    public required string PrimaryFirstName { get; set; }

    public required string PrimaryLastName { get; set; }

    public required string PrimaryTitle { get; set; }
    public required DateOnly PrimaryDateOfBirth { get; set; }

    public required string PrimaryContactEmail { get; set; }

    public required string PrimaryContactNumber { get; set; }

    public string? PreferredContactMethod { get; set; }
    public List<string> PrimaryContactTypes { get; set; } = [];
    public string? SecondaryFirstName { get; set; }
    public string? SecondaryLastName { get; set; }
    public string? SecondaryTitle { get; set; }
    public string? SecondaryEmail { get; set; }
    public List<string> SecondaryContactTypes { get; set; } = [];

    public string? SecondaryPhone { get; set; }
    public string? SecondaryPreferredContactMethod { get; set; }

    public required string BusinessDescription { get; set; }

    public required string BusinessServiceArea { get; set; }
    public string? Ein { get; set; }

    [Required(ErrorMessage = "Business type is required!")]
    public required string? BusinessType { get; set; }

    [Required(ErrorMessage = "Business entity type is required!")]
    public required string BusinessEntityType { get; set; }

    [Required(ErrorMessage = "Business start date is required!")]
    public required DateOnly BusinessStartDate { get; set; }

    public List<License> Licenses { get; set; } = [];

    public required int NumberOfFullTimeEmployees { get; set; }

    public required int NumberOfPartTimeEmployees { get; set; }

    public required long GrossAnnualRevenue { get; set; }

    public required string AvgCustomersPerYear { get; set; }

    public string? AdditionalBusinessInformation { get; set; }

    public required string SubmittedByName { get; set; }

    public required string SubmittedByTitle { get; set; }

    public required string SubmittedByEmail { get; set; }

    public required Source PartnershipSource { get; set; }
}

public class License
{
    [Required(ErrorMessage = "Date issued is required!")]
    public required DateOnly DateIssued { get; set; }

    [Required(ErrorMessage = "Agency is required!")]
    public required string Agency { get; set; }

    [Required(ErrorMessage = "Licensing number is required!")]
    public required string LicensingNumber { get; set; }

    public DateOnly? Expiration { get; set; }
}