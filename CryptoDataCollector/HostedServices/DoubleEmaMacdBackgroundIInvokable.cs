using Coravel.Invocable;
using CryptoDataCollector.BussinesExtensions;
using CryptoDataCollector.BussinesExtensions.Helper;
using CryptoDataCollector.CheckForSignall;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Domain.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using System.Data;

namespace CryptoDataCollector.HostedServices
{
    public class DoubleEmaMacdBackgroundIInvokable : IInvocable
    {
        public ApplicationDbContext _context { get; set; }
        public readonly IDbConnection _dbConnection;
        public List<DoubleEmaMacdSignalCheckingModel> List { get; set; } = new List<DoubleEmaMacdSignalCheckingModel>();
        public List<Candle> CandleList { get; set; } = new List<Candle>();
        public DateTime LastRequestDateTime { get; set; } = DateTime.MinValue;
        public DateTime FirstRequestDateTime { get; set; } = DateTime.MinValue;
        public TimeFrameType _timeFrame { get; set; } = TimeFrameType.Hour1;

        public int _symbol { get; set; } = (int)Symbol.BTC;
        public DateTime _to { get; set; } = new DateTime(2022, 6, 1, 0, 30, 0);

        public DoubleEmaMacdBackgroundIInvokable(ApplicationDbContext context, IDbConnection dbConnection)
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


            //custom period time
            var to = SecondsFromDate(_to);
            to = to - 12600;// - 2700000;
            int lookBack = 1200;// 1000;
            var from = to - (60 * 60 * lookBack);

            var startFrom = from;
            var test = DateFromTicks(from);

            int result = SignalExtensions.GetTimeFrameType(_timeFrame);

            bool isInit = true;
            int skip = 0;
            while (start)
            {
                try
                {

                    var chart = new Chart();
                    int triedNumber = 1;


                    var symbolCandle = new List<Candle>();
                    if ((symbolCandle is not null && symbolCandle.Count > 0 && symbolCandle.Last().DateTime == LastRequestDateTime) || _symbol == 8)
                        Thread.Sleep(30000);

                    if (CandleList is null || CandleList.Count == 0 || LastRequestDateTime >= CandleList.Last().DateTime || !CandleList.Any(p => p.Symbol == _symbol))
                    {
                        skip = 0;
                        if (!isInit)
                            ChangeSymbol(from, to);

                        CandleList = _context.Candles.Where(p => p.Symbol == _symbol && p.DateTime >= DateFromTicks(from)).OrderBy(p => p.Ticks).ToList();
                        isInit = false;
                    }
                    SignalExtensions.GroupedByTimeFrame(CandleList, result);

                    symbolCandle = CandleList.Skip(skip - 1).Take(lookBack).ToList();
                    //  symbolCandle = CandleList.ToList();


                    skip++;
                    FirstRequestDateTime = symbolCandle.First().DateTime;

                    ChartFilling(chart, symbolCandle);

                    if (triedNumber > 2) // 2 times
                        continue;


                    List<Quote> quotesList = new List<Quote>();

                    quotesList.AddRange(CreateQuotes(chart));
                    quotesList = quotesList.DistinctBy(p => p.Date).ToList();

                    List<MacdResult> macdList = quotesList.GetMacd().ToList();
                    List<EmaResult> ema20list = quotesList.GetEma(20).ToList();
                    List<EmaResult> ema200list = quotesList.GetEma(200).ToList();
                    var ta = new UsingTaIndicators();
                    var indicators = ta.Calculate(chart.o.Select(p => p).ToArray(), chart.h.Select(p => p).ToArray(), chart.l.Select(p => p).ToArray(), chart.c.Select(p => p).ToArray(), chart.v.Select(p => p).ToArray(), chart.t.Select(p => DateFromTicks(p)).ToArray());




                    var listTemp = new List<DoubleEmaMacdSignalCheckingModel>();
                    int quotesListCount = quotesList.Count;
                    var lastDatetime = DateFromTicks(to);
                    for (int i = 0; i < quotesList.Count; i++)
                    {
                        MacdResult macd = macdList[i];   // evaluation period
                        EmaResult ema20 = ema20list[i];   // evaluation period
                        EmaResult ema200 = ema200list[i];   // evaluation period


                        if (i <= quotesListCount)
                            listTemp.Add(new DoubleEmaMacdSignalCheckingModel()
                            {
                                Open = quotesList[i].Open,
                                High = quotesList[i].High,
                                Low = quotesList[i].Low,
                                Close = quotesList[i].Close,
                                DateTime = quotesList[i].Date,
                                Volume = quotesList[i].Volume,
                                Ema20 = ema20.Ema ?? 0,
                                Ema200 = ema200.Ema ?? 0,
                                TalibEma200 = (double)indicators.Ema200[i],
                                MacdLine = macd.Macd ?? 0,
                                MacdSignalLine = macd.Signal ?? 0,

                            });

                    }


                    var checkSingnal = new CheckDoubleEmaMacdSingnal(_context, _dbConnection);
                    List.AddRange(listTemp);
                    List = List.DistinctBy(p => p.DateTime).ToList();
                    var lastCandle = List.Last();
                    var lastCandleTehran = List.Last();
                    LastRequestDateTime = lastCandle.DateTime;

                    List = List.OrderBy(p => p.DateTime).TakeLast(289).ToList();
                    //for (int i = 0; i < List.Count; i++)
                    //{
                    //    Console.WriteLine(i.ToString() + " || " + "now: " + List[i].DateTime.ToString() + " || Open : " + List[i].Open.ToString() + " || High : " + List[i].High.ToString() + " || Low: " + List[i].Low.ToString() + " || Close : " + List[i].Close.ToString() + " || Ema200 TA: " + indicators.Ema200[i] + " || Ema200 Qoute: " + List[i].Ema200);

                    //}
                    Console.WriteLine("now: " + DateTime.Now.ToString() + " || from of List: " + List.First().DateTime + " and to of List: " + List.Last().DateTime);

                    var lastTake = List.TakeLast(10).ToList();

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
                        //from = from + 60 * 60;
                        //to = to + 60 * 60;


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

        private static void ChartFilling(Chart chart, List<Candle>? symbolCandle)
        {
            chart.o = new List<decimal>();
            chart.h = new List<decimal>();
            chart.l = new List<decimal>();
            chart.c = new List<decimal>();
            chart.v = new List<decimal>();
            chart.t = new List<long>();
            chart.o.AddRange(symbolCandle.Select(p => p.Open).ToList());
            chart.h.AddRange(symbolCandle.Select(p => p.High).ToList());
            chart.l.AddRange(symbolCandle.Select(p => p.Low).ToList());
            chart.c.AddRange(symbolCandle.Select(p => p.Close).ToList());
            chart.v.AddRange(symbolCandle.Select(p => p.Volume).ToList());
            chart.t.AddRange(symbolCandle.Select(p => p.Ticks).ToList());
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
            //  if (tradesForDelete.Count > 0) 
            _context.Trades.RemoveRange(trades); //remove all
                                                 // _context.Trades.RemoveRange(tradesForDelete); // remove last trade that is not completed
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
