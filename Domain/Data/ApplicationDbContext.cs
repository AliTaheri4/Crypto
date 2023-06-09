using CryptoDataCollector.Data;
using Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Data
{
    internal static class IConfigurationRootExtensions
    {
        public static IConfigurationBuilder AddBasePath(this IConfigurationBuilder builder)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var startupProjectPath = Path.Combine(currentDirectory, "../CryptoDataCollector");
            var basePathConfiguration = Directory.Exists(startupProjectPath) ? startupProjectPath : currentDirectory;

            return builder.SetBasePath(basePathConfiguration);
        }
    }
    public class ApplicationDbContext : DbContext
    {

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(true, cancellationToken));
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
           bool acceptAllChangesOnSuccess,
           CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess,
                          cancellationToken));
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var now = DateTime.Now;
            var user = "Code"; //_accessor?.HttpContext?.User?.Identity?.Name ?? "Code";

            foreach (var entry in entries)
            {
                // for entities that inherit from BaseEntity,
                // set UpdatedOn / CreatedOn appropriately
                if (entry.Entity is BaseEntity trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // set the updated date to "now"
                            trackable.ModifiedTime = now;
                            trackable.ModifiedName = user;
                            // mark property as "don't touch"
                            // we don't want to update on a Modify operation
                            entry.Property("CreatedTime").IsModified = false;
                            entry.Property("CreatedName").IsModified = false;

                            break;

                        case EntityState.Added:
                            // set both updated and created date to "now"
                            trackable.CreatedTime = now;
                            trackable.CreatedName = user;
                            trackable.ModifiedName = user;
                            trackable.ModifiedTime = now;
                            break;
                    }
                }
            }
        }
        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var config = new ConfigurationBuilder().AddBasePath().AddJsonFile("appsettings.json").AddCommandLine(args).Build();
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(config.GetConnectionString("Crypto"));
                //optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=Crypto; Trusted_Connection=True; MultipleActiveResultSets=true;TrustServerCertificate=true");

                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Signal> Signals { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Candle> Candles { get; set; }
        public DbSet<Asset> Assets { get; set; }
    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //=> optionsBuilder.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Debug)));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Signal>().Property(p => p.Profit).HasPrecision(18, 8);
            modelBuilder.Entity<Signal>().Property(p => p.Loss).HasPrecision(18, 8);
        
            modelBuilder.Entity<Trade>().Property(p => p.Buy).HasPrecision(18, 8);
            modelBuilder.Entity<Trade>().Property(p => p.Sell).HasPrecision(18, 8);

            modelBuilder.Entity<Trade>().Property(p => p.Profit).HasPrecision(18, 8);
            modelBuilder.Entity<Trade>().Property(p => p.Loss).HasPrecision(18, 8);
            modelBuilder.Entity<Trade>().Property(p => p.SignalCandleClosePrice).HasPrecision(18, 8);

            modelBuilder.Entity<Candle>().Property(p => p.Open).HasPrecision(18, 8);
            modelBuilder.Entity<Candle>().Property(p => p.High).HasPrecision(18, 8);
            modelBuilder.Entity<Candle>().Property(p => p.Low).HasPrecision(18, 8);
            modelBuilder.Entity<Candle>().Property(p => p.Close).HasPrecision(18, 8);
            modelBuilder.Entity<Candle>().Property(p => p.Volume).HasPrecision(18, 8);



            modelBuilder.Entity<Trade>().Property(p => p.OpenLast).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.HighLast).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.LowLast).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.CloseLast).HasPrecision(18, 4);

            modelBuilder.Entity<Trade>().Property(p => p.OpenThird).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.HighThird).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.LowThird).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.CloseThird).HasPrecision(18, 4);

            modelBuilder.Entity<Trade>().Property(p => p.OpenForth).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.HighForth).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.LowForth).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.CloseForth).HasPrecision(18, 4);

            modelBuilder.Entity<Trade>().Property(p => p.OpenCurrent).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.HighCurrent).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.LowCurrent).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.CloseCurrent).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Indicator1).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Indicator2).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Indicator3).HasPrecision(18, 4);

            modelBuilder.Entity<Trade>().Property(p => p.ThirdLastCandleVolume).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.ForthLastCandleVolume).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Sma21).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Sma50).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Sma100).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Sma200).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Ema21).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Ema50).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Ema100).HasPrecision(18, 4);
            modelBuilder.Entity<Trade>().Property(p => p.Ema200).HasPrecision(18, 4);
        
        }
    }
}
