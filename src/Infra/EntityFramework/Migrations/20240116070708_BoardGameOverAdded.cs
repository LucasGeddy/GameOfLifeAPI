using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameOfLife.Migrations
{
    /// <inheritdoc />
    public partial class BoardGameOverAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GameOver",
                table: "Boards",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameOver",
                table: "Boards");
        }
    }
}
