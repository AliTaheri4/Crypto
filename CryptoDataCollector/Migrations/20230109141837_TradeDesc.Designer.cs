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
    [Migration("20230109141837_TradeDesc")]
    partial class TradeDesc
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

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

                    b.Property<bool>("IsEmotional")
                        .HasColumnType("bit");

                    b.Property<int>("Leverage")
                        .HasColumnType("int");

                    b.Property<decimal>("Loss")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.Property<string>("ModifiedName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("NeedingInRangeCandles")
                        .HasColumnType("int");

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

                    b.Property<double>("Sma200")
                        .HasColumnType("float");

                    b.Property<int>("Symbol")
                        .HasColumnType("int");

                    b.Property<string>("SymbolName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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
