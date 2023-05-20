using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoDataCollector.Migrations
{
    /// <inheritdoc />
    public partial class ThirsForthCandles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CloseForth",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CloseThird",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Ema100",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Ema200",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Ema21",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Ema50",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ForthLastCandleVolume",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HighForth",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HighThird",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LowForth",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LowThird",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenForth",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenThird",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Sma100",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Sma200",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Sma21",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Sma50",
                table: "Trades",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ThirdLastCandleVolume",
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
                name: "CloseForth",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CloseThird",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Ema100",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Ema200",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Ema21",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Ema50",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ForthLastCandleVolume",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "HighForth",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "HighThird",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "LowForth",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "LowThird",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "OpenForth",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "OpenThird",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Sma100",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Sma200",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Sma21",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Sma50",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ThirdLastCandleVolume",
                table: "Trades");
        }
    }
}
