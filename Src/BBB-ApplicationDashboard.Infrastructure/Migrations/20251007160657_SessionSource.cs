using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBB_ApplicationDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SessionSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionSource",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionSource",
                table: "Sessions");
        }
    }
}
