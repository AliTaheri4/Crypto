using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoDataCollector.Migrations
{
    /// <inheritdoc />
    public partial class Trade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CountNowChangeSlope",
                table: "Signals",
                newName: "Symbol");

            migrationBuilder.AddColumn<string>(
                name: "CreatedName",
                table: "Signals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Signals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedName",
                table: "Signals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "Signals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Buy = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    Sell = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    IsEmotional = table.Column<bool>(type: "bit", nullable: false),
                    Symbol = table.Column<int>(type: "int", nullable: false),
                    SymbolName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TradeType = table.Column<int>(type: "int", nullable: false),
                    SignalType = table.Column<int>(type: "int", nullable: false),
                    TradeResultType = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropColumn(
                name: "CreatedName",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "ModifiedName",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "Signals");

            migrationBuilder.RenameColumn(
                name: "Symbol",
                table: "Signals",
                newName: "CountNowChangeSlope");
        }
    }
}
