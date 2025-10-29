using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisertatieIRIMIA.Migrations
{
    public partial class MonitorizedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "MonitorizedUsers",
                columns: table => new
                {
                    MonitorizationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountLatestTweets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorizedUsers", x => x.MonitorizationID);
                });

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropTable(
                name: "MonitorizedUsers");

         
        }
    }
}
