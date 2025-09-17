using System.ComponentModel.DataAnnotations;

namespace BBB_ApplicationDashboard.Application.DTOs;

public class SubmittedDataRequest
{
    [Required(ErrorMessage = "Business name is required")]
    public string BusinessName { get; set; } = string.Empty;
    public string? DoingBusinessAs { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business address is required")]
    public string BusinessAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business state is required")]
    public string BusinessState { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business city is required")]
    public string BusinessCity { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business zip is required")]
    public string BusinessZip { get; set; } = string.Empty;
    public string? MailingAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required!")]
    [Phone]
    public string PrimaryBusinessPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business email is required!")]
    public string? PrimaryBusinessEmail { get; set; } = string.Empty;
    public string? RequestQuoteEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Website is required!")]
    public string? Website { get; set; } = string.Empty;
    public string? ContactState { get; set; } = string.Empty;
    public string? ContactCity { get; set; } = string.Empty;
    public string? ContactZip { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact first name is required!")]
    public string PrimaryFirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact last name is required!")]
    public string PrimaryLastName { get; set; } = string.Empty;
    public string? PrimaryTitle { get; set; } = string.Empty;
    public DateTime? PrimaryDateOfBirth { get; set; }

    [Required(ErrorMessage = "Contact email is required!")]
    public string? PrimaryContactEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact email is required!")]
    public string? PrimaryContactNumber { get; set; } = string.Empty;
    public List<string> PreferredContactMethod { get; set; } = [];
    public List<string> PrimaryDelegationTasks { get; set; } = [];
    public string? SecondaryFirstName { get; set; } = string.Empty;
    public string? SecondaryLastName { get; set; } = string.Empty;
    public string? SecondaryEmail { get; set; } = string.Empty;
    public string? SecondaryPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business description is required!")]
    public string? BusinessDescription { get; set; } = string.Empty;
    public string? EIN { get; set; } = string.Empty;
    public string? SSN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type of business is required!")]
    public string? BusinessType { get; set; } = string.Empty;
    public string? IncorporationDetails { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business entity type is required!")]
    public List<string> BusinessEntityType { get; set; } = [];

    [Required(ErrorMessage = "State business license is required!")]
    public string? StateBusinessLicense { get; set; } = string.Empty;
    public string? ProfessionalLicense { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of employees is required!")]
    public string? NumberOfEmployees { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gross annual revenue is required!")]
    public string? GrossAnnualRevenue { get; set; } = string.Empty;

    [Required(ErrorMessage = "Average customer per year is required!")]
    public string? AvgCustomersPerYear { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tracking email is required!")]
    public string? TrackingEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Agreement is required!")]
    public bool Agreement { get; set; }
}
