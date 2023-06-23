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
using System.Security.Cryptography.Xml;

namespace CryptoDataCollector
{
    public class Person
    {
        public List<Person> GetPeople()
        {
            var now=DateTime.Now;
            return new List<Person> { 
                new Person() { Name="ali"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute,4)}, 
                new Person() { Name="ali2"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+1,0)}, 
                new Person() { Name="ali3"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+2,0)}, 
                new Person() { Name="ali4"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute,0)}, 
                new Person() { Name="ali5"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute,44)}, 
                new Person() { Name="ali6"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+4,0)}, 
                new Person() { Name="ali7"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+2,45)}, 
                new Person() { Name="ali8"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+5,0)}, 
                new Person() { Name="ali9"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+5,5)},
                new Person() { Name="ali10"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+6,0)}, 
                new Person() { Name="ali11"  , RegisterdTime=new DateTime(now.Year,now.Month,now.Day,now.Hour,now.Minute+6,0)}, 
            
            };
        }
        public string Name { get; set; }
        public DateTime RegisterdTime { get; set; }
    }
    public class Program
    {
        public static IConfiguration _configuration;

        public static DateTime RemoceSecondTicks(DateTime dateTime, bool toPersianDt = true)
        {
            var dt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
            if (toPersianDt)
                dt = dt.AddMinutes(-210);

            return dt;
        }

        public static void Main(string[] args)
        {
       //     var person = new Person();
      //      var gb = person.GetPeople().GroupBy(p => new { DateTime = RemoceSecondTicks(p.RegisterdTime) }).Select(p => new { Ali = p.First().Name, Dt = p.Count() }).ToList();
            string appSettingFileName = "appsettings.json";

            var hostBuilder = Host.CreateDefaultBuilder(args);
     //       Console.WriteLine(appSettingFileName);

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
