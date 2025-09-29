using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBB_ApplicationDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addExternalInternalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accreditations",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationNumber = table.Column<string>(type: "text", nullable: false),
                    BlueApplicationID = table.Column<string>(type: "text", nullable: true),
                    HubSpotApplicationID = table.Column<string>(type: "text", nullable: true),
                    BID = table.Column<string>(type: "text", nullable: true),
                    CompanyRecordID = table.Column<string>(type: "text", nullable: true),
                    TrackingLink = table.Column<string>(type: "text", nullable: false),
                    ApplicationStatusInternal = table.Column<int>(type: "integer", nullable: false),
                    ApplicationStatusExternal = table.Column<int>(type: "integer", nullable: false),
                    BusinessName = table.Column<string>(type: "text", nullable: false),
                    DoingBusinessAs = table.Column<string>(type: "text", nullable: true),
                    BusinessAddress = table.Column<string>(type: "text", nullable: false),
                    BusinessAptSuite = table.Column<string>(type: "text", nullable: true),
                    BusinessState = table.Column<string>(type: "text", nullable: false),
                    BusinessCity = table.Column<string>(type: "text", nullable: false),
                    BusinessZip = table.Column<string>(type: "text", nullable: false),
                    MailingAddress = table.Column<string>(type: "text", nullable: false),
                    MailingCity = table.Column<string>(type: "text", nullable: false),
                    MailingState = table.Column<string>(type: "text", nullable: false),
                    MailingZip = table.Column<string>(type: "text", nullable: false),
                    NumberOfLocations = table.Column<int>(type: "integer", nullable: true),
                    PrimaryBusinessPhone = table.Column<string>(type: "text", nullable: false),
                    PrimaryBusinessEmail = table.Column<string>(type: "text", nullable: false),
                    EmailToReceiveQuoteRequestsFromCustomers = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    SocialMediaLinks = table.Column<string>(type: "jsonb", nullable: false),
                    PrimaryFirstName = table.Column<string>(type: "text", nullable: false),
                    PrimaryLastName = table.Column<string>(type: "text", nullable: false),
                    PrimaryTitle = table.Column<string>(type: "text", nullable: false),
                    PrimaryDateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PrimaryContactEmail = table.Column<string>(type: "text", nullable: false),
                    PrimaryContactNumber = table.Column<string>(type: "text", nullable: false),
                    PreferredContactMethod = table.Column<string>(type: "text", nullable: true),
                    PrimaryContactTypes = table.Column<string>(type: "jsonb", nullable: false),
                    SecondaryFirstName = table.Column<string>(type: "text", nullable: true),
                    SecondaryLastName = table.Column<string>(type: "text", nullable: true),
                    SecondaryTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryEmail = table.Column<string>(type: "text", nullable: true),
                    SecondaryContactTypes = table.Column<string>(type: "jsonb", nullable: false),
                    SecondaryPhone = table.Column<string>(type: "text", nullable: true),
                    SecondaryPreferredContactMethod = table.Column<string>(type: "text", nullable: true),
                    BusinessDescription = table.Column<string>(type: "text", nullable: false),
                    BusinessServiceArea = table.Column<string>(type: "text", nullable: false),
                    EIN = table.Column<string>(type: "text", nullable: true),
                    BusinessType = table.Column<string>(type: "text", nullable: false),
                    BusinessEntityType = table.Column<string>(type: "text", nullable: false),
                    BusinessStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Licenses = table.Column<string>(type: "jsonb", nullable: false),
                    NumberOfFullTimeEmployees = table.Column<int>(type: "integer", nullable: false),
                    NumberOfPartTimeEmployees = table.Column<int>(type: "integer", nullable: false),
                    GrossAnnualRevenue = table.Column<int>(type: "integer", nullable: false),
                    AvgCustomersPerYear = table.Column<string>(type: "text", nullable: false),
                    AdditionalBusinessInformation = table.Column<string>(type: "text", nullable: true),
                    SubmittedByName = table.Column<string>(type: "text", nullable: false),
                    SubmittedByTitle = table.Column<string>(type: "text", nullable: false),
                    SubmittedByEmail = table.Column<string>(type: "text", nullable: false),
                    PartnershipSource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accreditations", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    UserSource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accreditations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
