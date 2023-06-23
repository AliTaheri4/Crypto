using CryptoDataCollector;
using CryptoDataCollector.BussinesExtensions;
using CryptoDataCollector.CheckForSignall;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using CryptoExchange.Net.CommonObjects;
using Domain.Data;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using MyProject.HostedServices.CheckForSignall;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using Symbol = CryptoDataCollector.Enums.Symbol;
using System.Data;
using CryptoDataCollector.Data;
using CryptoDataCollector.BussinesExtensions.Helper;
using Ninject.Activation;
using Microsoft.EntityFrameworkCore;
using Accord.IO;
using Accord;

namespace MyProject.Handlers
{
    public class StratgyRunnerCommand : MediatR.IRequest
    {

        public StratgyRunnerCommand()
        {
        }

    }

    public class StratgyRunnerHandler : AsyncRequestHandler<StratgyRunnerCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;
        public Dictionary<Symbol, List<RichCandleStickModel>> SymbolData = new Dictionary<Symbol, List<RichCandleStickModel>>();
        public TimeFrameType _maxTimeframe;
        public DateTime Now;
        public DateTime NowWithoutSeconds;
        public List<Candle> CandleList { get; set; } = new List<Candle>();

        public List<TradeIndexModel> TradeIndixes { get; set; }
        public StratgyRunnerHandler(IConfiguration configuration, ApplicationDbContext context, IDbConnection dbConnection)
        {
            _configuration = configuration;
            _context = context;
            _dbConnection = dbConnection;
            Now = DateTime.Now;
            NowWithoutSeconds = Now.RemoveSecondTicks();
        }
        protected override async Task Handle(StratgyRunnerCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var stratgiesDetails = _configuration.GetSection("Stratgies").Get<List<StratgiesDetailsModel>>().Where(p => p.IsActive).ToList();
            var timeframes = GetTimeFrames();
            if (timeframes == null || timeframes.Count == 0) return;
            _maxTimeframe = timeframes.Max();
            var validStratgiesDetails = new List<StratgiesDetailsModel>();

            var results = new List<TradeIndexModel>();
            foreach (var timeframe in timeframes)
            {
                validStratgiesDetails.Clear();
                validStratgiesDetails.AddRange(stratgiesDetails.Where(p => p.TimeFrameType == timeframe).ToList());

                foreach (var symbol in validStratgiesDetails.Select(p => p.Symbol).ToList())
                    await GetCandleStick(symbol);

                foreach (var item in validStratgiesDetails)
                {
                    switch (item.SignalType)
                    {
                        case SignalType.SmaCross:
                            break;
                        case SignalType.SmaDistance:
                            break;
                        case SignalType.SmaTouch:
                            break;
                        case SignalType.CciCrossLines:
                            break;
                        case SignalType.Fsp:
                            break;
                        case SignalType.DoubleEmaMacd:
                            break;
                        case SignalType.DivergenceCandleStickSr:
                            break;
                        case SignalType.LastTwoBigCandles:

                            await FillRichCandleStickModel(validStratgiesDetails.Select(p => p.Symbol).ToList(), timeframe);

                            var context = new BaseCheckSignal<RichCandleStickModel>();
                            context.SetStrategy(new CheckLastTwoCandlesSingnal<RichCandleStickModel>(_context, _dbConnection));
                            var res = context.CheckStrategy(SymbolData.Where(p => p.Key == item.Symbol).Select(p => p.Value).FirstOrDefault() ?? new List<RichCandleStickModel>(), (int)item.Symbol, timeframe);
                            if (res == SignalCheckerType.Buy)
                                results.Add(new TradeIndexModel()
                                {
                                    BuyTime = NowWithoutSeconds.AddMinutes((int)timeframe + 1),
                                    SignalType = SignalType.LastTwoBigCandles,
                                    Symbol = item.Symbol,
                                    TimeFrameType = timeframe
                                });

                            break;
                        case SignalType.ByLuck:
                            break;
                        default:
                            break;
                    }
                }

            }

        }

