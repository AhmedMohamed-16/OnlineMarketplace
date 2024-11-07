using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMarketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ApplicationUserId",
                table: "Payments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_ApplicationUserId",
                table: "Payments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_ApplicationUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ApplicationUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Payments");
        }
    }
}
