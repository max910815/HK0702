using Microsoft.EntityFrameworkCore.Migrations;

namespace HK_Product.Migrations
{
    public partial class hkcontext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_UserId",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    ApplicationsApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsersUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => new { x.ApplicationsApplicationId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Applications_ApplicationsApplicationId",
                        column: x => x.ApplicationsApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_UsersUserId",
                table: "ApplicationUser",
                column: "UsersUserId");
        }
    }
}
