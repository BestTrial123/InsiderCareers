using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsiderCareers.Migrations
{
    /// <inheritdoc />
    public partial class AddClickCountToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "Jobs");
        }
    }
}
