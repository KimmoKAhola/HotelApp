using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheSuiteSpot.Migrations
{
    /// <inheritdoc />
    public partial class Initial4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateIssued",
                table: "Message",
                newName: "DateSent");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Message",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "DateSent",
                table: "Message",
                newName: "DateIssued");
        }
    }
}
