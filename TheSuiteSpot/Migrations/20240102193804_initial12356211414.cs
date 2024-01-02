using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheSuiteSpot.Migrations
{
    /// <inheritdoc />
    public partial class initial12356211414 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_UserInbox_UserInboxId",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "UserInboxId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_UserInbox_UserInboxId",
                table: "Message",
                column: "UserInboxId",
                principalTable: "UserInbox",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_UserInbox_UserInboxId",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "UserInboxId",
                table: "Message",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_UserInbox_UserInboxId",
                table: "Message",
                column: "UserInboxId",
                principalTable: "UserInbox",
                principalColumn: "Id");
        }
    }
}
