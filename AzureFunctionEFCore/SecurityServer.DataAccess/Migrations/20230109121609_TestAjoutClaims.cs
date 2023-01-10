using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityServer.DataAccess.Migrations
{
    public partial class TestAjoutClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationClaim",
                columns: table => new
                {
                    ApplicationsId = table.Column<int>(type: "int", nullable: false),
                    ClaimsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationClaim", x => new { x.ApplicationsId, x.ClaimsId });
                    table.ForeignKey(
                        name: "FK_ApplicationClaim_Applications_ApplicationsId",
                        column: x => x.ApplicationsId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationClaim_Claims_ClaimsId",
                        column: x => x.ClaimsId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimUser",
                columns: table => new
                {
                    ClaimsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimUser", x => new { x.ClaimsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ClaimUser_Claims_ClaimsId",
                        column: x => x.ClaimsId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationClaim_ClaimsId",
                table: "ApplicationClaim",
                column: "ClaimsId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimUser_UsersId",
                table: "ClaimUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationClaim");

            migrationBuilder.DropTable(
                name: "ClaimUser");

            migrationBuilder.DropTable(
                name: "Claims");
        }
    }
}
