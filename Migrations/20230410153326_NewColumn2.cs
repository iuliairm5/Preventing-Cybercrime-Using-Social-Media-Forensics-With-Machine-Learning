using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisertatieIRIMIA.Migrations
{
    /// <inheritdoc />
    public partial class NewColumn2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "InputImage",
                table: "Predictions",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputImage",
                table: "Predictions");
        }
    }
}
