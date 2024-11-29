using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleCrudProject.Migrations
{
    /// <inheritdoc />
    public partial class addTemperature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "Cities",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Cities");
        }
    }
}
