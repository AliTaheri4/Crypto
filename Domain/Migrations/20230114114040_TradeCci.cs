using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class TradeCci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sma200",
                table: "Trades",
                newName: "Indicator3");

            migrationBuilder.AddColumn<double>(
                name: "Indicator1",
                table: "Trades",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Indicator2",
                table: "Trades",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Indicator1",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Indicator2",
                table: "Trades");

            migrationBuilder.RenameColumn(
                name: "Indicator3",
                table: "Trades",
                newName: "Sma200");
        }
    }
}
