using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterTankerManager.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHiddenToRoundEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "RoundEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "RoundEntries");
        }
    }
}
