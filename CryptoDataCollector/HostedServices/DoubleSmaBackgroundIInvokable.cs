using Coravel.Invocable;
using CryptoDataCollector.CheckForSignall;
using CryptoDataCollector.Data;
using CryptoDataCollector.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using System.Data;

namespace CryptoDataCollector.HostedServices
{
    public class DoubleSmaBackgroundIInvokable : IInvocable

    {
        public ApplicationDbContext _context { get; set; }
        public readonly IDbConnection _dbConnection;
        public List<SmaSignalCheckingModel> List { get; set; } = new List<SmaSignalCheckingModel>();
        public DateTime LastRequestDateTime { get; set; } = DateTime.Now;
        public int _symbol { get; set; } = 7;
        public DateTime _to { get; set; } = new DateTime(2023, 3, 7, 0, 10, 0);

        public DoubleSmaBackgroundIInvokable(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
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
            var to = SecondsFromDate(now.AddMinutes(30).AddDays(-1).AddHours(-13));

            //custom period time
            to = SecondsFromDate(_to);
            to = to - 12600;
            var from = to - 86400 - 12600 - 600;

            var startFrom = from;


            while (start)
            {
                try
                {

                    var chart = new Chart();
                    int triedNumber = 1;



                    var candles = _context.Candles.Where(p => p.Ticks >= from && p.Ticks <= to && p.Symbol == _symbol).OrderBy(p => p.Ticks).ToList();
                    chart.o = new List<decimal>();
                    chart.h = new List<decimal>();
                    chart.l = new List<decimal>();
                    chart.c = new List<decimal>();
                    chart.v = new List<decimal>();
                    chart.t = new List<long>();
                    chart.o.AddRange(candles.Select(p => p.Open).ToList());
                    chart.h.AddRange(candles.Select(p => p.High).ToList());
                    chart.l.AddRange(candles.Select(p => p.Low).ToList());
                    chart.c.AddRange(candles.Select(p => p.Close).ToList());
                    chart.v.AddRange(candles.Select(p => p.Volume).ToList());
                    chart.t.AddRange(candles.Select(p => p.Ticks).ToList());

                    if (triedNumber > 2) // 2 times
                        continue;


                    List<Quote> quotesList = new List<Quote>();
                    if (List.Count >= 208)
                    {
                        foreach (var item in List)
                        {
                            quotesList.Add(new Quote()
                            {
                                Open = item.Open,
                                Close = item.Close,
                                Low = item.Low,
                                High = item.High,
                                Date = item.DateTime,
                                Volume = item.Volume
                            });
                        }
                    }
                    quotesList.AddRange(CreateQuotes(chart));
                    quotesList = quotesList.DistinctBy(p => p.Date).ToList();

                    List<SmaResult> sma14List = quotesList.GetSma(14).ToList();
                    List<SmaResult> sma200list = quotesList.GetSma(200).ToList();
                    List<EmaResult> ema200list = quotesList.GetEma(200).ToList();
                    List<EmaResult> ema100list = quotesList.GetEma(100).ToList();

                    var listTemp = new List<SmaSignalCheckingModel>();
                    int quotesListCount = quotesList.Count;
                    var lastDatetime = DateFromTicks(to);
                    for (int i = 1; i < quotesList.Count; i++)
                    {
                        SmaResult sma14 = sma14List[i];   // evaluation period
                        SmaResult sma200 = sma200list[i];   // evaluation period
                        EmaResult ema100 = ema100list[i];   // evaluation period
                        EmaResult ema200 = ema200list[i];   // evaluation period

                        if (i <= quotesListCount)
                            listTemp.Add(new SmaSignalCheckingModel()
                            {
                                Open = quotesList[i].Open,
                                High = quotesList[i].High,
                                Low = quotesList[i].Low,
                                Close = quotesList[i].Close,
                                DateTime = quotesList[i].Date,
                                Sma14 = sma14.Sma ?? 0,
                                Sma200 = sma200.Sma ?? 0,
                                Volume = quotesList[i].Volume,
                                Ema100= ema100.Ema?? 0,
                                Ema200= ema200.Ema?? 0,

                            });
                        //     Console.WriteLine(@$"dt:{DateFromTicks(chart.t[i - 1]).AddMinutes(210)} - open: {chart.o[i - 1]}  - high: {chart.h[i - 1]}  - low: {chart.l[i - 1]}  - close: {chart.c[i - 1]}  - sma14: {sma14.Sma} - sma200: {sma200.Sma}");

                    }
                 
                    var checkSingnal = new CheckSmaSingnal(_context, _dbConnection);
                    List.AddRange(listTemp);
                    List = List.DistinctBy(p => p.DateTime).ToList();
                    var lastCandle = List.Last();
                    var lastCandleTehran = List.Last();

                    List = List.TakeLast(289).ToList();
                    Console.WriteLine("now: " + DateTime.Now.ToString() + " || from of List: " + List.First().DateTime + " and to of List: " + List.Last().DateTime);

                    for (int i = 0; i < List.Count; i++)
                    {
                        Console.WriteLine(i.ToString() + " || " + "now: " + List[i].DateTime.ToString() + " || Open : " + List[i].Open.ToString() + " || Ema200 TA: " + List[i].Ema200);

                    }
                    var resultType = checkSingnal.CheckStrategy(List, _symbol);
                    if (resultType == Enums.SignalCheckerType.Buy)
                    {
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");

                        for (int i = 0; i < 1; i++)
                        {
                            var nowBuy = DateTime.Now;
                            start = true;
                            Console.WriteLine("=================================");
                            Console.WriteLine("Buy at:" + List.OrderByDescending(p => p.DateTime).First().DateTime);
                            Console.WriteLine("=================================");
                        }

                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine("\n");

                    }
                    if (resultType != Enums.SignalCheckerType.ListEmpty && List.Count >= 208)
                    {

                        //      for gathering data 
                        from = from + 300;
                        to = to + 300;


                    }


                    // Thread.Sleep(300);
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


                if (List.Count >= 289)
                    from = to - 12600 - 1800;




                if (_symbol == 1)
                    uri = @$"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:BNBUSDT&resolution=5&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 2)
                    uri = @$"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:ADAUSDT&resolution=5&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 3)
                    uri = @$"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:ATOMUSDT&resolution=5&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                if (_symbol == 4)
                    uri = $@"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:XRPUSDT&resolution=5&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 5)
                    uri = $@"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:SOLUSDT&resolution=5&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 6)
                    uri = $@"http://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:ETHUSDT&resolution=5&from={from}&to={to}&token=cei44eiad3idnd1tlspgcei44eiad3idnd1tlsq0";
                else if (_symbol == 7)
                    Thread.Sleep(60000);

                using (var webClient = new System.Net.WebClient())
                {
                    var json = webClient.DownloadString(uri);
                    if (json != null && json.Contains("ok"))
                    {
                        chart = JsonConvert.DeserializeObject<Chart>(json);
                        var lastDateTime = DateFromTicks(chart.t.Last()).AddMinutes(210);

                        if (lastDateTime == LastRequestDateTime)
                        {
                            Thread.Sleep(5000);
                        }
                        LastRequestDateTime = lastDateTime;
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
            if (DateFromTicks(to) > DateTime.Now.AddMinutes(-10))
            {
                _symbol++;
                var toTicks = SecondsFromDate(_to);
                var fromTicks = toTicks - 86400 - 12600 - 600;
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
