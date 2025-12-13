using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterTankerManager.Migrations
{
    /// <inheritdoc />
    public partial class AddOffDayToEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOffDay",
                table: "RoundEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOffDay",
                table: "ExpenseEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOffDay",
                table: "RoundEntries");

            migrationBuilder.DropColumn(
                name: "IsOffDay",
                table: "ExpenseEntries");
        }
    }
}
