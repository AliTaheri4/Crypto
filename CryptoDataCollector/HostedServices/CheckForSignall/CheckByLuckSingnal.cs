using Accord;
using Accord.Math.Optimization.Losses;
using CryptoDataCollector.BussinesExtensions;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Domain.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RepoDb;
using Skender.Stock.Indicators;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoDataCollector.CheckForSignall
{
    public class CheckByLuckSingnal
    {

        public List<CandleStichDivergengeSRSignalCheckingModel> AllList { get; set; }
        public CandleStichDivergengeSRSignalCheckingModel LastCandle { get; set; }
        public TradeType _TradeType { get; set; }
        public SignalType? _SignalType { get; set; } = SignalType.ByLuck;
        public ChartProcessType? _ProcessType { get; set; }
        public FutureSlopeType? _FutureSlopeType { get; set; }
        public PriceToMovingAverageType? _PriceToSmaType { get; set; }
        public bool _IsEmotional { get; set; }
        public ProfitLossModel? ProfitLess { get; set; }
        public GeneralStaus? _GeneralStatus { get; set; }
        public Trade LastTrade { get; set; }
        public Symbol _Symbol { get; set; }
        public TimeFrameType _TimeFrameType { get; set; }

        public ProfitLossModel? LastTradedProfitLess { get; set; } = new ProfitLossModel();

        public ApplicationDbContext _context { get; set; }

        public readonly IDbConnection _dbConnection;

        public decimal RR { get; set; }
        public int PivotPeriod { get; set; } = 10;
        public decimal SlopeVirtualLine { get; set; } = 0.001M;
        public int CountDecimal { get; set; }
        public decimal? ResisetanceSupportDiff { get; set; } = 0;
        public decimal? SlopePricePercent { get; set; }
        public int? DistanceBetweenPivots { get; set; }
        public int? LastCandleVolume { get; set; }
        public int? LastPivotVolume { get; set; }
        public decimal? AverageOf5LatestVolume { get; set; }
        public decimal? AverageOf10LatestVolume { get; set; }
        public int CandleTypeInt { get; set; }
        public string Description { get; set; } = "";
        public int CountOfLastestPP { get; set; } = 3;
        public decimal BigCandle { get; set; } = 2;
        public decimal StopLoss { get; set; } = 0.25M;


        public CheckByLuckSingnal(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
            LastTrade = _context.Trades.AsNoTracking().OrderByDescending(p => p.Id).FirstOrDefault();
        }


        public void SetParams(int step)
        {
            GC.Collect(2, GCCollectionMode.Forced);

            if (step == 1)
            {
                ////timeframe 5 min
                _Symbol = Symbol.BNB;
                CountDecimal = 1;
                _IsEmotional = false;
                RR = 2M;
                BigCandle = 1.1M; // 1hout 2M; 
            }
            else if (step == 2)
            {
                _Symbol = Symbol.ADA;
                CountDecimal = 4;
                _IsEmotional = false;
                RR = 2M;
                BigCandle = 1.1M;
            }
            else if (step == 3)
            {

                _Symbol = Symbol.ATOM;
                CountDecimal = 3;
                BigCandle = 1.1M;
                RR = 2M;
                _IsEmotional = false;
            }
            else if (step == 4)
            {
                _Symbol = Symbol.XRP;
                CountDecimal = 4;
                _IsEmotional = false;
                RR = 2M;
                BigCandle = 1.1M;

            }
            else if (step == 5)
            {
                _Symbol = Symbol.SOL;
                CountDecimal = 2;
                _IsEmotional = false;
                RR = 2M;
                BigCandle = 1.1M;
            }
            else if (step == 6)
            {

                _Symbol = Symbol.ETH;
                CountDecimal = 2;
                _IsEmotional = false;
                RR = 2M;
                BigCandle = 1.1M;

            }
            else if (step == 7)
            {
                _Symbol = Symbol.BTC;
                CountDecimal = 2;
                _IsEmotional = false;
                RR = 4M;
                BigCandle = 1.1M;
                StopLoss = 0.25M;
            }
            else
            {
                Thread.Sleep(30000);
            }



        }

        public SignalCheckerType CheckStrategy(List<CandleStichDivergengeSRSignalCheckingModel> list, int step, TimeFrameType timeFrame)
        {
            SetParams(step);
            //   return SignalCheckerType.HaveBefore;
            _Symbol = (Symbol)step;
            _TimeFrameType = timeFrame;

            var test = _context.Trades.All(t => t.Symbol == _Symbol);
            var test2 = _context.Trades.Any(t => t.Symbol == _Symbol);

            var lastTradePending = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType && p.TradeResultType == TradeResultType.Pending).AsNoTracking().OrderByDescending(p => p.Id).LastOrDefault();
            LastTradedProfitLess = new ProfitLossModel() { Loss = lastTradePending is null ? 0 : lastTradePending.Loss, Profit = lastTradePending is null ? 0 : lastTradePending.Profit };
            _GeneralStatus = LastTradedProfitLess.Loss > 0 ? GeneralStaus.Hold : GeneralStaus.Empty;
            list = list.OrderBy(p => p.DateTime).ToList();
            if (list.Count == 0)
            {
                Console.WriteLine("List Is Empty");
                Thread.Sleep(30 * 1000);
                return SignalCheckerType.ListEmpty;
            }
            AllList = list;
            LastCandle = list.Last();
            _TradeType = GetTradeType();

            _TradeType = LastTrade?.TradeResultType != null && LastTrade.TradeResultType == TradeResultType.Pending ? LastTrade.TradeType : GetTradeType();


            var tradePending = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType && p.TradeResultType == TradeResultType.Pending && p.BuyTime == DateTime.MinValue).FirstOrDefault();
            if (tradePending is not null)
            {
                tradePending.Buy = LastCandle.Open;
                tradePending.TradeResultType = TradeResultType.Pending;
                tradePending.BuyTime = LastCandle.DateTime;
                _SignalType = tradePending.SignalType;
                var pl = CalculateProfitLess(true);
                tradePending.Profit = pl.Profit;
                tradePending.Loss = pl.Loss;
                tradePending.SignalCandleClosePrice = LastCandle.Open;
                string seperator = string.IsNullOrEmpty(tradePending.Description) ? "" : " |||| ";
                tradePending.Description = tradePending.Description + @$"{seperator}Profit: {pl.Profit} and Loss: {pl.Loss}";
                _context.SaveChanges();
            }

            CheckForSell();


            _ProcessType = SignalExtensions.GetChartProcessType(GetBaseSignalCheckingList(list), 40);
            _PriceToSmaType = (double)LastCandle.Close > LastCandle.Cci ? PriceToMovingAverageType.Over : PriceToMovingAverageType.Under;
            _TradeType = GetTradeType();



            if (LastCandle.DateTime == new DateTime(2023, 3, 30, 19, 50, 0))
            {

            }

            var mode1 = CheckLuck();
            if (mode1.HasTriggred)
            {
                AverageOf5LatestVolume = AllList.TakeLast(5).Average(p => p.Volume);
                AverageOf10LatestVolume = AllList.TakeLast(10).Average(p => p.Volume);
                return Buy();
            }

            return SignalCheckerType.NotHappendInBuyChecker;
        }




        public SignalCheckerResult CheckLuck()
        {
            return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
        }



        public TradeType GetTradeType()
        {
            var random = new Random();
            var number = random.Next(1,11);

            if (number % 2 == 0)
                _TradeType = TradeType.Long;
            else if (number % 2 == 1)
                _TradeType = TradeType.Short;
            else _TradeType = TradeType.Unkouwn;

            return _TradeType;
        }

        public void CheckForSell()
        {
            if (_GeneralStatus != GeneralStaus.Hold)
                return;

            if (LastTradedProfitLess.Profit > 0 && LastTradedProfitLess.Loss > 0 && LastTradedProfitLess.Profit > LastTradedProfitLess.Loss)
            {
                if (LastCandle.High >= LastTradedProfitLess.Profit)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ ((($$$$ Profit $$$$)))  ------------------- ");
                    Console.WriteLine("\n");
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameProfit;
                    lastTrade.Sell = LastTradedProfitLess.Profit;
                    lastTrade.SellTime = LastCandle.DateTime;

                    _context.SaveChanges();

                    return;//selll
                }
                else if (LastCandle.Low <= LastTradedProfitLess.Loss)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ (((!!!! Loss !!!!)))  ------------------- ");
                    Console.WriteLine("\n");
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameLoss;
                    lastTrade.Sell = LastTradedProfitLess.Loss;
                    lastTrade.SellTime = LastCandle.DateTime;
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
                    _GeneralStatus = GeneralStaus.Empty;
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameProfit;
                    lastTrade.Sell = LastTradedProfitLess.Profit;
                    lastTrade.SellTime = LastCandle.DateTime;
                    _context.SaveChanges();

                    return;//selll

                }
                else if (LastCandle.High >= LastTradedProfitLess.Loss)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ (((!!!! Loss !!!!)))  ------------------- ");
                    Console.WriteLine("\n \n");
                    _GeneralStatus = GeneralStaus.Empty;
                    var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameLoss;
                    lastTrade.Sell = LastTradedProfitLess.Loss;
                    lastTrade.SellTime = LastCandle.DateTime;
                    _context.SaveChanges();

                    return;//selll

                }
            }
        }

        public SignalCheckerType Buy()
        {
            if (_GeneralStatus == GeneralStaus.Empty || _GeneralStatus == null || _GeneralStatus == GeneralStaus.Null)
            {
                var lastTrade = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType).OrderByDescending(p => p.Id).FirstOrDefault();

                if (lastTrade != null && lastTrade?.SellTime == null)
                    return SignalCheckerType.HaveBefore;


                var isTradeExist = _context.Trades.Where(p => p.Symbol == _Symbol && p.TimeFrameType == _TimeFrameType && p.BuyTime <= LastCandle.DateTime.AddMinutes((int)_TimeFrameType) && p.SellTime > LastCandle.DateTime.AddMinutes((int)_TimeFrameType)).Any();
                //if (lastTrade != null && lastTrade?.SellTime != null && lastTrade.SellTime > AllList[AllList.Count - 2].DateTime)
                if (lastTrade != null && lastTrade?.SellTime != null && isTradeExist)
                    return SignalCheckerType.HaveBefore;

                //        if (isTradeExist && lastTrade.SellTime.Value.AddMinutes(2 * (int)_TimeFrameType) > LastCandle.DateTime)
                //           return SignalCheckerType.SellRecently;


                var test = AllList.OrderBy(p => p.Volume).TakeLast(3).First().Volume;
                _context.Trades.Add(new Trade()
                {
                    Buy = 0,
                    Sell = 0,
                    IsEmotional = _IsEmotional,
                    Symbol = _Symbol,
                    TimeFrameType = _TimeFrameType,
                    SymbolName = _Symbol.GetEnumDescription(),
                    TradeResultType = TradeResultType.Pending,
                    TradeType = _TradeType,
                    SignalType = _SignalType.Value,
                    CountRedCandles = (int)LastCandle.Rsi,
                    CountGrayCandles = (int)LastCandle.Volume,
                    CountGreenCandles = (int)AllList.OrderBy(p => p.DateTime).TakeLast(2).First().Volume,
                    ThirdLastCandleVolume = AllList.OrderBy(p => p.DateTime).TakeLast(3).First().Volume,
                    ForthLastCandleVolume = AllList.OrderBy(p => p.DateTime).TakeLast(4).First().Volume,
                    DistancePercentFromSma = ResisetanceSupportDiff.Value,
                    Indicator1 = (decimal)LastCandle.Macd,
                    Indicator2 = AverageOf5LatestVolume.Value,
                    Indicator3 = AverageOf10LatestVolume.Value,
                    SignalCandleClosePrice = (decimal)LastCandle.Cci,
                    Profit = 0,
                    Loss = 0,
                    Description = Description,
                    Leverage = 0,
                    OpenLast = AllList.OrderBy(p => p.DateTime).TakeLast(2).First().Open,
                    HighLast = AllList.OrderBy(p => p.DateTime).TakeLast(2).First().High,
                    LowLast = AllList.OrderBy(p => p.DateTime).TakeLast(2).First().Low,
                    CloseLast = AllList.OrderBy(p => p.DateTime).TakeLast(2).First().Close,

                    OpenThird = AllList.OrderBy(p => p.DateTime).TakeLast(3).First().Open,
                    HighThird = AllList.OrderBy(p => p.DateTime).TakeLast(3).First().High,
                    LowThird = AllList.OrderBy(p => p.DateTime).TakeLast(3).First().Low,
                    CloseThird = AllList.OrderBy(p => p.DateTime).TakeLast(3).First().Close,

                    OpenForth = AllList.OrderBy(p => p.DateTime).TakeLast(4).First().Open,
                    HighForth = AllList.OrderBy(p => p.DateTime).TakeLast(4).First().High,
                    LowForth = AllList.OrderBy(p => p.DateTime).TakeLast(4).First().Low,
                    CloseForth = AllList.OrderBy(p => p.DateTime).TakeLast(4).First().Close,

                    OpenCurrent = LastCandle.Open,
                    HighCurrent = LastCandle.High,
                    LowCurrent = LastCandle.Low,
                    CloseCurrent = LastCandle.Close,

                    Sma21 = (decimal)LastCandle.Sma21,
                    Sma50 = (decimal)LastCandle.Sma50,
                    Sma100 = (decimal)LastCandle.Sma100,
                    Sma200 = (decimal)LastCandle.Sma200,
                    Ema21 = (decimal)LastCandle.Ema21,
                    Ema50 = (decimal)LastCandle.Ema50,
                    Ema100 = (decimal)LastCandle.Ema100,
                    Ema200 = (decimal)LastCandle.Ema200,
                });
                _context.SaveChanges();

                CalculateProfitLess(false);
                Console.WriteLine("==========>        pl.profit: " + ProfitLess.Profit.ToString() + " -----  pl.loss: " + ProfitLess.Loss.ToString());
                return SignalCheckerType.Buy;
            }
            return SignalCheckerType.HaveBefore;
        }


        public ProfitLossModel CalculateProfitLess(bool isBuyingCandle = true)
        {
            var pl = new ProfitLossModel();


            var diffVal = SignalExtensions.GetPercentOfValue(LastCandle.Open, StopLoss);
            if (_TradeType == TradeType.Long)
            {

                pl.Loss = LastCandle.Open - Decimal.Round(diffVal, CountDecimal, MidpointRounding.ToPositiveInfinity);//choose lesser
                pl.Profit = LastCandle.Open + Decimal.Round(diffVal * RR, CountDecimal, MidpointRounding.ToNegativeInfinity);//choose greater
            }
            if (_TradeType == TradeType.Short)
            {
                pl.Loss = LastCandle.Open + Decimal.Round(diffVal, CountDecimal, MidpointRounding.ToNegativeInfinity);
                pl.Profit = LastCandle.Open - Decimal.Round(diffVal * RR, CountDecimal, MidpointRounding.ToPositiveInfinity);
            }


            ProfitLess = pl;
            return pl;

        }

        public BaseSignalCheckingModel GetBaseSignalCheckingModel(CandleStichDivergengeSRSignalCheckingModel model)
        {
            return new BaseSignalCheckingModel()
            {
                Close = model.Close,
                High = model.High,
                Low = model.Low,
                Open = model.Open,
                DateTime = model.DateTime,
                Volume = model.Volume,
                Pivot = model.Cci,
                Changable = (double)(model.Open + model.Close) / 2
            };
        }
        public List<BaseSignalCheckingModel> GetBaseSignalCheckingList(List<CandleStichDivergengeSRSignalCheckingModel> list)
        {
            return list.Select(p => new BaseSignalCheckingModel()
            {
                Close = p.Close,
                High = p.High,
                Low = p.Low,
                Open = p.Open,
                DateTime = p.DateTime,
                Volume = p.Volume,
                Pivot = p.Cci,
                Changable = (double)(p.Open + p.Close) / 2
            }).ToList();
        }
    }


}
