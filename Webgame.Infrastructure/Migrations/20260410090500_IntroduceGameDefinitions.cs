using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webgame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceGameDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameKey",
                table: "PlayerGames",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE PlayerGames
                SET GameKey = CASE
                    WHEN Name = 'Game #1' THEN 'game-1'
                    ELSE 'game-1'
                END
                """);

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PlayerGames");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PlayerGames",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE PlayerGames
                SET Name = CASE
                    WHEN GameKey = 'game-1' THEN 'Game #1'
                    ELSE GameKey
                END
                """);

            migrationBuilder.DropColumn(
                name: "GameKey",
                table: "PlayerGames");
        }
    }
}
