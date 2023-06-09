using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class LastCandlesTrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CloseCurrent",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CloseLast",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HighCurrent",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HighLast",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LowCurrent",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LowLast",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenCurrent",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenLast",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseCurrent",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CloseLast",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "HighCurrent",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "HighLast",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "LowCurrent",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "LowLast",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "OpenCurrent",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "OpenLast",
                table: "Trades");
        }
    }
}
