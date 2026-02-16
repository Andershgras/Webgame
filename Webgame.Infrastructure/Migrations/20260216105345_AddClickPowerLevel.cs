using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClickPowerLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stats_ClickPowerLevel",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats_ClickPowerLevel",
                table: "Players");
        }
    }
}
