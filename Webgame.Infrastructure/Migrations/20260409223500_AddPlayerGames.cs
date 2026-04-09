using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Players = table.Column<int>(type: "int", nullable: false),
                    Revenue = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerGames_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGames_PlayerId",
                table: "PlayerGames",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerGames");
        }
    }
}
