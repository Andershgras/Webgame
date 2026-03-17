using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOfflineProgressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stats_CoinsPerClickLevel",
                table: "Players",
                newName: "Stats_OfflineCapLevel");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveUtc",
                table: "Players",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActiveUtc",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "Stats_OfflineCapLevel",
                table: "Players",
                newName: "Stats_CoinsPerClickLevel");
        }
    }
}
