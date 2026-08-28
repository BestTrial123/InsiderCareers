using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsiderCareers.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipientFieldsToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderEmail",
                table: "Messages",
                newName: "SenderType");

            migrationBuilder.RenameColumn(
                name: "EmployerId",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.AddColumn<int>(
                name: "RecipientId",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RecipientType",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RecipientType",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderType",
                table: "Messages",
                newName: "SenderEmail");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "EmployerId");

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "Messages",
                type: "text",
                nullable: true);
        }
    }
}
