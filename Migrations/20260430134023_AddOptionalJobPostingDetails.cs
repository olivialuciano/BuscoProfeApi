using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuscoProfe.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalJobPostingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaysAndHours",
                table: "JobPostings",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Discipline",
                table: "JobPostings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "JobPostings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalType",
                table: "JobPostings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysAndHours",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Discipline",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "ProfessionalType",
                table: "JobPostings");
        }
    }
}
