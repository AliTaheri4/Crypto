﻿// <auto-generated />
using System;
using CryptoDataCollector.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CryptoDataCollector.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230413181022_ThirsForthCandles")]
    partial class ThirsForthCandles
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CryptoDataCollector.Data.Candle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Close")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("High")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<decimal>("Low")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<decimal>("Open")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<int>("Symbol")
                        .HasColumnType("int");

                    b.Property<string>("SymbolName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Ticks")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Volume")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.HasKey("Id");

                    b.ToTable("Candles");
                });

            modelBuilder.Entity("CryptoDataCollector.Data.Signal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("GeneralStatus")
                        .HasColumnType("int");

                    b.Property<decimal>("Loss")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<string>("ModifiedName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Profit")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<int>("Symbol")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Signals");
                });

            modelBuilder.Entity("CryptoDataCollector.Data.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Buy")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<DateTime>("BuyTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("CloseCurrent")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("CloseForth")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("CloseLast")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("CloseThird")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<int?>("CountGrayCandles")
                        .HasColumnType("int");

                    b.Property<int?>("CountGreenCandles")
                        .HasColumnType("int");

                    b.Property<int?>("CountRedCandles")
                        .HasColumnType("int");

                    b.Property<string>("CreatedName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DistancePercentFromSma")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Ema100")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Ema200")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Ema21")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Ema50")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("ForthLastCandleVolume")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("HighCurrent")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("HighForth")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("HighLast")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("HighThird")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Indicator1")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Indicator2")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Indicator3")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<bool>("IsEmotional")
                        .HasColumnType("bit");

                    b.Property<int>("Leverage")
                        .HasColumnType("int");

                    b.Property<decimal>("Loss")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<decimal>("LowCurrent")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("LowForth")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("LowLast")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("LowThird")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("ModifiedName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("NeedingInRangeCandles")
                        .HasColumnType("int");

                    b.Property<decimal>("OpenCurrent")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("OpenForth")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("OpenLast")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("OpenThird")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Profit")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<decimal>("Sell")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<DateTime?>("SellTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("SignalCandleClosePrice")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<int>("SignalType")
                        .HasColumnType("int");

                    b.Property<decimal>("Sma100")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Sma200")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Sma21")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Sma50")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("Symbol")
                        .HasColumnType("int");

                    b.Property<string>("SymbolName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ThirdLastCandleVolume")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("TradeResultType")
                        .HasColumnType("int");

                    b.Property<int>("TradeType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Trades");
                });
#pragma warning restore 612, 618
        }
    }
}
