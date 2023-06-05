using Accord.IO;
using Coravel.Invocable;
using CryptoDataCollector.BussinesExtensions;
using CryptoDataCollector.BussinesExtensions.Helper;
using CryptoDataCollector.CheckForSignall;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.MainServices;
using Skender.Stock.Indicators;
using System.Data;
using System.Xml.Linq;

namespace CryptoDataCollector.HostedServices
{
    public class LastTwoBigCandlesBackground : IInvocable

    {
        public TradeServices _tradeServices { get; set; }
        public ApplicationDbContext _context { get; set; }
        public readonly IDbConnection _dbConnection;
        public List<CandleStichDivergengeSRSignalCheckingModel> List { get; set; } = new List<CandleStichDivergengeSRSignalCheckingModel>();
        public int _symbol { get; set; } = (int)Symbol.BTC;
        public SignalType _signalType { get; set; } = SignalType.LastTwoBigCandles;
        public static TimeFrameType _timeFrame { get; set; } = TimeFrameType.Minute30;
        public int _lookback { get; set; } = 1250;
        public int _daysPeriod { get; set; } = (int)_timeFrame;// 5; FOR 5 TIMEFRAMETYPE -- 50 for 1Hour
        public DateTime _start { get; set; } = new DateTime(2023, 5, 31, 0, 0, 0);// new DateTime(2020, 4, 5, 0, 0, 0);// new DateTime(2020, 4, 5, 0, 0, 0);
        public DateTime _till { get; set; } = DateTime.MinValue;  //new DateTime(2022, 12, 25, 0, 0, 0);//
        public DateTime _to { get; set; }
        public bool DataFromApi = false;
        public bool SavingData = false;
        public List<Candle> CandleList { get; set; } = new List<Candle>();
        public DateTime LastRequestDateTime { get; set; } = DateTime.MinValue;
        public DateTime FirstRequestDateTime { get; set; } = DateTime.MinValue;

        public LastTwoBigCandlesBackground(TradeServices tradeServices,ApplicationDbContext context, IDbConnection dbConnection)
        {
            _tradeServices=tradeServices;
            _context = context;
            _dbConnection = dbConnection;
            if (SavingData)
                DataFromApi = true;

        }

