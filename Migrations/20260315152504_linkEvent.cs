using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeKind.Migrations
{
    /// <inheritdoc />
    public partial class linkEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Events",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ContactId",
                table: "Events",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Contacts_ContactId",
                table: "Events",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Contacts_ContactId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ContactId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Events");
        }
    }
}
