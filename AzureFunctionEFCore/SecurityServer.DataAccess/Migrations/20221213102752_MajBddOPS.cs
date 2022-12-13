using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityServer.DataAccess.Migrations
{
    public partial class MajBddOPS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Application_ApplicationsId",
                table: "ApplicationUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Application",
                table: "Application");

            migrationBuilder.RenameTable(
                name: "Application",
                newName: "Applications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                table: "Applications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Applications_ApplicationsId",
                table: "ApplicationUser",
                column: "ApplicationsId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Applications_ApplicationsId",
                table: "ApplicationUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                table: "Applications");

            migrationBuilder.RenameTable(
                name: "Applications",
                newName: "Application");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Application",
                table: "Application",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Application_ApplicationsId",
                table: "ApplicationUser",
                column: "ApplicationsId",
                principalTable: "Application",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
