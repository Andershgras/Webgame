using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateCoinsToEnergy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalCoinsSpent",
                table: "Players",
                newName: "TotalMerges");

            migrationBuilder.RenameColumn(
                name: "TotalCoinsEarned",
                table: "Players",
                newName: "TotalEnergySpent");

            migrationBuilder.RenameColumn(
                name: "Coins",
                table: "Players",
                newName: "TotalEnergyEarned");

            migrationBuilder.AddColumn<long>(
                name: "Energy",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StellarEnergy",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StormEnergy",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Energy",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StellarEnergy",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StormEnergy",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "TotalMerges",
                table: "Players",
                newName: "TotalCoinsSpent");

            migrationBuilder.RenameColumn(
                name: "TotalEnergySpent",
                table: "Players",
                newName: "TotalCoinsEarned");

            migrationBuilder.RenameColumn(
                name: "TotalEnergyEarned",
                table: "Players",
                newName: "Coins");
        }
    }
}
