using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stats_TotalCoinsSpent",
                table: "Players",
                newName: "TotalCoinsSpent");

            migrationBuilder.RenameColumn(
                name: "Stats_TotalCoinsEarned",
                table: "Players",
                newName: "TotalCoinsEarned");

            migrationBuilder.RenameColumn(
                name: "Stats_TotalClicks",
                table: "Players",
                newName: "TotalClicks");

            migrationBuilder.RenameColumn(
                name: "Stats_OfflineCapLevel",
                table: "Players",
                newName: "OfflineCapLevel");

            migrationBuilder.RenameColumn(
                name: "Stats_ClickPowerLevel",
                table: "Players",
                newName: "ClickPowerLevel");

            migrationBuilder.RenameColumn(
                name: "Stats_AutoClickerLevel",
                table: "Players",
                newName: "AutoClickerLevel");

            migrationBuilder.AddColumn<long>(
                name: "Experience",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ExperienceToNextLevel",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Name",
                table: "Players",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_Name",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ExperienceToNextLevel",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "TotalCoinsSpent",
                table: "Players",
                newName: "Stats_TotalCoinsSpent");

            migrationBuilder.RenameColumn(
                name: "TotalCoinsEarned",
                table: "Players",
                newName: "Stats_TotalCoinsEarned");

            migrationBuilder.RenameColumn(
                name: "TotalClicks",
                table: "Players",
                newName: "Stats_TotalClicks");

            migrationBuilder.RenameColumn(
                name: "OfflineCapLevel",
                table: "Players",
                newName: "Stats_OfflineCapLevel");

            migrationBuilder.RenameColumn(
                name: "ClickPowerLevel",
                table: "Players",
                newName: "Stats_ClickPowerLevel");

            migrationBuilder.RenameColumn(
                name: "AutoClickerLevel",
                table: "Players",
                newName: "Stats_AutoClickerLevel");
        }
    }
}
