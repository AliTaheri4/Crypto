// temporary

//using Coravel.Invocable;
//using CryptoDataCollector.CheckForSignall;
//using CryptoDataCollector.Data;
//using CryptoDataCollector.Models;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using Skender.Stock.Indicators;
//using System.Data;

//namespace CryptoDataCollector.HostedServices
//{
//    public class CciBackgroundIInvokable : IInvocable

//    {
//        public ApplicationDbContext _context { get; set; }
//        public readonly IDbConnection _dbConnection;
//        public List<CciSignalCheckingModel> List { get; set; } = new List<CciSignalCheckingModel>();
//        public DateTime LastRequestDateTime { get; set; } = DateTime.Now;
//        public int _symbol { get; set; } = 1;
//        public DateTime _to { get; set; } = new DateTime(2022, 8, 1, 1, 5, 0);
//        public bool DataFromApi = false;
//        public bool SavingData = false;
//        public CciBackgroundIInvokable(ApplicationDbContext context, IDbConnection dbConnection)
//        {
//            _context = context;
//            _dbConnection = dbConnection;
//            if (SavingData)
//                DataFromApi = true;

//        }

//        public async Task Invoke()
//        {
//            await Execute();
//            return;
//        }
//        public async Task Execute()
//        {



//            await Init();
//            //Console.WriteLine("start at:" + DateTime.Now);
//            //return;
//            bool start = true;
//            int sleep = 10 * 1000;
//            var now = DateTime.Now;
//            var to = SecondsFromDate(now.AddMinutes(30).AddDays(-1).AddHours(-13));

//            //custom period time
//            to = SecondsFromDate(_to);
//            to = to - 12600;
//            var from = to - 86400 ;

//            var startFrom = from;


//            while (start)
//            {
//                try
//                {

//                    var chart = new Chart();
//                    int triedNumber = 1;

//                    var dtFrom = DateFromTicks(from);
//                    var dtTo = DateFromTicks(to);
               
//                        var candles = _context.Candles.Where(p => p.Ticks >= from && p.Ticks <= to && p.Symbol == _symbol).OrderBy(p => p.Ticks).ToList();
//                        chart.o = new List<decimal>();
//                        chart.h = new List<decimal>();
//                        chart.l = new List<decimal>();
//                        chart.c = new List<decimal>();
//                        chart.v = new List<decimal>();
//                        chart.t = new List<long>();
//                        chart.o.AddRange(candles.Select(p => p.Open).ToList());
//                        chart.h.AddRange(candles.Select(p => p.High).ToList());
//                        chart.l.AddRange(candles.Select(p => p.Low).ToList());
//                        chart.c.AddRange(candles.Select(p => p.Close).ToList());
//                        chart.v.AddRange(candles.Select(p => p.Volume).ToList());
//                        chart.t.AddRange(candles.Select(p => p.Ticks).ToList());
                    
//                    if (triedNumber > 2) // 2 times
//                        continue;


//                    List<Quote> quotesList = new List<Quote>();
             
//                    quotesList.AddRange(CreateQuotes(chart));
//                    quotesList = quotesList.DistinctBy(p => p.Date).ToList();

//                    List<SmaResult> sma14List = quotesList.GetSma(14).ToList();
//                    List<SmaResult> sma200list = quotesList.GetSma(200).ToList();
//                    List<CciResult> cci200list = quotesList.GetCci(20).ToList();

//                    var listTemp = new List<CciSignalCheckingModel>();
//                    int quotesListCount = quotesList.Count;
//                    var lastDatetime = DateFromTicks(to);
//                    for (int i = 1; i < quotesList.Count; i++)
//                    {
//                        CciResult cci200 = cci200list[i];  

//                        if (i <= quotesListCount)
//                            listTemp.Add(new CciSignalCheckingModel()
//                            {
//                                Open = quotesList[i].Open,
//                                High = quotesList[i].High,
//                                Low = quotesList[i].Low,
//                                Close = quotesList[i].Close,
//                                DateTime = quotesList[i].Date.AddMinutes(210),
//                                Cci = cci200.Cci ?? 0,
//                                Volume = quotesList[i].Volume

