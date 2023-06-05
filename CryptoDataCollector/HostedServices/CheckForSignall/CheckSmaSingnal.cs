using CryptoDataCollector.BussinesExtensions;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RepoDb;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace CryptoDataCollector.CheckForSignall
{
    public class CheckSmaSingnal
    {
        public int GapBetweenSignalls { get; set; }
        public decimal Changable { get; set; }
        public int DecimalCount { get; set; }
        public decimal BigCandle { get; set; }
        public decimal MinDistanceToEnablingDistanceSignalCheking { get; set; }
        public decimal MaxProfitPercent { get; set; }
        public decimal MinProfitPercent { get; set; }
        public decimal MinLossPercent { get; set; }
        public decimal DistanceFromSma { get; set; }
        public double DistanceAfterCross { get; set; }
        public decimal RiskToReward { get; set; }
        public bool EnableRR { get; set; }
        public decimal TouchKnockArea { get; set; }
        public decimal MinDistanceForTouch { get; set; }
        public int DistanceBetweenBigCandles { get; set; }
        public List<SmaSignalCheckingModel> AllList { get; set; }
        public SmaSignalCheckingModel LastCandle { get; set; }
        public TradeType _TradeType { get; set; }
        public SignalType? _SignalType { get; set; }
        public ChartProcessType? _ProcessType { get; set; }
        public FutureSlopeType? _FutureSlopeType { get; set; }
        public PriceToMovingAverageType? _PriceToSmaType { get; set; }
        public bool _IsEmotional { get; set; }
        public ProfitLossModel? ProfitLess { get; set; }
        public GeneralStaus? _GeneralStaus { get; set; }
        public List<Trade> LastTrades { get; set; }
        public Symbol _Symbol { get; set; }

        public ProfitLossModel? LastTradedProfitLess { get; set; } = new ProfitLossModel();

        public ApplicationDbContext _context { get; set; }

        public readonly IDbConnection _dbConnection;

        public decimal DistancePercentFromSma { get; set; }
        public decimal Between0_9And1_3 { get; set; }
        public decimal Between1_3And1_8 { get; set; }
        public decimal Between1_8And3 { get; set; }
        public decimal BetweenUp3 { get; set; }
        public int? NeedingInRangeCandles { get; set; }
        public int? CountGreenCandles { get; set; }
        public int? CountRedCandles { get; set; }
        public int? CountGrayCandles { get; set; }
        public DateTime changeSymbolChecking { get; set; }
        public string Description { get; set; } = "MaxProfitPercent = 1.6M ;               MinProfitPercent = 0;               MinLossPercent = 0;             MinDistanceToEnablingDistanceSignalCheking = 1.6M;";

        public CheckSmaSingnal(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
            LastTrades = _context.Trades.AsNoTracking().Where(p => p.Symbol == _Symbol).OrderByDescending(p => p.Id).Take(3).ToList();


            GapBetweenSignalls = 5;
            MaxProfitPercent = 1.6M;
            MinProfitPercent = 0.3M;
            RiskToReward = 2.5M; // for example: 1.67=> reward=> 1 and risk=> 0.67  or 2 => reward 3 and risk 1.5
            EnableRR = true; // for example: 1.67=> reward=> 1 and risk=> 0.67  or 2 => reward 3 and risk 1.5
            DistanceBetweenBigCandles = 12;
            _IsEmotional = false;


            Between0_9And1_3 = 8;
            Between1_3And1_8 = 9;
            Between1_8And3 = 4;
            BetweenUp3 = 10;




            //_Symbol = Symbol.Btc;
            //Changable = 0.01M; //1;// 0.1M;// 0.0001M; // 0.1M;
            //DecimalCount = 2;
            //BigCandle = 0.5M;
            //DistanceFromSma=

            //_Symbol = Symbol.Srp;
            //Changable = 0.0001M;
            //DecimalCount = 4;
            //BigCandle = 1M;
            //DistanceFromSma = 50;

            //_Symbol = Symbol.Sol;
            //Changable = 0.01M;
            //DecimalCount = 2;
            //BigCandle = 4.5M;


            DistanceAfterCross = (double)DistanceFromSma / 3 * 2; // for example: reward=> 1 and risk=> 0.67 
        }


        public void SetParams(int step)
        {

            if (step == 1)
            {
                _Symbol = Symbol.BNB;
                Changable = 0.01m;
                DecimalCount = 2; //count of changable digit places
                BigCandle = 1.20m; // percent from low to high
                DistanceFromSma = 50;
                MinDistanceForTouch = 0.6M;//0.8M
                TouchKnockArea = 0.06M;//0.04M
                MaxProfitPercent = 2.5M;// 1.6M;
                MinProfitPercent = 0.75M;//;0.5M //0.3M;
                MinLossPercent = 0.5M;// //0.35M;
                MinDistanceToEnablingDistanceSignalCheking = 0;// 1.6M;
            }
            else if (step == 2)
            {
                _Symbol = Symbol.ADA;
                Changable = 0.0001M; //1;// 0.1M;// 0.0001M; // 0.1M;
                DecimalCount = 4;
                BigCandle = 0.8M;
                DistanceFromSma = 130;//  become divided on 100 and then percent
                MinDistanceForTouch = 1M;
                TouchKnockArea = 0.06M;
                MaxProfitPercent = 1.6M;// 0.75M;
                MinProfitPercent = 0;// 0.35M; 
                MinLossPercent = 0;// 0.35M;
                MinDistanceToEnablingDistanceSignalCheking = 0M;
            }
            else if (step == 3)
            {

                _Symbol = Symbol.ATOM;
                Changable = 0.0001M;
                DecimalCount = 4;
                BigCandle = 1.5M;
                DistanceFromSma = 75;
                MinDistanceForTouch = 1M;
                TouchKnockArea = 0.1M;
                MaxProfitPercent = 1.6M;// 0.75M;
                MinProfitPercent = 0;// 0.35M; 
                MinLossPercent = 0;// 0.35M; //1.6M;
                MinDistanceToEnablingDistanceSignalCheking = 1.6M;
            }
            if (step == 4)
            {
                _Symbol = Symbol.XRP;
                Changable = 0.01m;
                DecimalCount = 2; //count of changable digit places
                BigCandle = 1.20m; // percent from low to high
                DistanceFromSma = 50;
                MinDistanceForTouch = 0.8M;
                TouchKnockArea = 0.04M;
                MaxProfitPercent = 1.6M;// 1.6M;// 0.8M; //; 0.8
                MinProfitPercent = 0.3M;//; //1.6M;
                MinLossPercent = 0.35M;// //1.6M;
                MinDistanceToEnablingDistanceSignalCheking = 0;// 1.6M;
            }
            else if (step == 5)
            {
                _Symbol = Symbol.SOL;
                Changable = 0.0001M; //1;// 0.1M;// 0.0001M; // 0.1M;
                DecimalCount = 4;
                BigCandle = 0.8M;
                DistanceFromSma = 130;//  become divided on 100 and then percent
                MinDistanceForTouch = 1M;
                TouchKnockArea = 0.06M;
                MaxProfitPercent = 1.6M;// 0.75M;
                MinProfitPercent = 0;// 0.35M; 
                MinLossPercent = 0;// 0.35M;
                MinDistanceToEnablingDistanceSignalCheking = 1.6M;
            }
            else if (step == 6)
            {

                _Symbol = Symbol.ETH;
                Changable = 0.0001M;
                DecimalCount = 4;
                BigCandle = 1.5M;
                DistanceFromSma = 75;
                MinDistanceForTouch = 1M;
                TouchKnockArea = 0.1M;
                MaxProfitPercent = 1.6M;// 0.75M;
                MinProfitPercent = 0;// 0.35M; 
                MinLossPercent = 0;// 0.35M; //1.6M;
                MinDistanceToEnablingDistanceSignalCheking = 1.6M;
            }
            else
            {

            }
        }

        public SignalCheckerType CheckStrategy(List<SmaSignalCheckingModel> list, int step)
        {

            SetParams(step);
            //       SaveCandles(list);
            //     return SignalCheckerType.HaveBefore;

            var lastSignal = _context.Signals.Where(p => p.Symbol == _Symbol).AsNoTracking().OrderByDescending(p => p.Id).LastOrDefault();
            LastTradedProfitLess = new ProfitLossModel() { Loss = lastSignal is null ? 0 : lastSignal.Loss, Profit = lastSignal is null ? 0 : lastSignal.Profit };
            _GeneralStaus = lastSignal?.GeneralStatus ?? GeneralStaus.Empty;

            if (list.Count == 0)
            {
                Console.WriteLine("List Is Empty");
                Thread.Sleep(30 * 1000);
                return SignalCheckerType.ListEmpty;
            }
            list = list.OrderBy(p => p.DateTime).ToList();
            AllList = list;
            LastCandle = list.Last();
            if (LastCandle.DateTime == new DateTime(2022, 12, 20, 0, 0, 0))
            {
            }
            var tradePending = _context.Trades.Where(p => p.Symbol == _Symbol && p.TradeResultType == TradeResultType.Pending && p.BuyTime == DateTime.MinValue).FirstOrDefault();
            if (tradePending is not null)
            {
                tradePending.Buy = LastCandle.Open;
                tradePending.TradeResultType = TradeResultType.Pending;
                tradePending.BuyTime = LastCandle.DateTime;
                _context.SaveChanges();
            }

            CheckForSell();


            _ProcessType = SignalExtensions.GetChartProcessType(GetBaseSignalCheckingList(list), 40);
            _PriceToSmaType = (double)LastCandle.Close > LastCandle.Sma200 ? PriceToMovingAverageType.Over : PriceToMovingAverageType.Under;
            list = list.OrderByDescending(p => p.DateTime).Take(GapBetweenSignalls + 1).ToList();
            list = list.OrderBy(p => p.DateTime).ToList();
            _TradeType = GetTradeType(list);



            //چک میکنه که یه وقت چندتا کندل بزرگ در حد فاصل کمی از هم قرار نداشته باشن در کندل های اخیر. چرا که اگر وجود داشته باشد، تغییر روند اتفاق افتاده است و غیر قابل پیشبینی خواهد بود
            var isExistSomeBigCandleInRecently = ExistSomeBigCandlesNearEachOther();
            if (isExistSomeBigCandleInRecently == true)
                return SignalCheckerType.NotHappendInBuyChecker;

            var mode1 = CheckForCrossSmas(list);
            if (mode1.HasTriggred)
            {
                _SignalType = SignalType.SmaCross;

                var isValid = CrossIsValid(mode1.TradeType);
                if (isValid == false)
                    return SignalCheckerType.NotHappendInBuyChecker;

                if (ProfitLess is null)
                    return SignalCheckerType.NotHappendInBuyChecker;

                //    Console.WriteLine("==========> pl.profit: " + ProfitLess.Profit.ToString() + " ----- pl.loss: " + ProfitLess.Loss.ToString());

            }

            var mode2 = CheckForDistances(list);
            if (mode2.HasTriggred)
            {
                _SignalType = SignalType.SmaDistance;

                //        var futureSlopeType = CheckDontExistBigCandleLowerInLongSignalCandle();
                //        if (futureSlopeType != FutureSlopeType.Ascending)
                //             _IsEmotional = true;
            }

            var mode3 = CheckForTouchKnock(list);
            if (mode3.HasTriggred)
            {
                _SignalType = SignalType.SmaTouch;

            }

            if (/*mode1.HasTriggred || mode2.HasTriggred ||*/ mode3.HasTriggred)
            {

                return Buy();
            }

            return SignalCheckerType.NotHappendInBuyChecker;

        }

        public TradeType GetTradeType(List<SmaSignalCheckingModel> list)
        {


            //CheckForCrossSmas
            var tradeType = list[5].Sma14 > list[5].Sma200 && list[4].Sma14 < list[4].Sma200 ? TradeType.Long :
                         list[5].Sma14 < list[5].Sma200 && list[4].Sma14 > list[4].Sma200 ? TradeType.Short
                         : TradeType.Unkouwn;

            if (tradeType != TradeType.Unkouwn)
                return tradeType;

            //CheckForDistances
            tradeType = list[5].Sma14 < list[5].Sma200 ? TradeType.Long :
                         list[5].Sma14 > list[5].Sma200 ? TradeType.Short
                         : TradeType.Unkouwn;


            return tradeType;
        }
        private void SaveCandles(List<SmaSignalCheckingModel> list)
        {
            var candles = new List<Candle>();
            foreach (var item in list)
            {
                candles.Add(new Candle()
                {
                    Open = item.Open,
                    High = item.High,
                    Low = item.Low,
                    Close = item.Close,
                    Symbol = (int)_Symbol,
                    SymbolName = _Symbol.GetEnumDescription(),
                    DateTime = item.DateTime,
                    Ticks = SecondsFromDate(item.DateTime),
                    Volume = (long)item.Volume,
                });
            }
                    //_context.Candles.AddRange(candles);
                    //_context.SaveChanges();


                    ((SqlConnection)_dbConnection).BulkMergeAsync("dbo.Candles", candles, x => new { x.Symbol, x.Ticks },
                         bulkCopyTimeout: 60 * 60,
                         batchSize: 5000
                        );
        }

        public void CheckForSell()
        {
            if (_GeneralStaus != GeneralStaus.Hold)
                return;

            if (LastTradedProfitLess.Profit > 0 && LastTradedProfitLess.Loss > 0 && LastTradedProfitLess.Profit > LastTradedProfitLess.Loss)
            {
                if (LastCandle.High >= LastTradedProfitLess.Profit)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ ((($$$$ Profit $$$$)))  ------------------- ");
                    Console.WriteLine("\n");
                    _GeneralStaus = GeneralStaus.Empty;
                    var lastSignal = _context.Signals.OrderByDescending(p => p.Id).LastOrDefault();
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameProfit;
                    lastTrade.Sell = lastSignal.Profit;
                    lastTrade.SellTime = LastCandle.DateTime;
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Profit = 0;
                    lastSignal.Loss = 0;
                    lastSignal.Symbol = _Symbol;


                    //_context.Signals.Remove(lastSignal);
                    _context.SaveChanges();

                    return;//selll
                }
                else if (LastCandle.Low <= LastTradedProfitLess.Loss)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ (((!!!! Loss !!!!)))  ------------------- ");
                    Console.WriteLine("\n");
                    _GeneralStaus = GeneralStaus.Empty;
                    var lastSignal = _context.Signals.OrderByDescending(p => p.Id).LastOrDefault();
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameLoss;
                    lastTrade.Sell = lastSignal.Loss;
                    lastTrade.SellTime = LastCandle.DateTime;
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Profit = 0;
                    lastSignal.Loss = 0;
                    lastSignal.Symbol = _Symbol;
                    _context.SaveChanges();

                    return;//selll
                }
            }
            else if (LastTradedProfitLess.Profit > 0 && LastTradedProfitLess.Loss > 0 && LastTradedProfitLess.Profit < LastTradedProfitLess.Loss)
            {
                if (LastCandle.Low <= LastTradedProfitLess.Profit)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ ((($$$$ Profit $$$$)))  ------------------- ");
                    Console.WriteLine("\n");
                    _GeneralStaus = GeneralStaus.Empty;
                    var lastSignal = _context.Signals.OrderByDescending(p => p.Id).LastOrDefault();
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameProfit;
                    lastTrade.Sell = lastSignal.Loss;
                    lastTrade.SellTime = LastCandle.DateTime;
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Profit = 0;
                    lastSignal.Loss = 0;
                    lastSignal.Symbol = _Symbol;
                    _context.SaveChanges();

                    return;//selll

                }
                else if (LastCandle.High >= LastTradedProfitLess.Loss)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ (((!!!! Loss !!!!)))  ------------------- ");
                    Console.WriteLine("\n \n");
                    _GeneralStaus = GeneralStaus.Empty;
                    var lastSignal = _context.Signals.OrderByDescending(p => p.Id).LastOrDefault();
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameLoss;
                    lastTrade.Sell = lastSignal.Loss;
                    lastTrade.SellTime = LastCandle.DateTime;
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Profit = 0;
                    lastSignal.Loss = 0;
                    lastSignal.Symbol = _Symbol;
                    _context.SaveChanges();

                    return;//selll

                }
            }
        }

        public SignalCheckerType Buy()
        {
            if (_GeneralStaus == GeneralStaus.Empty || _GeneralStaus == null || _GeneralStaus == GeneralStaus.Null)
            {
                if (_SignalType == SignalType.SmaDistance)
                {
                    CalculateProfitLess();
                    SetEmotionalOnProfitLoss();
                    SetLimitationOnProfitLoss(EnableRR);

                    _GeneralStaus = GeneralStaus.Hold;
                    var lastSignal = _context.Signals.OrderByDescending(p => p.Id).FirstOrDefault();

                    lastSignal.Profit = ProfitLess.Profit;
                    lastSignal.Loss = ProfitLess.Loss;
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Symbol = _Symbol;
                    _context.Trades.Add(new Trade()
                    {
                        Buy = 0,
                        Sell = 0,
                        IsEmotional = _IsEmotional,
                        Symbol = _Symbol,
                        SymbolName = _Symbol.GetEnumDescription(),
                        TradeResultType = TradeResultType.Pending,
                        TradeType = _TradeType,
                        SignalType = SignalType.SmaDistance,
                        CountRedCandles = CountRedCandles,
                        CountGrayCandles = CountGrayCandles,
                        CountGreenCandles = CountGreenCandles,
                        NeedingInRangeCandles = NeedingInRangeCandles,
                        DistancePercentFromSma = DistancePercentFromSma,
                        Indicator1 = (decimal)LastCandle.Ema100,
                        Indicator2 = (decimal)LastCandle.Ema200,
                        Indicator3 = LastCandle.Volume,
                        SignalCandleClosePrice = LastCandle.Close,
                        Profit = ProfitLess.Profit,
                        Loss = ProfitLess.Loss,
                        Description = Description,
                        Leverage = SignalExtensions.GetLeverage(DistancePercentFromSma)

                    });
                    _context.SaveChanges();

                    Console.WriteLine("==========>        pl.profit: " + ProfitLess.Profit.ToString() + " -----  pl.loss: " + ProfitLess.Loss.ToString());
                    return SignalCheckerType.Buy;
                }
                if (_SignalType == SignalType.SmaCross)
                {
                    CalculateProfitLess();
                    SetEmotionalOnProfitLoss();
                    SetLimitationOnProfitLoss(EnableRR);

                    _GeneralStaus = GeneralStaus.Hold;
                    var lastSignal = _context.Signals.OrderByDescending(p => p.Id).FirstOrDefault();

                    lastSignal.Profit = ProfitLess.Profit;
                    lastSignal.Loss = ProfitLess.Loss;
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Symbol = _Symbol;
                    _context.Trades.Add(new Trade()
                    {
                        Buy = 0,
                        Sell = 0,
                        IsEmotional = _IsEmotional,
                        Symbol = _Symbol,
                        SymbolName = _Symbol.GetEnumDescription(),
                        TradeResultType = TradeResultType.Pending,
                        TradeType = _TradeType,
                        SignalType = SignalType.SmaCross,
                        CountRedCandles = CountRedCandles,
                        CountGrayCandles = CountGrayCandles,
                        CountGreenCandles = CountGreenCandles,
                        DistancePercentFromSma = DistancePercentFromSma,
                        Indicator1 =(decimal) LastCandle.Ema100,
                        Indicator2 = (decimal)LastCandle.Ema200,
                        Indicator3 = LastCandle.Volume,
                        SignalCandleClosePrice = LastCandle.Close,
                        Profit = ProfitLess.Profit,
                        Loss = ProfitLess.Loss,
                        Description = Description,
                        Leverage = SignalExtensions.GetLeverage(DistancePercentFromSma)
                    });
                    _context.SaveChanges();
                    Console.WriteLine("==========>        pl.profit: " + ProfitLess.Profit.ToString() + " -----  pl.loss: " + ProfitLess.Loss.ToString());
                    return SignalCheckerType.Buy;
                }
                if (_SignalType == SignalType.SmaTouch)
                {

                    CalculateProfitLess();
                    SetEmotionalOnProfitLoss();
                    SetLimitationOnProfitLoss(EnableRR);

                    _GeneralStaus = GeneralStaus.Hold;
                    var lastSignal = _context.Signals.OrderByDescending(p => p.Id).FirstOrDefault();

                    lastSignal.Profit = ProfitLess.Profit;
                    lastSignal.Loss = ProfitLess.Loss;
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Symbol = _Symbol;
                    _context.Trades.Add(new Trade()
                    {
                        Buy = 0,
                        Sell = 0,
                        IsEmotional = _IsEmotional,
                        Symbol = _Symbol,
                        SymbolName = _Symbol.GetEnumDescription(),
                        TradeResultType = TradeResultType.Pending,
                        TradeType = _TradeType,
                        SignalType = SignalType.SmaTouch,
                        CountRedCandles = CountRedCandles,
                        CountGrayCandles = CountGrayCandles,
                        CountGreenCandles = CountGreenCandles,
                        DistancePercentFromSma = DistancePercentFromSma,
                        Indicator1 = (decimal)LastCandle.Ema100,
                        Indicator2 = (decimal)LastCandle.Ema200,
                        Indicator3 = LastCandle.Volume,
                        SignalCandleClosePrice = LastCandle.Close,
                        Profit = ProfitLess.Profit,
                        Loss = ProfitLess.Loss,
                        Description = Description,
                        Leverage = SignalExtensions.GetLeverage(DistancePercentFromSma)
                    });
                    _context.SaveChanges();
                    Console.WriteLine("==========>        pl.profit: " + ProfitLess.Profit.ToString() + " -----  pl.loss: " + ProfitLess.Loss.ToString());
                    return SignalCheckerType.Buy;

                }
            }


            return SignalCheckerType.HaveBefore;
        }
        public void SetForEmotinalIsHappenChecking()
        {
            _IsEmotional = SomeDistanceFrequentlyHappenedUnderSma();
            if (_IsEmotional == true)
                return;

            return;
        }
        public bool ExistSomeBigCandlesNearEachOther()
        {
            var lastBars = AllList.TakeLast(180).ToList();
            for (int i = 0; i < lastBars.Count - DistanceBetweenBigCandles; i++)
            {
                var percent = SignalExtensions.GetPercentWithDiffrences(lastBars[i].High, lastBars[i].Low);
                int count = 1;
                if (percent > BigCandle)
                    for (int j = 1; j < DistanceBetweenBigCandles; j++)
                    {
                        if (SignalExtensions.GetPercentWithDiffrences(lastBars[i + j].High, lastBars[i + j].Low) > BigCandle)
                            count++;

                        if (count >= 4)
                            return true;
                    }
            }
            return false;
        }

        public bool SomeDistanceFrequentlyHappenedUnderSma()
        {
            if (_SignalType != SignalType.SmaDistance)
                return false;

            var lastDistanceTrades = new List<int>();


            foreach (var item in LastTrades.OrderByDescending(p => p.Id).ToList())
            {
                if (item.SignalType == SignalType.SmaDistance && item.TradeResultType == TradeResultType.BecameProfit && item.TradeType == TradeType.Long)
                    lastDistanceTrades.Add(item.Id);
                else
                    break;
            }
            if (lastDistanceTrades.Count >= 3)
                return false;

            else if (lastDistanceTrades.Count == 0)
            {
                var hasCross = false;
                AllList.TakeLast(15).ToList().ForEach((i) => { if (i.Sma14 > i.Sma200 || (double)i.High > i.Sma200) hasCross = true; });
                return hasCross;
            }
            else
                return true;

        }


        public void SetEmotionalOnProfitLoss()
        {

            //bnb from 12/28 21:40
            //SetForEmotinalIsHappenChecking();
            if (NeedingInRangeCandles == 7)
                _IsEmotional = true;

            if (_IsEmotional)
            {
                var temp = ProfitLess.Profit;
                ProfitLess.Profit = ProfitLess.Loss;
                ProfitLess.Loss = temp;
            }
        }

        //public ChartProcessType GetChartProcessType(List<SmaSignalCheckingModel> list, int countLasts)
        //{
        //    var lastCandles = list.TakeLast(countLasts).ToList();
        //    if (lastCandles[0].Sma200 + ((lastCandles[0].Sma200 / 10000) * 5) < lastCandles[countLasts - 1].Sma200)
        //    {
        //        return ChartProcessType.Ascending;
        //    }
        //    else if (lastCandles[0].Sma200 - ((lastCandles[0].Sma200 / 10000) * 5) > lastCandles[countLasts - 1].Sma200)
        //    {
        //        return ChartProcessType.Descending;
        //    }
        //    else
        //        return ChartProcessType.Flat;
        //}


        //public decimal SelectBottomOfBodyCandle(SmaSignalCheckingModel model)
        //{

        //    if (model.Open < model.Close)
        //        return model.Open;

        //    return model.Close;
        //}
        //public decimal SelectTopOfBodyCandle(SmaSignalCheckingModel model)
        //{

        //    if (model.Open > model.Close)
        //        return model.Open;

        //    return model.Close;
        //}
        //public decimal GetValueOfPercent(decimal val, decimal percent)
        //{


        //    var value = val / 100 * percent;
        //    // var percent = ((great - less) / less) * 100;
        //    return value;
        //}

        //public decimal GetPercent(decimal great, decimal less)
        //{

        //    if (great < less)
        //    {
        //        var temp = great;
        //        great = less;
        //        less = temp;
        //    }
        //    var percent = less / great * 100;
        //    // var percent = ((great - less) / less) * 100;
        //    return percent;
        //}



        //public CandleType GetCandleType(SmaSignalCheckingModel candle)
        //{
        //    if (candle.Open > candle.Close)
        //        return CandleType.Red;
        //    else if (candle.Open < candle.Close)
        //        return CandleType.Green;
        //    else
        //        return CandleType.Gray;
        //}


        public bool CrossIsValid(TradeType tradeType)
        {
            var lastList = AllList.TakeLast(4).ToList();
            var lastCandle = AllList.Last();
            var greatSma = lastList.First().Sma14 > lastList.Last().Sma14 ? lastList.First().Sma14 : lastList.Last().Sma14;
            var lessSma = lastList.First().Sma14 > lastList.Last().Sma14 ? lastList.Last().Sma14 : lastList.First().Sma14;

            //distance between sma200s(must have good slope)
            if (((greatSma - lessSma) / lessSma) * 100 < 0.04)
                return false;

            var candleType = SignalExtensions.GetCandleType(AllList.Select(p => new BaseSignalCheckingModel()
            {
                Close = p.Close,
                High = p.High,
                Low = p.Low,
                Open = p.Open,
                DateTime = p.DateTime,
                Pivot = p.Sma14,
                Changable = p.Sma200
            }).Last());


            //color of signal candle 
            if (TradeType.Long == tradeType)
            {
                if (CandleType.Red == candleType)
                    return false;
            }
            else if (TradeType.Short == tradeType)
            {
                if (CandleType.Green == candleType /*|| CandleType.Gray == candleType*/)
                    return false;
            }



            //distance between signal candle and sma200
            if (TradeType.Long == tradeType)
            {
                if ((((double)lastCandle.Close - lastCandle.Sma200) / lastCandle.Sma200) * 100 > DistanceAfterCross)
                    return false;
            }
            else if (TradeType.Short == tradeType)
            {
                if (((lastCandle.Sma200 - (double)lastCandle.Open) / (double)lastCandle.Open) * 100 > DistanceAfterCross)
                    return false;
            }


            if (AllList.Last().Sma200 > (double)AllList.Last().Low && AllList.Last().Sma200 < (double)AllList.Last().High)
                return false;


            var lastBars = AllList.TakeLast(50).ToList();

            //dont have cross in before nearly
            for (int i = 6; i < lastBars.Count - 1; i++)
            {
                var res = CheckForCrossSmas(lastBars.Skip(i - 6).Take(6).ToList());
                if (res.HasTriggred && res.TradeType != TradeType.Unkouwn)
                    return false;
            }

            return true;

        }

        public ProfitLossModel CalculateProfitLess()
        {
            var pl = new ProfitLossModel();
            decimal min = 0, max = 0;
            //var lastBars = CheckValidHighLow(AllList).TakeLast(50).ToList();
            var lastBars = AllList.TakeLast(50).ToList();

            if (_SignalType == SignalType.SmaCross)
            {

                if (_TradeType == TradeType.Long)
                {
                    min = lastBars.Select(p => p.Low).Min();
                    var diff = SignalExtensions.GetPercentWithDiffrences(LastCandle.Close, min);

                    diff = diff < 0.3M ? 0.3M : diff > 1.1M ? 1.1M : diff;

                    pl.Profit = LastCandle.Close + diff;
                    pl.Loss = LastCandle.Close - diff;//((percent / 3) * 2);
                }
                else
                {
                    max = lastBars.Select(p => p.High).Max();
                    var diff = SignalExtensions.GetPercentWithDiffrences(max, LastCandle.Close);

                    diff = diff < 0.3M ? 0.3M : diff > 1.1M ? 1.1M : diff;

                    pl.Profit = LastCandle.Close - diff;
                    pl.Loss = LastCandle.Close + diff;//((percent / 3) * 2);

                }

                //if (isSlopeEmotional)
                //{
                //    var temp = pl.Profit;
                //    pl.Profit = pl.Loss;
                //    pl.Loss = temp;
                //}
            }
            else if (_SignalType == SignalType.SmaDistance)
            {
                double diffFromSma = 0;// sma / (100 * 5);   => 0.2%

                switch (_ProcessType)
                {
                    case ChartProcessType.Ascending:
                        diffFromSma = (LastCandle.Sma200 / 1000) * 2;// 0.4;
                        break;
                    case ChartProcessType.Descending:
                        diffFromSma = (LastCandle.Sma200 / 1000) * 1;
                        break;
                    case ChartProcessType.Flat:
                        diffFromSma = (LastCandle.Sma200 / 1000) * 1;// 0.2;
                        break;
                    default:
                        diffFromSma = (LastCandle.Sma200 / 1000) * 1;// 0.2;
                        break;
                }

                decimal diff = 0;
                if ((double)LastCandle.Close > LastCandle.Sma200)
                {
                    pl.Profit = (decimal)(LastCandle.Sma200 + diffFromSma);
                    diff = ToPositive(pl.Profit - LastCandle.Close);
                }
                else
                {
                    pl.Profit = (decimal)(LastCandle.Sma200 - diffFromSma);
                    diff = ToPositive(pl.Profit - LastCandle.Close);
                }
                //var diff = pl.Loss - LastCandle.Close;
                //if (diff < 0)
                //    diff = diff * -1;


                if ((double)LastCandle.Close > LastCandle.Sma200)
                {
                    pl.Loss = LastCandle.Close + diff;// ((diff / 2) * 3);
                }
                else
                {
                    pl.Loss = LastCandle.Close - diff;//((diff / 2) * 3);
                }

                //  }
            }
            else if (_SignalType == SignalType.SmaTouch)
            {
                if (_PriceToSmaType == PriceToMovingAverageType.Over)
                {
                    var percent = SignalExtensions.GetPercentWithDiffrences((decimal)LastCandle.Sma200, LastCandle.Low);
                    percent = percent > 0.125M ? 0.125M : percent;
                    pl.Loss = LastCandle.Low - SignalExtensions.GetPercentOfValue(LastCandle.Low, percent * 4);
                    pl.Profit = LastCandle.Low + SignalExtensions.GetPercentOfValue(LastCandle.Low, percent * 4);
                }
                if (_PriceToSmaType == PriceToMovingAverageType.Under)
                {
                    var percent = SignalExtensions.GetPercentWithDiffrences((decimal)LastCandle.Sma200, LastCandle.High);
                    percent = percent > 0.125M ? 0.125M : percent;
                    pl.Loss = LastCandle.High + SignalExtensions.GetPercentOfValue(LastCandle.High, percent * 4);
                    pl.Profit = LastCandle.High - SignalExtensions.GetPercentOfValue(LastCandle.High, percent * 4);
                }
            }
            ProfitLess = pl;
            return pl;

        }

        public void SetLimitationOnProfitLoss(bool enableRR)
        {
            bool isProfitGreaterThanPrice = ProfitLess.Profit > LastCandle.Close;
            var profitToPricePercent = SignalExtensions.GetPercentWithDiffrences(ProfitLess.Profit, LastCandle.Close);
            profitToPricePercent = profitToPricePercent > MaxProfitPercent ? MaxProfitPercent : profitToPricePercent;
            profitToPricePercent = profitToPricePercent < MinProfitPercent ? MinProfitPercent : profitToPricePercent;
            if (isProfitGreaterThanPrice)
            {
                ProfitLess.Profit = LastCandle.Close + (LastCandle.Close / 100) * profitToPricePercent;
                ProfitLess.Loss = LastCandle.Close - (LastCandle.Close / 100) * profitToPricePercent;
            }
            else
            {
                ProfitLess.Profit = LastCandle.Close - (LastCandle.Close / 100) * profitToPricePercent;
                ProfitLess.Loss = LastCandle.Close + (LastCandle.Close / 100) * profitToPricePercent;

            }
            if (enableRR)
                SetRiskToReward(ProfitLess);
        }

        public void SetRiskToReward(ProfitLossModel model)
        {
   

            var diff = SignalExtensions.GetPercentWithDiffrences(LastCandle.Close, model.Profit);
            decimal percent = RiskToReward * 100;
            percent = 100 / percent;
            var diffLost = diff * percent;
            diffLost = diffLost > MinLossPercent ? diffLost : MinLossPercent;

            if (model.Profit > model.Loss)
                model.Loss = LastCandle.Close - (LastCandle.Close / 100) * (diffLost);
            else
                model.Loss = LastCandle.Close + (LastCandle.Close / 100) * (diffLost);


        }

        public ProfitLossModel GetProfitLossBySmaFlatStocks(decimal price, double sma, ChartProcessType chartProcessType)
        {
            var pl = new ProfitLossModel();
            double diffFromSma = 0;// sma / (100 * 5);   => 0.2%

            switch (chartProcessType)
            {
                case ChartProcessType.Ascending:
                    diffFromSma = (sma / 1000) * 2;// 0.4;
                    break;
                case ChartProcessType.Descending:
                    diffFromSma = 0;//( sma / 1000) * 1 ;// 0.1;
                    break;
                case ChartProcessType.Flat:
                    diffFromSma = (sma / 1000) * 1;// 0.2;
                    break;
                default:
                    diffFromSma = (sma / 1000) * 1;// 0.2;
                    break;
            }



            if ((double)price > sma)
            {
                pl.Profit = (decimal)(sma + diffFromSma);

            }
            else
            {
                pl.Profit = (decimal)(sma - diffFromSma);

            }



            var diff = pl.Profit - price;
            if (diff < 0)
                diff = diff * -1;


            if ((double)price > sma)
            {
                pl.Loss = price + diff; //((diff / 3) * 2);
            }
            else
            {
                pl.Loss = price - diff; //((diff / 3) * 2);

            }

            return pl;
        }

        public ProfitLossModel GetProfitLossBySmaEmotionalStocks(decimal price, double sma, ChartProcessType chartProcessType)
        {
            var pl = new ProfitLossModel();
            double diffFromSma = 0;// sma / (100 * 5);   => 0.2%

            switch (chartProcessType)
            {
                case ChartProcessType.Ascending:
                    diffFromSma = (sma / 1000) * 2;// 0.4;
                    break;
                case ChartProcessType.Descending:
                    diffFromSma = 0;//( sma / 1000) * 1 ;// 0.1;
                    break;
                case ChartProcessType.Flat:
                    diffFromSma = (sma / 1000) * 1;// 0.2;
                    break;
                default:
                    diffFromSma = (sma / 1000) * 1;// 0.2;
                    break;
            }
            if ((double)price > sma)
            {
                pl.Loss = (decimal)(sma + diffFromSma);
            }
            else
            {
                pl.Loss = (decimal)(sma - diffFromSma);

            }

            var diff = pl.Loss - price;
            if (diff < 0)
                diff = diff * -1;


            if ((double)price > sma)
            {
                pl.Profit = price + diff;//((diff / 2) * 3);
            }
            else
            {
                pl.Profit = price - diff;//((diff / 2) * 3);

            }

            return pl;
        }

        public decimal ToPositive(decimal n)
        {
            if (n < 0)
                return n * (-1);
            return n;
        }
        public double ToPositive(double n)
        {
            if (n < 0)
                return n * (-1);
            return n;
        }



        public SignalCheckerResult CheckForTouchKnock(List<SmaSignalCheckingModel> list)
        {


            var lastBars = AllList.TakeLast(24).ToList();

            if (_PriceToSmaType == PriceToMovingAverageType.Over)
            {
                var highest = AllList.TakeLast(70).Max(p => p.High);

                //با یه شیب مناسبی به خط اس ام ای رسیده باشه. جلوگیری از اینگه درحال رنج زدن نباشه
                if (SignalExtensions.GetPercentWithDiffrences(highest, LastCandle.Close) < MinDistanceForTouch)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //آیا قبل از ورود به سیگنال، روند مناسب داشته است؟
                if (SignalExtensions.GetPercentWithDiffrences((decimal)lastBars[12].Ema200, lastBars[12].Low) < TouchKnockArea || (decimal)lastBars[10].Ema200 > lastBars[10].Low)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //آیا در دو کندل قبلی، سیگنال رخ داده است؟
                if (SignalExtensions.GetPercentWithDiffrences((decimal)list[3].Ema200, list[3].Low) > TouchKnockArea)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //آیا مجددا از خط اس ام ای فاصله گرفته است
                if (SignalExtensions.GetPercentWithDiffrences((decimal)list[4].Ema200, list[4].Low) < TouchKnockArea && SignalExtensions.GetPercentWithDiffrences((decimal)list[5].Ema200, list[5].Low) < TouchKnockArea)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //حداقل یه پله در جهت مناسب داشته باشه
                if (SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[5])) < SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[4])))
                    if (SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[4])) < SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[3])))
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //وسط کندل بالاتر از اس ام ای باشه
                if ((list[5].Open + list[5].Close) / 2 < (decimal)list[5].Ema200)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };


                //خودش کندل سیگنال نباشه
                if ((list[5].Low < (decimal)list[5].Ema200))
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };



                for (int i = 0; i < lastBars.Count - 8; i++)
                {
                    //خط اس ام ای نباید با بدنه کندل به صورت کامل قطع شود
                    if (SignalExtensions.SelectTopOfBodyCandle(GetBaseSignalCheckingModel(lastBars[i + 8])) < (decimal)lastBars[i + 8].Ema200)
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                    // اگر اخیرا با خط اس ام ای تماس داشته، این تماس جدید نامعتبر خواهد بود
                    if (SignalExtensions.GetPercentWithDiffrences((decimal)lastBars[i].Ema200, lastBars[i].Low) < TouchKnockArea)
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                    //وسط کندل بالاتر از اس ام ای باشه
                    if ((lastBars[i].Open + lastBars[i].Close) / 2 < (decimal)lastBars[i].Ema200)
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };


                }
            }
            if (_PriceToSmaType == PriceToMovingAverageType.Under)
            {
                var lowest = AllList.TakeLast(70).Min(p => p.Low);
                //با یه شیب مناسبی به خط اس ام ای رسیده باشه. جلوگیری از اینگه درحال رنج زدن نباشه
                if (SignalExtensions.GetPercentWithDiffrences(lowest, LastCandle.Close) < MinDistanceForTouch)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };


                //آیا قبل از ورود به سیگنال، روند مناسب داشته است؟
                if (SignalExtensions.GetPercentWithDiffrences((decimal)lastBars[12].Ema200, lastBars[12].High) < TouchKnockArea || (decimal)lastBars[10].Ema200 < lastBars[10].High)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //آیا در دو کندل قبلی، سیگنال رخ داده است؟
                if (SignalExtensions.GetPercentWithDiffrences((decimal)list[3].Ema200, list[3].High) > TouchKnockArea)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //آیا مجددا از خط اس ام ای فاصله گرفته است
                if (SignalExtensions.GetPercentWithDiffrences((decimal)list[4].Ema200, list[4].High) < TouchKnockArea && SignalExtensions.GetPercentWithDiffrences((decimal)list[5].Ema200, list[5].High) < TouchKnockArea)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //حدقال یه پله در جچهت مناسب داشته باشه
                if (SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[5])) > SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[4])))
                    if (SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[4])) > SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[3])))
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };


                //حداقل یه پله در جهت مناسب داشته باشه
                if (SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[5])) > SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[4])))
                    if (SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[4])) > SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(list[3])))
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //وسط کندل پایینتر از اس ام ای باشه
                if ((list[5].Open + list[5].Close) / 2 > (decimal)list[5].Ema200)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                //خودش کندل سیگنال نباشه
                if ((list[5].High > (decimal)list[5].Ema200))
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                for (int i = 0; i < lastBars.Count - 8; i++)
                {
                    //خط اس ام ای نباید با بدنه کندل به صورت کامل قطع شود
                    if (SignalExtensions.SelectBottomOfBodyCandle(GetBaseSignalCheckingModel(lastBars[i + 8])) > (decimal)lastBars[i + 8].Ema200)
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                    if (SignalExtensions.GetPercentWithDiffrences((decimal)lastBars[i].Ema200, lastBars[i].High) < TouchKnockArea)
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };


                    //وسط کندل بالاتر از اس ام ای باشه
                    if ((lastBars[i].Open + lastBars[i].Close) / 2 > (decimal)lastBars[i].Ema200)
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

                }
            }

            return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };

        }


        public SignalCheckerResult CheckForCrossSmas(List<SmaSignalCheckingModel> list)
        {
            if (list.Last().Sma200 == 0 || list.Last().Sma14 == 0)
                return new SignalCheckerResult() { TradeType = TradeType.Unkouwn, HasTriggred = false };

            var tradeType = list[5].Sma14 > list[5].Sma200 && list[4].Sma14 < list[4].Sma200 ? TradeType.Long :
                         list[5].Sma14 < list[5].Sma200 && list[4].Sma14 > list[4].Sma200 ? TradeType.Short
                         : TradeType.Unkouwn;

            if (tradeType == TradeType.Unkouwn)
                return new SignalCheckerResult() { TradeType = TradeType.Unkouwn, HasTriggred = false };

            foreach (var item in list)
            {
                if (tradeType == TradeType.Long)
                {
                    if (item.Sma14 < item.Sma200 || item.DateTime == list.Last().DateTime)
                        continue;
                    else
                        return new SignalCheckerResult() { TradeType = TradeType.Long, HasTriggred = false };

                }
                else //if (tradeType == TradeTypeType.Short)
                {
                    if (item.Sma14 > item.Sma200 || item.DateTime == list.Last().DateTime)
                        continue;
                    else
                        return new SignalCheckerResult() { TradeType = TradeType.Short, HasTriggred = false };

                }
            }
            return new SignalCheckerResult() { TradeType = tradeType, HasTriggred = true };

        }





        public SignalCheckerResult CheckForDistances(List<SmaSignalCheckingModel> list)
        {

            if (list.Last().Sma200 == 0)
                return new SignalCheckerResult() { TradeType = TradeType.Unkouwn, HasTriggred = false };


            //var tradeType = list[5].Sma14 < list[5].Sma200 ? TradeType.Long :
            //             list[5].Sma14 > list[5].Sma200 ? TradeType.Short
            //             : TradeType.Unkouwn;

            if (_TradeType == TradeType.Unkouwn)
                return new SignalCheckerResult() { TradeType = TradeType.Unkouwn, HasTriggred = false };


            var check6 = CheckDontOnOverSupportLine(list);

            //CheckValidHighLow(list);
            var countOfNeedingCandles = GetCandlesInRangeBySma200IsHappened();
            NeedingInRangeCandles = countOfNeedingCandles;
            var check1 = CheckExistGreenRedCandles(countOfNeedingCandles);
            var check2 = CheckPricesHaveSameRecords(countOfNeedingCandles);
            var check3 = true;// CheckDontExistCandleUnderSignalCandle();
            var check4 = !IsBigCandle(LastCandle);

            if (check1 == false || check2 == false || check3 == false || check4 == false || check6 == false)
                return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

            return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
        }


        public int GetCandlesInRangeBySma200IsHappened()
        {

            var diff = SignalExtensions.GetPercentWithDiffrences((decimal)LastCandle.Ema100, (LastCandle.Close + LastCandle.Open) / 2);
            DistancePercentFromSma = diff;
            if (diff < MinDistanceToEnablingDistanceSignalCheking)
                return 100;

            if (diff > 0.9M && diff < 1.3M)
                return (int)Between0_9And1_3;
            if (diff >= 1.3M && diff < 1.8M)
                return (int)Between0_9And1_3;
            if (diff >= 1.8M && diff < 3M)
                return (int)Between1_3And1_8;
            if (diff >= 3M)
                return (int)BetweenUp3;//7

            return 100;

        }

        public bool CheckExistGreenRedCandles(int needingCandles)
        {

            var candelsIsGreen = new List<bool>();
            var lastList = AllList.TakeLast(needingCandles).ToList();
            foreach (var item in lastList)
            {
                if (item.Open > item.Close)
                {
                    CountRedCandles++;
                    candelsIsGreen.Add(false);
                }
                else if (item.Open == item.Close)
                {
                    CountGreenCandles++;
                    candelsIsGreen.Add(true);
                }
                else if (item.Open < item.Close)
                {
                    CountGreenCandles++;
                    candelsIsGreen.Add(true);

                }
            }

            if (candelsIsGreen.Count(p => p == true) >= 3 && candelsIsGreen.Count(p => p == false) >= 3)
                return true;
            else
                return false;

        }


        //چک برای اینکه اگر لحظه خرید روی قله نیستیم، حداقل روی مقاومت نباشیم
        public bool CheckDontOnOverSupportLine(List<SmaSignalCheckingModel> list)
        {
            var isValid = false;
            var greenCandles = new List<bool>();
            foreach (var item in list)
                greenCandles.Add(SignalExtensions.GetCandleType(GetBaseSignalCheckingModel(item)) == CandleType.Red ? false : true);

            if (_TradeType == TradeType.Long)
            {
                if (greenCandles[0] && greenCandles[1] && !greenCandles[2] && !greenCandles[3] && !greenCandles[4] && !greenCandles[5])
                    isValid = false;
                else if (greenCandles[0] && greenCandles[1] && greenCandles[2] && !greenCandles[3] && !greenCandles[4] && !greenCandles[5])
                    isValid = false;
                else if (greenCandles[0] && greenCandles[1] && greenCandles[2] && greenCandles[3] && !greenCandles[4] && !greenCandles[5])
                    isValid = false;
                else
                    isValid = true;
            }
            else
            {
                if (!greenCandles[0] && !greenCandles[1] && greenCandles[2] && greenCandles[3] && greenCandles[4] && greenCandles[5])
                    isValid = false;
                else if (!greenCandles[0] && !greenCandles[1] && !greenCandles[2] && greenCandles[3] && greenCandles[4] && greenCandles[5])
                    isValid = false;
                else if (!greenCandles[0] && !greenCandles[1] && !greenCandles[2] && !greenCandles[3] && greenCandles[4] && greenCandles[5])
                    isValid = false;
                else
                    isValid = true;

            }
            return isValid;
        }

        public bool CheckPricesHaveSameRecords(int needingCandles)
        {
            var lastItems = AllList.TakeLast(needingCandles).ToList();
            var Listprices = new List<List<decimal>>();
            var samePrices = new List<decimal>();

            for (int i = 0; i < lastItems.Count; i++)
            {
                Listprices.Add(ExtractValuesInRangePrices(lastItems[i].Low, lastItems[i].High));
            }

            samePrices.AddRange(Listprices[0].Intersect(Listprices[1]));
            for (int i = 2; i < lastItems.Count; i++)
            {

                samePrices = samePrices.Intersect(Listprices[i]).ToList();
                if (samePrices.Count > 0)
                    continue;
                else
                    return false;

            }
            return true;
        }



        public FutureSlopeType CheckDontExistBigCandleLowerInLongSignalCandle()
        {
            decimal bigBefore = 0;
            decimal bigNear = 0;
            decimal price = 0;
            var listNear = AllList.TakeLast(12).ToList();
            var listBefore = AllList.TakeLast(40).Take(40 - 12).ToList();
            foreach (var item in listNear)
            {
                if (IsBigCandle(item) && item.Close < item.Open)
                    bigNear = bigNear < (item.High - item.Low) ? (item.High - item.Low) : bigNear;
            }
            foreach (var item in listBefore)
            {
                if (IsBigCandle(item) && item.Close < item.Open)
                    bigBefore = bigBefore < (item.High - item.Low) ? (item.High - item.Low) : bigBefore;
            }
            if (_TradeType == TradeType.Long)
            {
                if (bigNear < 0)
                    bigNear = bigNear * (-1);

                if (bigBefore < 0)
                    bigBefore = bigBefore * (-1);

                if (bigNear > bigBefore)
                    return FutureSlopeType.Descending;

                else
                    return FutureSlopeType.Ascending;

            }
            return FutureSlopeType.Ascending;
        }
        public bool IsBigCandle(SmaSignalCheckingModel item)
        {
            //        if ((GetPercent(LastCandle.Close, (item.High - item.Low)) > BigCandle) && item.Close < item.Open)
            //            return true;

            if ((SignalExtensions.GetPercent(LastCandle.Close, (item.High - item.Low)) > BigCandle))
                return true;
            return false;
        }
        //public bool CheckDontExistCandleUnderSignalCandle()
        //{
        //    var lastBars = AllList.TakeLast(10).ToList();
        //    var highOfMinCandle = lastBars.Min(p => p.High);
        //    if (LastCandle.Low > highOfMinCandle)
        //        return false;

        //    return true;
        //}

        public List<decimal> ExtractValuesInRangePrices(decimal low, decimal high)
        {
            var list = new List<decimal>();

            while (low <= high)
            {
                if (DecimalCount == 0)
                    low = (int)low;
                else if (DecimalCount == 1)
                    low = decimal.Parse(string.Format("{0:F1}", low));
                else if (DecimalCount == 2)
                    low = decimal.Parse(string.Format("{0:F2}", low));
                else if (DecimalCount == 3)
                    low = decimal.Parse(string.Format("{0:F3}", low));
                else if (DecimalCount == 4)
                    low = decimal.Parse(string.Format("{0:F4}", low));


                list.Add(low);
                low = low + Changable;
            }

            return list;
        }

        public List<SmaSignalCheckingModel> CheckValidHighLow(List<SmaSignalCheckingModel> list)
        {
            foreach (var item in list)
            {
                decimal up = item.Close > item.Open ? item.Close : item.Open;
                decimal dowm = item.Close > item.Open ? item.Open : item.Close;

                decimal diff = up / (100 * 5); // 0.02 %
                if (up + diff < item.High)
                {
                    item.High = up + diff;
                }

                if (dowm - diff > item.Low)
                {
                    item.Low = dowm - diff;
                }
            }
            return list;
        }


        public static long SecondsFromDate(DateTime date)
        {
            //date = date.Date;
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return (long)date.Subtract(baseDate).TotalSeconds;
        }
        public BaseSignalCheckingModel GetBaseSignalCheckingModel(SmaSignalCheckingModel model)
        {
            return new BaseSignalCheckingModel()
            {
                Close = model.Close,
                High = model.High,
                Low = model.Low,
                Open = model.Open,
                DateTime = model.DateTime,
                Volume = model.Volume,
                Pivot = model.Sma200,
                Changable = (double)(model.Open + model.Close) / 2
            };
        }
        public List<BaseSignalCheckingModel> GetBaseSignalCheckingList(List<SmaSignalCheckingModel> list)
        {
            return list.Select(p => new BaseSignalCheckingModel()
            {
                Close = p.Close,
                High = p.High,
                Low = p.Low,
                Open = p.Open,
                DateTime = p.DateTime,
                Volume = p.Volume,
                Pivot = p.Sma200,
                Changable = (double)(p.Open + p.Close) / 2
            }).ToList();
        }
    }



}
