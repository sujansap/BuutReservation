using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBatteryUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentUserId1",
                table: "Battery",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battery_CurrentUserId1",
                table: "Battery",
                column: "CurrentUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Battery_User_CurrentUserId1",
                table: "Battery",
                column: "CurrentUserId1",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Battery_User_CurrentUserId1",
                table: "Battery");

            migrationBuilder.DropIndex(
                name: "IX_Battery_CurrentUserId1",
                table: "Battery");

            migrationBuilder.DropColumn(
                name: "CurrentUserId1",
                table: "Battery");
        }
    }
}
