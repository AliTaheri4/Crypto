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

namespace CryptoDataCollector
{
    public class Program
    {
        public static IConfiguration _configuration;

        public static void Main(string[] args)
        {
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
                    services.AddMemoryCache();
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
                    Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .CreateLogger();

                    var currentConnectionString = hostContext.Configuration.GetConnectionString("Crypto");
                    services.AddTransient<IDbConnection>(x => new SqlConnection(currentConnectionString));

                    services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
                    {
                        dbContextOptions.UseSqlServer(currentConnectionString, x => x.EnableRetryOnFailure(3).CommandTimeout(120));
                        dbContextOptions.EnableSensitiveDataLogging(false);
                        //if (!_isProduction)
                        // dbContextOptions.UseLoggerFactory(LoggerFactory.Create(c => c.AddConsole())).EnableSensitiveDataLogging();
                    }, ServiceLifetime.Scoped);
                    services.AddHostedService<Service>();
                    services.AddMediatR(Assembly.GetExecutingAssembly());
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
                                  scheduler.Schedule<SaveCandlesBackgroundInvokable>().Cron("* * * * *");

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
    public class Chart
    {

        public List<decimal> o { get; set; }
        public List<decimal> h { get; set; }
        public List<decimal> l { get; set; }
        public List<decimal> c { get; set; }
        public List<long> t { get; set; }
        public List<decimal> v { get; set; }
        public string s { get; set; }
    }
}
