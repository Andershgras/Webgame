using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStellarEnergyUpgrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stats_FasterLevelUpLevel",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stats_StellarMoreStellarEnergyLevel",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats_FasterLevelUpLevel",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Stats_StellarMoreStellarEnergyLevel",
                table: "Players");
        }
    }
}
