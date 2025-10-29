using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisertatieIRIMIA.Migrations.ApplicationDbContext2Migrations
{
    public partial class FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
            name: "UserId",
            table: "MonitorizedUsers",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_MonitorizedUsers_UserId",
                table: "MonitorizedUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitorizedUsers_AspNetUsers_UserId",
                table: "MonitorizedUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
    name: "UserId",
    table: "MonitorizedUsers",
    type: "nvarchar(max)",
    nullable: false,
    oldClrType: typeof(string),
    oldType: "nvarchar(450)",
    oldNullable: false);

            migrationBuilder.DropForeignKey(
                name: "FK_MonitorizedUsers_AspNetUsers_UserId",
                table: "MonitorizedUsers");

            migrationBuilder.DropIndex(
                name: "IX_MonitorizedUsers_UserId",
                table: "MonitorizedUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MonitorizedUsers");
        }
    }
}
