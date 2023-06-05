using Coravel.Invocable;
using CryptoDataCollector.BussinesExtensions.Helper;
using CryptoDataCollector.CheckForSignall;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyProject.Handlers;
using Newtonsoft.Json;
using RepoDb;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Data;

namespace CryptoDataCollector.HostedServices
{
    public class SaveToDbBackgroundIInvokable : IInvocable

    {
        public ApplicationDbContext _context { get; set; }
        public IMediator _mediator { get; set; }
        public readonly IDbConnection _dbConnection;
        public List<SmaSignalCheckingModel> List { get; set; } = new List<SmaSignalCheckingModel>();
        public DateTime LastRequestDateTime { get; set; } = DateTime.Now;
        public int _symbol { get; set; } = (int)Symbol.Bnb;
        public int _addTicks { get; set; } = 3600 * 48;
        public int _timeFrame { get; set; } = 1;
        public DateTime _to { get; set; } = new DateTime(2023, 6, 2, 0, 0, 0);
        //   public DateTime _to { get; set; } = new DateTime(2020, 3, 1, 0, 0, 0);
        public DateTime _getFrom { get; set; } = new DateTime(2030, 1, 1, 0, 0, 0);
        public SaveToDbBackgroundIInvokable(ApplicationDbContext context, IDbConnection dbConnection, IMediator mediator)
        {
            _context = context;
            _dbConnection = dbConnection;
            _mediator = mediator;
        }

        public async Task Invoke()
        {
            await Execute();
            return;
        }
        public async Task Execute()
        {



            await Init();
            //Console.WriteLine("start at:" + DateTime.Now);
            //return;
            bool start = true;
            int sleep = 10 * 1000;
            var now = DateTime.Now;
            var to = SecondsFromDate(_to);
            var from = to - _addTicks;


            while (start)
            {
                try
                {

                    var chart = new Chart();
                    int triedNumber = 1;


                    GetChartFromAPI(ref to, ref from, ref chart, ref triedNumber);

                    if (triedNumber > 2) // 2 times
                        continue;

                    Console.WriteLine("now: " + DateTime.Now.ToString() + " || Symbol: " + _symbol + " || Count: " + chart.t.Count + " || from : " + DateFromTicks(chart.t.First()) + " || to: " + DateFromTicks(chart.t.Last()));

                    SaveCandles(chart);

                    from = from + _addTicks;  //  86400 - 600;
                    to = to + _addTicks;// 86400 - 600;    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(sleep);
                }

            }

            //await Task.Delay(10000, stoppingToken);

            return;
        }
        private void SaveCandles(Chart chart)
        {

            try
            {


                var candles = new List<Candle>();
                for (int i = 0; i < chart.t.Count; i++)
                {
                    candles.Add(new Candle()
                    {
                        Open = chart.o[i],
                        High = chart.h[i],
                        Low = chart.l[i],
                        Close = chart.c[i],
                        Symbol = (int)_symbol,
                        SymbolName = ((Symbol)_symbol).GetEnumDescription(),
                        DateTime = DateFromTicks(chart.t[i]),
                        Ticks = chart.t[i],
                        Volume = chart.v[i],
                    });
                }

                    ((SqlConnection)_dbConnection).BulkMergeAsync("dbo.Candles", candles, x => new { x.Symbol, x.Ticks },
                         bulkCopyTimeout: 60 * 60,
                         batchSize: 5000
                        );
                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");

            }
            catch (Exception ex)
            {
                Console.WriteLine("########################################################################################################");
                Console.WriteLine("########################################################################################################");
                Console.WriteLine("########################################################################################################");
                throw;
            }

        }

        private void GetChartFromAPI(ref long to, ref long from, ref Chart? chart, ref int triedNumber)
        {
            while (chart is null || chart.s is null || !chart.s.Contains("ok"))
            {
                if (triedNumber > 2) // 2 times
                    break;


                var uri = string.Empty;
                var ticks = ChangeSymbol(from, to);
                from = ticks.Item1;
                to = ticks.Item2;




                if (_symbol == 1)
                    uri = @$"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:BNBUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 2)
                    uri = @$"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:ADAUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 3)
                    uri = @$"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:ATOMUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 4)
                    uri = $@"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:XRPUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 5)
                    uri = $@"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:SOLUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 6)
                    uri = $@"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:ETHUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 7)
                    uri = $@"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:BTCUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 8)
                    // uri = @$"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:BNBUSDT&resolution={_timeFrame}&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";

                    Thread.Sleep(60000);

                using (var webClient = new System.Net.WebClient())
                {
                    var json = webClient.DownloadString(uri);
                    if (json != null && json.Contains("ok"))
                    {
                        chart = JsonConvert.DeserializeObject<Chart>(json);
                        var lastDateTime = DateFromTicks(chart.t.Last()).AddMinutes(210);


                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }

                    triedNumber++;
                }
            }
        }

        private (long, long) ChangeSymbol(long from, long to)
        {
            //   var dt = DateFromTicks(to);
            var dt = DateFromTicks(from);
            if (dt > DateTime.Now.AddMinutes(-10))
            // if (dt > _getFrom)
            {
                _symbol++;
                var toTicks = SecondsFromDate(_to);
                var fromTicks = toTicks - _addTicks;
                List.Clear();
                return (fromTicks, toTicks);
            }
            return (from, to);
        }

        private async Task Init()
        {
            var signals = await _context.Signals.ToListAsync();
            _context.RemoveRange(signals);
            var signal = new Signal() { GeneralStatus = Enums.GeneralStaus.Null, Profit = 0, Loss = 0 };
            _context.Signals.Add(signal);
            var trades = _context.Trades.ToList();
            var tradesForDelete = new List<Trade>();
            foreach (var item in trades)
            {
                if (item.TradeResultType == Enums.TradeResultType.Holding || item.TradeResultType == Enums.TradeResultType.Pending)
                    item.TradeResultType = Enums.TradeResultType.ForceStop;

                if (item.SellTime is null)
                    tradesForDelete.Add(item);
            }
            if (tradesForDelete.Count > 0)
                _context.Trades.RemoveRange(tradesForDelete);
            await _context.SaveChangesAsync();
        }

        public static DateTime DateFromTicks(long ticks)
        {
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return baseDate.AddMilliseconds(ticks * 1000);
        }

        public static long SecondsFromDate(DateTime date)
        {
            //date = date.Date;
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return (long)date.Subtract(baseDate).TotalSeconds;
        }

        public static List<Quote> CreateQuotes(Chart chart)
        {
            var quotes = new List<Quote>();
            int indexChartQuote = 0;
            foreach (var item in chart.t)
            {

                quotes.Add(new Quote()
                {
                    Open = chart.o[indexChartQuote],
                    High = chart.h[indexChartQuote],
                    Low = chart.l[indexChartQuote],
                    Close = chart.c[indexChartQuote],
                    Date = DateFromTicks(chart.t[indexChartQuote]).AddMinutes(210),
                    Volume = chart.v[indexChartQuote],
                });
                indexChartQuote++;
            }


            return quotes;
        }
    }
}
