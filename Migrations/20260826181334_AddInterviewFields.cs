using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsiderCareers.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InterviewRoom",
                table: "JobApplications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterviewStatus",
                table: "JobApplications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewRoom",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "InterviewStatus",
                table: "JobApplications");
        }
    }
}