        public async Task Invoke()
        {
            await Execute();
            return;
        }
        public (long, long) InitToFrom()
        {
            var to = SecondsFromDate(_to);
            to = to - 12600;// - 2700000;
            var from = to - ((60 * ((int)_timeFrame) * _lookback) - ((int)_timeFrame * 60));
            var testFrom = DateFromTicks(from);
            var testTo = DateFromTicks(to);
            return (from, to);
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
            //var to = SecondsFromDate(_to);
            //to = to - 12600;// - 2700000;
            //var from = to - (60 * ((int)TimeFrameType.Minute5) * _lookback) + ((int)TimeFrameType.Minute5 * 60);
            //var res = InitToFrom();
            long from = 0;
            long to = 0;

            //    var startFrom = from;
            //    var test = DateFromTicks(from);

            int result = SignalExtensions.GetTimeFrameType(_timeFrame);
            bool isInit = true;
            int skip = 0;
            _to = _start;

            while (start)
            {
                try
                {
                    //Console.WriteLine($"Mem usage: {GC.GetTotalMemory(true):#,0} bytes");
                    //Console.WriteLine($"Mem usage: {GC.GetTotalMemory(true):#,0} bytes");
                    var chart = new Chart();
                    int triedNumber = 1;
                    GC.Collect(0, GCCollectionMode.Forced);

                    var symbolCandle = new List<Candle>();
                    if ((symbolCandle is not null && symbolCandle.Count > 0 && symbolCandle.Last().DateTime == LastRequestDateTime) || _symbol == 8)
                        Thread.Sleep(30000);

                    if (CandleList is null || CandleList.Count == 0 || LastRequestDateTime >= CandleList.Last().DateTime || !CandleList.Any(p => p.Symbol == _symbol))
                    {
                        Console.Clear();
                        var res = InitToFrom();
                        from = res.Item1;
                        to = res.Item2;

                        DateTime dtFrom = DateFromTicks(from);
                        DateTime dtTo = DateFromTicks(to);

                        skip = 0;
                        if (!isInit)
                            ChangeSymbol(from, to);

                        //partition

                        //each times
                        if (_timeFrame >= TimeFrameType.Hour1)
                            LastRequestDateTime = LastRequestDateTime.AddMinutes(-(LastRequestDateTime.Minute));

                        if (CandleList?.LastOrDefault()?.DateTime == LastRequestDateTime)
                        {
                            _to = _to.AddDays(_daysPeriod).AddMinutes(-210);
                            res = InitToFrom();
                            from = res.Item1;
                            to = res.Item2;

                            dtFrom = DateFromTicks(from);
                            dtTo = DateFromTicks(to);

                            List.Clear();
                            CandleList = _context.Candles.Where(p => p.Symbol == _symbol && p.DateTime > dtFrom && p.DateTime <= dtTo.AddDays(_daysPeriod)).OrderBy(p => p.Ticks).ToList();
                            skip++;
                        }
                        else//first time
                        {
                            _to = _start;
                            res = InitToFrom();
                            from = res.Item1;
                            to = res.Item2;

                            dtFrom = DateFromTicks(from);
                            dtTo = DateFromTicks(to);

                            CandleList = _context.Candles.Where(p => p.Symbol == _symbol && p.DateTime >= dtFrom && p.DateTime <= dtTo.AddDays(_daysPeriod)).OrderBy(p => p.Ticks).ToList();

                        }

                        //end partition

                        isInit = false;
                    }
                    CandleList = SignalExtensions.GroupedByTimeFrame(CandleList, result);
                    var candleListLast = CandleList.Last();
                    skip++;

                    symbolCandle = CandleList.Skip(skip - 1).Take(_lookback).ToList();
                    var symbolCandleLast = symbolCandle.Last();
                    FirstRequestDateTime = symbolCandle.First().DateTime;
                    ChartFilling(chart, symbolCandle);

                    if (triedNumber > 2) // 2 times
                        continue;


                    List<Quote> quotesList = new List<Quote>();

                    quotesList.AddRange(CreateQuotes(chart));
                    quotesList = quotesList.DistinctBy(p => p.Date).ToList();

                    var rsi = quotesList.GetRsi(14).ToList();
                    var macd = quotesList.GetMacd(12, 26, 9).ToList();
                    var cci = quotesList.GetCci(10).ToList();

                    var sma21 = quotesList.GetSma(21).ToList();
                    var sma50 = quotesList.GetSma(50).ToList();
                    var sma100 = quotesList.GetSma(100).ToList();
                    var sma200 = quotesList.GetSma(200).ToList();

                    var ema21 = quotesList.GetEma(21).ToList();
                    var ema50 = quotesList.GetEma(50).ToList();
                    var ema100 = quotesList.GetEma(100).ToList();
                    var ema200 = quotesList.GetEma(200).ToList();




                    int quotesListCount = quotesList.Count;


                    decimal[] Mom = new decimal[chart.l.Count];
                    _ = TALib.Core.Mom(
                    chart.c.Select(p => p).ToArray(),
                   startIdx: 0,
                   endIdx: chart.h.Count - 1,
                    Mom,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   10
                   );
                    Mom = SignalExtensions.ShiftData(Mom, f, e);



                    //var ta = new UsingTaIndicators();
                    //var indicators = ta.Calculate(chart.o.Select(p => p).ToArray(),
                    //     chart.h.Select(p => p).ToArray(),
                    //     chart.l.Select(p => p).ToArray(),
                    //     chart.c.Select(p => p).ToArray(),
                    //     chart.v.Select(p => p).ToArray(),
                    //     chart.t.Select(p => DateFromTicks(p)).ToArray());





                    var listTemp = new List<CandleStichDivergengeSRSignalCheckingModel>();
                    for (int i = 1; i < quotesList.Count; i++)
                    {

                        if (i <= quotesListCount)
                            listTemp.Add(new CandleStichDivergengeSRSignalCheckingModel()
                            {
                                Open = quotesList[i].Open,
                                High = quotesList[i].High,
                                Low = quotesList[i].Low,
                                Close = quotesList[i].Close,
                                DateTime = quotesList[i].Date < new DateTime(2023, 1, 1) ? quotesList[i].Date.AddMinutes(210) : quotesList[i].Date.AddMinutes(210),
                                Volume = quotesList[i].Volume,
                                Macd = macd[i].Macd ?? 0,
                                MacdHist = macd[i].Histogram ?? 0,
                                Rsi = rsi[i].Rsi ?? 0,
                                Mom = (double)Mom[i],
                                Cci = cci[i].Cci ?? 0,
                                Stoch = 0,
                                Mfi = 0,
                                Cmf = 0,
                                Sma21 = sma21[i].Sma ?? 0,
                                Sma50 = sma50[i].Sma ?? 0,
                                Sma100 = sma100[i].Sma ?? 0,
                                Sma200 = sma200[i].Sma ?? 0,
                                Ema21 = ema21[i].Ema ?? 0,
                                Ema50 = ema50[i].Ema ?? 0,
                                Ema100 = ema100[i].Ema ?? 0,
                                Ema200 = ema200[i].Ema ?? 0,

                            });
                        //     Console.WriteLine(@$"dt:{DateFromTicks(chart.t[i - 1]).AddMinutes(210)} - open: {chart.o[i - 1]}  - high: {chart.h[i - 1]}  - low: {chart.l[i - 1]}  - close: {chart.c[i - 1]}  - sma14: {sma14.Sma} - sma200: {sma200.Sma}");

                    }

                    var checkSingnal = new CheckLastTwoCandlesSingnal(_context, _dbConnection);
                    List.AddRange(listTemp);
                    List = List.DistinctBy(p => p.DateTime).ToList();
                    var lastCandle = List.Last();
                    var lastCandleTehran = List.Last();

                    //List = List.TakeLast(289).ToList();
                    Console.WriteLine("now: " + DateTime.Now.ToString() + " || from of List: " + List.First().DateTime + " and to of List: " + List.Last().DateTime);
                    var resultType = checkSingnal.CheckStrategy(List, _symbol, _timeFrame);
                    LastRequestDateTime = List.Last().DateTime;
                    if (resultType == Enums.SignalCheckerType.Buy)
                    {

                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        _ = await _tradeServices.StartBuyProcessing(new Domain.Models.TradeIndexModel()
                        {
                            Symbol = (Symbol)_symbol,
                            SignalType = _signalType,
                            BuyTime = LastRequestDateTime.AddMinutes((int)_timeFrame),
                            TimeFrameType = _timeFrame
                        });

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
            DateTime till = DateTime.MinValue;
            if (_till > DateTime.MinValue)
                till = _till;
            else
            {
                var latestDatetime = _context.Candles.Where(p => p.Symbol == _symbol).Select(p => p.DateTime).OrderByDescending(p => p).First();
                till = latestDatetime;
            }

            if (LastRequestDateTime.AddMinutes((int)_timeFrame * 60 * 2) > till)
            {

                _symbol++;
                if (_symbol == 5)
                    _symbol++;

                _to = _start;
                var res = InitToFrom();
                var fromTicks = res.Item1;
                var toTicks = res.Item2;
                List.Clear();
                CandleList.Clear();
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
                    Date = DateFromTicks(chart.t[indexChartQuote]),
                    Volume = chart.v[indexChartQuote],
                });
                indexChartQuote++;
            }
            return quotes;
        }
    }
}