        private List<TimeFrameType> GetTimeFrames()
        {
            var now = Now;
            var newNow = now;
            if (now.Second >= 50 && now.Second < 59)
                newNow = now.AddMinutes(1);
            if (now.Minute < 0 /*|| now.Second<50*/) { return null; }

            var timeFrames = new List<TimeFrameType>();
            timeFrames.Add(TimeFrameType.Minute1);

            if (newNow.Minute % 5 == 0) timeFrames.Add(TimeFrameType.Minute5);
            if (newNow.Minute % 15 == 0) timeFrames.Add(TimeFrameType.Minute15);
            if (newNow.Minute % 30 == 0) timeFrames.Add(TimeFrameType.Minute30);
            if (newNow.Minute == 0) timeFrames.Add(TimeFrameType.Hour1);
            if (newNow.Minute == 0 && now.Hour % 2 == 0) timeFrames.Add(TimeFrameType.Hour2);
            if (newNow.Minute == 0 && now.Hour % 4 == 0) timeFrames.Add(TimeFrameType.Hour4);
            if (newNow.Minute == 0 && now.Hour % 24 == 0) timeFrames.Add(TimeFrameType.Day);

            return timeFrames;
        }

        private async Task GetCandleStick(Symbol symbol)
        {
            var lookback = _configuration.GetValue<int>("StratgyConfig:Lookback") + 10;
            if (SymbolData.Where(p => p.Key == symbol).Select(p => p.Value.Count()).FirstOrDefault() >= lookback - 10)
                return;

            var to = SecondsFromDate(DateTime.Now.AddMinutes(1));
            var from = to - (60 * (int)_maxTimeframe * lookback);
            DateTime dtFrom = DateFromTicks(from);
            CandleList = await _context.Candles.FromSqlRaw("SELECT * FROM dbo.IranizeTimeZoneCandle").Where(p => p.Symbol == (int)symbol && p.DateTime >= dtFrom).OrderBy(p => p.Ticks).ToListAsync();
            //   CandleList = _context.Candles.Where(p => p.Symbol == (int)symbol && p.DateTime >= dtFrom).OrderBy(p => p.Ticks).ToList();
            SymbolData.Remove(symbol);
            SymbolData.Add(symbol, CandleList.Select(p => new RichCandleStickModel()
            {
                DateTime = p.DateTime,
                Open = p.Open,
                High = p.High,
                Low = p.Low,
                Close = p.Close,
                Volume = p.Volume
            }).ToList());
        }

        public static long SecondsFromDate(DateTime date)
        {
            //date = date.Date;
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return (long)date.Subtract(baseDate).TotalSeconds;
        }
        public static DateTime DateFromTicks(long ticks)
        {
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return baseDate.AddMilliseconds(ticks * 1000);
        }
        private static void ChartFilling(Chart chart, List<Quote>? symbolCandle)
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
            chart.t.AddRange(symbolCandle.Select(p => SecondsFromDate(p.Date)).ToList().OrderBy(p => p).ToList());
        }

        private async Task FillRichCandleStickModel(List<Symbol> symbols, TimeFrameType timeFrame)
        {
            int result = SignalExtensions.GetTimeFrameType(timeFrame);

            var symbolDataClone = new Dictionary<Symbol, List<RichCandleStickModel>>();

            foreach (var symbolData in SymbolData)
            {
                CandleList = SignalExtensions.GroupedByTimeFrame(CandleList, (int)timeFrame);
                var candleListLast = CandleList.Last();

                List<Quote> quotesList = new List<Quote>();

                quotesList.AddRange(symbolData.Value.Select(p => new Quote() { Open = p.Open, High = p.High, Low = p.Low, Close = p.Close, Date = p.DateTime, Volume = p.Volume }));
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
                var chart = new Chart();
                ChartFilling(chart, quotesList);


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

                var ta = new UsingTaIndicators();
                var indicators = ta.Calculate(chart.o.Select(p => p).ToArray(),
                     chart.h.Select(p => p).ToArray(),
                     chart.l.Select(p => p).ToArray(),
                     chart.c.Select(p => p).ToArray(),
                     chart.v.Select(p => p).ToArray(),
                     chart.t.Select(p => DateFromTicks(p)).ToArray());


                var listTemp = new List<RichCandleStickModel>();
                for (int i = 0; i < quotesList.Count; i++)
                {
                    //     if (i <= quotesListCount)
                    listTemp.Add(new RichCandleStickModel()
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
                        Ema200IndicatorHelp = (double)indicators.Ema200[i]
                    });
                }
                symbolDataClone.Add(symbolData.Key, listTemp);

            }

            SymbolData.Clear();

            SymbolData = symbolDataClone;
        }
    }
}