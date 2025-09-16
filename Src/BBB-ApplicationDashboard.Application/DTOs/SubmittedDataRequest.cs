namespace BBB_ApplicationDashboard.Application.DTOs;

public class SubmittedDataRequest
{
    public string BusinessName { get; set; } = string.Empty;
    public string DoingBusinessAs { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string BusinessState { get; set; } = string.Empty;
    public string BusinessCity { get; set; } = string.Empty;
    public string BusinessZip { get; set; } = string.Empty;
    public string MailingAddress { get; set; } = string.Empty;
    public string ContactState { get; set; } = string.Empty;
    public string ContactCity { get; set; } = string.Empty;
    public string ContactZip { get; set; } = string.Empty;
    public string PrimaryBusinessPhone { get; set; } = string.Empty;
    public string PrimaryBusinessEmail { get; set; } = string.Empty;
    public string RequestQuoteEmail { get; set; } = string.Empty;
    public string PrimaryContactName { get; set; } = string.Empty;
    public string PrimaryTitle { get; set; } = string.Empty;
    public string PrimaryDateOfBirth { get; set; } = string.Empty;
    public string PrimaryContactEmail { get; set; } = string.Empty;
    public string PrimaryContactNumber { get; set; } = string.Empty;
    public string[] PreferredContactMethod { get; set; } = [];
    public string[] PrimaryDelegationTasks { get; set; } = [];
    public string SecondaryContactName { get; set; } = string.Empty;
    public string SecondaryEmail { get; set; } = string.Empty;
    public string SecondaryPhone { get; set; } = string.Empty;
    public string[] SecondaryDelegationTasks { get; set; } = [];
    public string Website { get; set; } = string.Empty;
    public string EIN { get; set; } = string.Empty;
    public string SSN { get; set; } = string.Empty;
    public string StateBusinessLicense { get; set; } = string.Empty;
    public string ProfessionalLicense { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
    public string BusinessTypes { get; set; } = string.Empty;
    public string[] BusinessEntityType { get; set; } = [];
    public string IncorporationDetails { get; set; } = string.Empty;
    public string NumberOfEmployees { get; set; } = string.Empty;
    public string GrossAnnualRevenue { get; set; } = string.Empty;
    public string[] AvgCustomersPerYear { get; set; } = [];
    public string SubmittedByName { get; set; } = string.Empty;
    public string TrackingEmail { get; set; } = string.Empty;
    public bool Agreement { get; set; }
}
