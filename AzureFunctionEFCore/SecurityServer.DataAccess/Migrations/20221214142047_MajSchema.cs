using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityServer.DataAccess.Migrations
{
    public partial class MajSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Applications_ApplicationId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicationId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Applications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationId",
                table: "Applications",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Applications_ApplicationId",
                table: "Applications",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id");
        }
    }
}
