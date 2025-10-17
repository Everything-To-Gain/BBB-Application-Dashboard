using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBB_ApplicationDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveApplicationNumberUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Accreditations_ApplicationNumber",
                table: "Accreditations");

            migrationBuilder.CreateIndex(
                name: "IX_Accreditations_ApplicationNumber",
                table: "Accreditations",
                column: "ApplicationNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Accreditations_ApplicationNumber",
                table: "Accreditations");

            migrationBuilder.CreateIndex(
                name: "IX_Accreditations_ApplicationNumber",
                table: "Accreditations",
                column: "ApplicationNumber",
                unique: true);
        }
    }
}
