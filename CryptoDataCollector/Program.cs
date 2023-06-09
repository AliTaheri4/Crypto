using CryptoDataCollector.HostedServices;
using Coravel;
using CryptoDataCollector.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Data;
using MediatR;
using System.Reflection;
using Services.MainServices;
using Services.HostedServices;
using CryptoDataCollector.Enums;
using Domain.Data;
using Services.BybitServices;

namespace CryptoDataCollector
{
    public class Program
    {
        public static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            var symbol = Symbol.BTC;
            Console.WriteLine($@"{symbol} sdxs");
            string appSettingFileName = "appsettings.json";

            var hostBuilder = Host.CreateDefaultBuilder(args);
            Console.WriteLine(appSettingFileName);

            _configuration = new ConfigurationBuilder()
                    .AddJsonFile(appSettingFileName)
                    .Build();

            var builder = CreateHostBuilder(args).Build();//.Run();

            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(_configuration)
                    .MinimumLevel.Error()
                    .CreateLogger();
            RepoDb.SqlServerBootstrap.Initialize();




            builder.Run();

        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var justSaveHistoryCandlesFromService = _configuration.GetValue<bool>("JustSaveHistoryCandlesFromService");

            var builder = Host.CreateDefaultBuilder(args)
             .ConfigureLogging(builder =>
             {
                 builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);
                 builder.AddFilter("Microsoft", LogLevel.Warning);
                 builder.AddFilter("System", LogLevel.Error);
             })
                .ConfigureServices((hostContext, services) =>
                {
                    // services.AddQuartz();
                    services.AddTransient<SaveCandlesBackgroundInvokable>();
                    services.AddTransient<GenerateLastCandleByTicksBackgroundIInvokable>();
                    services.AddTransient<DoubleEmaMacdBackgroundIInvokable>();
                    //services.AddTransient<CciBackgroundIInvokable>();
                    services.AddTransient<DoubleSmaBackgroundIInvokable>();
                    services.AddTransient<CandleStichDivergengeSRBackground>();
                    services.AddTransient<LastTwoBigCandlesBackground>();
                    services.AddTransient<LastByLuckBackground>();
                    services.AddTransient<SaveToDbBackgroundIInvokable>();
                    services.AddTransient<CalculationProsConsBackgroundIInvokable>();
                    services.AddTransient<FSPBackgroundJob>();
                    services.AddSingleton<SpFilteringFillerService>();
                    services.AddSingleton<TradeServices>();
                    services.AddSingleton<BybitFacade>();
                    Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .CreateLogger();

                    var currentConnectionString = hostContext.Configuration.GetConnectionString("Crypto");
                    services.AddTransient<IDbConnection>(x => new SqlConnection(currentConnectionString));

                    services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
                    {
                        dbContextOptions.UseSqlServer(currentConnectionString, x => x.EnableRetryOnFailure(3).CommandTimeout(120).MigrationsAssembly("Domain"));
                        dbContextOptions.EnableSensitiveDataLogging(false);
                        //if (!_isProduction)
                        // dbContextOptions.UseLoggerFactory(LoggerFactory.Create(c => c.AddConsole())).EnableSensitiveDataLogging();
                    }, ServiceLifetime.Singleton);

                    services.AddHostedService<Service>();
                    services.AddMediatR(Assembly.GetExecutingAssembly());
                    services.AddMemoryCache();
                })
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder.ConfigureServices((hostContext, services) =>
                      {
                          services.AddRazorPages();
                          services.AddScheduler();
                      });

                      webBuilder.Configure(app =>
                      {
                          app.UseRouting();

                          app.UseAuthorization();
                          var provider = app.ApplicationServices;
                          provider.UseScheduler(scheduler =>
                          {
                              if (!justSaveHistoryCandlesFromService)
                                 // scheduler.Schedule<SaveCandlesBackgroundInvokable>().Cron("* * * * *");
                                  scheduler.Schedule<SaveCandlesBackgroundInvokable>().EveryTenSeconds();

                          });
                          app.UseEndpoints(endpoints =>
                          {
                              endpoints.MapRazorPages();
                          });
                      });
                  }).UseSerilog();

            return builder;

        }
    }
}
