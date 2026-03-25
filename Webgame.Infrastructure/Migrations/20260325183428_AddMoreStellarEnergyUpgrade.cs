using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreStellarEnergyUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stats_MoreStellarEnergyLevel",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats_MoreStellarEnergyLevel",
                table: "Players");
        }
    }
}