//                            });
//                        //     Console.WriteLine(@$"dt:{DateFromTicks(chart.t[i - 1]).AddMinutes(210)} - open: {chart.o[i - 1]}  - high: {chart.h[i - 1]}  - low: {chart.l[i - 1]}  - close: {chart.c[i - 1]}  - sma14: {sma14.Sma} - sma200: {sma200.Sma}");

//                    }

//                    var checkSingnal = new CheckCciSingnal(_context, _dbConnection);
//                    List.AddRange(listTemp);
//                    List = List.DistinctBy(p => p.DateTime).ToList();
//                    var lastCandle = List.Last();
//                    var lastCandleTehran = List.Last();

//                    List = List.TakeLast(289).ToList();
//                    Console.WriteLine("now: " + DateTime.Now.ToString() + " || from of List: " + List.First().DateTime + " and to of List: " + List.Last().DateTime);
//                    var resultType = checkSingnal.CheckStrategy(List, _symbol);
//                    if (resultType == Enums.SignalCheckerType.Buy)
//                    {
//                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");

//                        for (int i = 0; i < 1; i++)
//                        {
//                            var nowBuy = DateTime.Now;
//                            start = true;
//                            Console.WriteLine("=================================");
//                            Console.WriteLine("Buy at:" + List.OrderByDescending(p => p.DateTime).First().DateTime);
//                            Console.WriteLine("=================================");
//                        }

//                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");
//                        Console.WriteLine("\n");

//                    }
//                    if (resultType != Enums.SignalCheckerType.ListEmpty && List.Count >= 208)
//                    {
                    
//                            //      for gathering data 
//                            from = from + 300;
//                            to = to + 300;
                        
//                    }


//                    // Thread.Sleep(300);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.Message);
//                    Thread.Sleep(sleep);
//                }

//            }

//            //await Task.Delay(10000, stoppingToken);

//            return;
//        }

  

//        private (long, long) ChangeSymbol(long from, long to)
//        {
//            if (DateFromTicks(to) > DateTime.Now.AddMinutes(-10))
//            {
//                _symbol++;
//                var toTicks = SecondsFromDate(_to);
//                var fromTicks = toTicks - 86400 - 12600 - 600;
//                List.Clear();
//                return (fromTicks, toTicks);
//            }
//            return (from, to);
//        }

//        private async Task Init()
//        {
//            var signals = await _context.Signals.ToListAsync();
//            _context.RemoveRange(signals);
//            var signal = new Signal() { GeneralStatus = Enums.GeneralStaus.Null, Profit = 0, Loss = 0 };
//            _context.Signals.Add(signal);
//            var trades = _context.Trades.ToList();
//            var tradesForDelete = new List<Trade>();
//            foreach (var item in trades)
//            {
//                if (item.TradeResultType == Enums.TradeResultType.Holding || item.TradeResultType == Enums.TradeResultType.Pending)
//                    item.TradeResultType = Enums.TradeResultType.ForceStop;

//                if (item.SellTime is null)
//                    tradesForDelete.Add(item);
//            }
//            if (tradesForDelete.Count > 0)
//                _context.Trades.RemoveRange(tradesForDelete);
//            await _context.SaveChangesAsync();
//        }

//        public static DateTime DateFromTicks(long ticks)
//        {
//            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//            return baseDate.AddMilliseconds(ticks * 1000);
//        }

//        public static long SecondsFromDate(DateTime date)
//        {
//            //date = date.Date;
//            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
//            return (long)date.Subtract(baseDate).TotalSeconds;
//        }

//        public static List<Quote> CreateQuotes(Chart chart)
//        {
//            var quotes = new List<Quote>();
//            int indexChartQuote = 0;
//            foreach (var item in chart.t)
//            {

//                quotes.Add(new Quote()
//                {
//                    Open = chart.o[indexChartQuote],
//                    High = chart.h[indexChartQuote],
//                    Low = chart.l[indexChartQuote],
//                    Close = chart.c[indexChartQuote],
//                    Date = DateFromTicks(chart.t[indexChartQuote]),
//                    Volume = chart.v[indexChartQuote],
//                });
//                indexChartQuote++;
//            }
//            return quotes;
//        }
//    }
//}
