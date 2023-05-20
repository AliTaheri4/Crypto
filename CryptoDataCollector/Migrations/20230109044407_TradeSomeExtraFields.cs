using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoDataCollector.Migrations
{
    /// <inheritdoc />
    public partial class TradeSomeExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountGrayCandles",
                table: "Trades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountGreenCandles",
                table: "Trades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountRedCandles",
                table: "Trades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DistancePercentFromSma",
                table: "Trades",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Leverage",
                table: "Trades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Loss",
                table: "Trades",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NeedingInRangeCandles",
                table: "Trades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Profit",
                table: "Trades",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SignalCandleClosePrice",
                table: "Trades",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "Sma200",
                table: "Trades",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountGrayCandles",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CountGreenCandles",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CountRedCandles",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "DistancePercentFromSma",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Leverage",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Loss",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "NeedingInRangeCandles",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Profit",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "SignalCandleClosePrice",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Sma200",
                table: "Trades");
        }
    }
}
