using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRunUpgradeLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stats_FasterCoresLevel",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stats_BetterCoresLevel",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stats_BetterCores2Level",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stats_OfflineProductionLevel",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats_FasterCoresLevel",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Stats_BetterCoresLevel",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Stats_BetterCores2Level",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Stats_OfflineProductionLevel",
                table: "Players");
        }
    }
}