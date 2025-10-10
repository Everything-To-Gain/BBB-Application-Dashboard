using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs;

public class SubmittedDataRequest
{
    [Required(ErrorMessage = "Business name is required")]
    public required string BusinessName { get; set; }

    public string? DoingBusinessAs { get; set; }

    [Required(ErrorMessage = "Business address is required")]
    public required string BusinessAddress { get; set; }

    public string? BusinessAptSuite { get; set; }

    [Required(ErrorMessage = "Business state is required")]
    public required string BusinessState { get; set; }

    [Required(ErrorMessage = "Business city is required")]
    public required string BusinessCity { get; set; }

    [Required(ErrorMessage = "Business zip is required")]
    public required string BusinessZip { get; set; }

    [Required(ErrorMessage = "Mailing address is required")]
    public required string MailingAddress { get; set; }

    [Required(ErrorMessage = "Mailing city is required")]
    public required string MailingCity { get; set; }

    [Required(ErrorMessage = "Mailing state is required")]
    public required string MailingState { get; set; }

    [Required(ErrorMessage = "Mailing zip is required")]
    public required string MailingZip { get; set; }

    public int? NumberOfLocations { get; set; }

    [Required(ErrorMessage = "Phone number is required!")]
    [Phone]
    public required string PrimaryBusinessPhone { get; set; }

    [Required(ErrorMessage = "Business email is required!")]
    [EmailAddress]
    public required string PrimaryBusinessEmail { get; set; }

    public string? EmailToReceiveQuoteRequestsFromCustomers { get; set; }
    public string? Website { get; set; }

    public List<string> SocialMediaLinks { get; set; } = [];

    [Required(ErrorMessage = "Primary first name is required!")]
    public required string PrimaryFirstName { get; set; }

    [Required(ErrorMessage = "Primary last name is required!")]
    public required string PrimaryLastName { get; set; }

    [Required(ErrorMessage = "Primary title is required!")]
    public required string PrimaryTitle { get; set; }

    [Required(ErrorMessage = "Primary date of birth is required!")]
    public required DateOnly PrimaryDateOfBirth { get; set; }

    [Required(ErrorMessage = "Primary contact email is required!")]
    public required string PrimaryContactEmail { get; set; }

    [Required(ErrorMessage = "Primary contact email is required!")]
    [Phone]
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

    [Required(ErrorMessage = "Business description is required!")]
    public required string BusinessDescription { get; set; }

    [Required(ErrorMessage = "Business service area is required!")]
    public required string BusinessServiceArea { get; set; }

    public string? Ein { get; set; }

    [Required(ErrorMessage = "Business type is required!")]
    public required string? BusinessType { get; set; }

    [Required(ErrorMessage = "Business entity type is required!")]
    public required string BusinessEntityType { get; set; }

    [Required(ErrorMessage = "Business start date is required!")]
    public required DateOnly BusinessStartDate { get; set; }

    public List<License> Licenses { get; set; } = [];

    [Required(ErrorMessage = "Number of full time employees is required!")]
    public required int NumberOfFullTimeEmployees { get; set; }

    public int? NumberOfPartTimeEmployees { get; set; }

    [Required(ErrorMessage = "Gross annual revenue is required!")]
    public required long GrossAnnualRevenue { get; set; }

    [Required(ErrorMessage = "Average customer per year is required!")]
    public required string AvgCustomersPerYear { get; set; }

    public string? AdditionalBusinessInformation { get; set; }

    [Required(ErrorMessage = "Sumbitted by name is required!")]
    public required string SubmittedByName { get; set; }

    [Required(ErrorMessage = "Sumbitted by title is required!")]
    public required string SubmittedByTitle { get; set; }

    [Required(ErrorMessage = "Sumbitted by email is required!")]
    public required string SubmittedByEmail { get; set; }

    [Required(ErrorMessage = "Partnership source is required!")]
    public required Source PartnershipSource { get; set; }
}