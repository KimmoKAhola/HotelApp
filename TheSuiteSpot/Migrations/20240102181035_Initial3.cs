using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheSuiteSpot.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Inbox_UserInboxId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Inbox_UserInboxId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inbox",
                table: "Inbox");

            migrationBuilder.RenameTable(
                name: "Inbox",
                newName: "UserInbox");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInbox",
                table: "UserInbox",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_UserInbox_UserInboxId",
                table: "Message",
                column: "UserInboxId",
                principalTable: "UserInbox",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserInbox_UserInboxId",
                table: "User",
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

            migrationBuilder.DropForeignKey(
                name: "FK_User_UserInbox_UserInboxId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInbox",
                table: "UserInbox");

            migrationBuilder.RenameTable(
                name: "UserInbox",
                newName: "Inbox");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inbox",
                table: "Inbox",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Inbox_UserInboxId",
                table: "Message",
                column: "UserInboxId",
                principalTable: "Inbox",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Inbox_UserInboxId",
                table: "User",
                column: "UserInboxId",
                principalTable: "Inbox",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
