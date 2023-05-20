using Bybit.Net.Clients;
using Bybit.Net.Objects.Models.Socket;
using Bybit.Net.Objects.Models.V5;
using CryptoDataCollector;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using CryptoExchange.Net.Sockets;
using MediatR;
using MediatR.Wrappers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyProject.Models;
using Newtonsoft.Json;
using RepoDb;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Handlers
{
    public class SaveCandlesCommand : IRequest
    {

        public SaveCandlesCommand()
        {
        }

    }

    public class SaveCandlesHandler : AsyncRequestHandler<SaveCandlesCommand>
    {
        public ApplicationDbContext _context { get; set; }
        public IMediator _mediator { get; set; }
        public readonly IDbConnection _dbConnection;
        public readonly IConfiguration _configuration;
        public List<TickModel> TicksList { get; set; } = new List<TickModel>();
        public static List<TickModel> AllList { get; set; } = new List<TickModel>();
        public List<SmaSignalCheckingModel> List { get; set; } = new List<SmaSignalCheckingModel>();
        public DateTime LastRequestDateTime { get; set; } = DateTime.Now;
        public int _symbol { get; set; } = (int)Symbol.Bnb;
        public int _addTicks { get; set; } = 3600 * 4;
        public int _timeFrame { get; set; } = 1;
        public DateTime _to { get; set; } = DateTime.Now; //new DateTime(2023, 5, 7, 0, 0, 0);
        //   public DateTime _to { get; set; } = new DateTime(2020, 3, 1, 0, 0, 0);
        public DateTime _getFrom { get; set; } = new DateTime(2030, 1, 1, 0, 0, 0);
        public bool DoGetTicksData { get; set; }
        public SaveCandlesHandler(ApplicationDbContext context, IDbConnection dbConnection, IMediator mediator, IConfiguration configuration)
        {
            _context = context;
            _dbConnection = dbConnection;
            _mediator = mediator;
            _configuration = configuration;
            DoGetTicksData = _configuration.GetValue<bool>("DoGetTicksData");
            if (!DoGetTicksData)
            {
                _to = new DateTime(2023, 5, 8, 0, 0, 0);
                _addTicks = 3600 * 6;
            }
        }
        protected override async Task Handle(SaveCandlesCommand request, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(GetHistoryCandles());
            if (DoGetTicksData)
                tasks.Add(GenerateLatestCandle());

            await Task.WhenAll(tasks);
        }


        public async Task GetHistoryCandles()
        {
            var symbols = GetAllSymbols();
            var tasks = new List<Task>();
            foreach (var item in symbols)
            {
                tasks.Add(Task.Factory.StartNew(() => GetHistoryCandlesBySymbol((int)item)));
                tasks.Add(Task.Factory.StartNew(() => GetHistoryCandlesBySymbol((int)item)));
            }  
      




            int numOfThreads = tasks.Count;
            WaitHandle[] waitHandles = new WaitHandle[numOfThreads];

            for (int i = 0; i < numOfThreads; i++)
            {
                var j = i;
                // Or you can use AutoResetEvent/ManualResetEvent
                var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                var thread = new Thread( () =>
                {
                    //   tasks[0].Wait();
                    Console.WriteLine("Thread{0} exits", j);
                    handle.Set();
                });
                waitHandles[j] = handle;
                thread.Start();
            }

            //     WaitHandle[] test = new WaitHandle[1];
            //     test[0] = waitHandles[0];
            //    WaitHandle.WaitAny(test);
            Console.WriteLine("Main thread exits");




              await Task.WhenAll(tasks);
            return;
        }

        public async Task GetHistoryCandlesBySymbol(int symbol)
        {

            bool start = true;
            int sleep = 10 * 1000;
            var now = DateTime.Now;
            var to = SecondsFromDate(_to);
            var from = to - _addTicks;

            try
            {
                int triedNumber = 1;
                var ticks = ChangeSymbol(from, to);
                from = ticks.Item1;
                to = ticks.Item2;
                var res = await GetChartFromAPI(symbol, to, from, triedNumber);

                if (res == null)
                    return;

            
                Console.WriteLine("now: " + DateTime.Now.ToString() + " || Symbol: " + symbol + " || Count: " + res.t.Count + " || from : " + DateFromTicks(res.t.First()) + " || to: " + DateFromTicks(res.t.Last()));

                SaveCandles(res);
                symbol++;

                from = from + _addTicks;  //  86400 - 600;
                to = to + _addTicks;// 86400 - 600;    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(sleep);
            }


        }
        private async Task<Chart> GetChartFromAPI(int _symbol, long to, long from, int triedNumber)
        {
            var uri = string.Empty;

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
                return null;

            // Thread.Sleep(5000);

            using (var webClient = new WebDownload())
            {

                var client = new HttpClient();
                client.Timeout = new TimeSpan(40000);
                var response = await client.GetAsync(uri);
                var res = JsonConvert.DeserializeObject<Chart>(await response.Content.ReadAsStringAsync());
                return res;
                //response.EnsureSuccessStatusCode();
                //var result = await response.Content.ReadAsStringAsync();


                //var json = webClient.DownloadString(uri);
                //if (json != null && json.Contains("ok"))
                //{
                //    var res = JsonConvert.DeserializeObject<Chart>(json);
                //    var lastDateTime = DateFromTicks(res.t.Last()).AddMinutes(210);
                //    return res;

                //}
                //else
                //{
                //    //        Thread.Sleep(5000);
                //}

                triedNumber++;


            }
            return null;

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
        public List<Symbol> GetAllSymbols()
        {
            var values = Enum.GetValues(typeof(Symbol));
            var list = new List<Symbol>();
            foreach (var item in values)
            {
                list.Add((Symbol)item);
            }
            return list;
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
            var signal = new Signal() { GeneralStatus = GeneralStaus.Null, Profit = 0, Loss = 0 };
            _context.Signals.Add(signal);
            var trades = _context.Trades.ToList();
            var tradesForDelete = new List<Trade>();
            foreach (var item in trades)
            {
                if (item.TradeResultType == TradeResultType.Holding || item.TradeResultType == TradeResultType.Pending)
                    item.TradeResultType = TradeResultType.ForceStop;

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
        public async Task GenerateLatestCandle()
        {
            Thread.Sleep(50000);
            var now = DateTime.Now;
            var lastItemDt = AllList.Last().DateTime;
            var dt = new DateTime(lastItemDt.Year, lastItemDt.Month, lastItemDt.Day, lastItemDt.Hour, lastItemDt.Minute, 0);
            AllList = AllList.Where(p => p.DateTime >= now.AddMinutes(-210).AddSeconds(-now.Second) && p.DateTime < now.AddMinutes(-210).AddSeconds(-now.Second).AddMinutes(1)).OrderBy(p => p.DateTime).ToList();
            var groupby = AllList.GroupBy(p => p.Symbol).Select(p => new Candle()
            {
                Open = p.OrderBy(o => o.DateTime).First().Price,
                High = p.Max(p => p.Price),
                Low = p.Min(p => p.Price),
                Close = p.OrderBy(p => p.DateTime).Last().Price,
                DateTime = dt,
                Symbol = (int)p.Key,
                SymbolName = p.Key.GetEnumDescription(),
                Ticks = SecondsFromDate(dt),
                Volume = 1
            }).ToList();



            await ((SqlConnection)_dbConnection).BulkMergeAsync("dbo.Candles", groupby, x => new { x.Symbol, x.Ticks },
                       bulkCopyTimeout: 60 * 60,
                       batchSize: 5000
                      );

            AllList.Clear();
            return;
        }


    }
    public class WebDownload : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public WebDownload() : this(30000) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }
}