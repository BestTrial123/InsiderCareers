using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsiderCareers.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployerVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InterviewStatus",
                table: "Employers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewStatus",
                table: "Employers");
        }
    }
}
