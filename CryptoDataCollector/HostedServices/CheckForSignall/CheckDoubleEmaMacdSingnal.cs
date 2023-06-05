using Accord.Math.Optimization.Losses;
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
    public class CheckDoubleEmaMacdSingnal
    {

        public decimal RiskToReward { get; set; }
        public bool EnableRR { get; set; }
        public List<DoubleEmaMacdSignalCheckingModel> AllList { get; set; }
        public DoubleEmaMacdSignalCheckingModel LastCandle { get; set; }
        public TradeType _TradeType { get; set; }
        public SignalType? _SignalType { get; set; }
        public PriceToMovingAverageType? _PriceToSmaType { get; set; }
        public bool _IsEmotional { get; set; }
        public ProfitLossModel? ProfitLess { get; set; }
        public GeneralStaus? _GeneralStaus { get; set; }
        public List<Trade> LastTrades { get; set; }
        public Symbol _Symbol { get; set; }

        public ProfitLossModel? LastTradedProfitLess { get; set; } = new ProfitLossModel();

        public ApplicationDbContext _context { get; set; }

        public readonly IDbConnection _dbConnection;

        public decimal DistanceFromResistence { get; set; }
        public decimal DistancePercentFromSma { get; set; }
        public decimal MinLoss { get; set; }
        public decimal MaxLoss { get; set; }
        public decimal BigCandle { get; set; }

        public string Description { get; set; } = "";

        public CheckDoubleEmaMacdSingnal(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
            LastTrades = _context.Trades.AsNoTracking().Where(p => p.Symbol == _Symbol).OrderByDescending(p => p.Id).Take(3).ToList();


            RiskToReward =1M; //2M; // for example: 1.67=> reward=> 1 and risk=> 0.67  or 2 => reward 3 and risk 1.5
            EnableRR = false; // for example: 1.67=> reward=> 1 and risk=> 0.67  or 2 => reward 3 and risk 1.5
            _IsEmotional = false;



        }


        public void SetParams(int step)
        {

            if (step == 1)
            {
                _Symbol = Symbol.BNB;
                DistanceFromResistence = 0.3M;//percent
                MinLoss = 0.025M;//percent
                MaxLoss = 20M;
                BigCandle = 10.5M;//percent
            }
            else if (step == 2)
            {
                _Symbol = Symbol.ADA;

            }
            else if (step == 3)
            {

                _Symbol = Symbol.ATOM;

            }
           else if (step == 4)
            {
                _Symbol = Symbol.XRP;

            }
            else if (step == 5)
            {
                _Symbol = Symbol.SOL;

            }
            else if (step == 6)
            {

                _Symbol = Symbol.ETH;
                DistanceFromResistence = 0.2M;//percent
                MinLoss = 0.025M;//percent
                MaxLoss = 20M;
                BigCandle = 10.5M;//percent

            }
            else if (step == 7)
            {

                _Symbol = Symbol.BTC;
                DistanceFromResistence = 0.1M;//percent
                MinLoss = 0.25M;//percent
                MaxLoss = 2M;
                BigCandle = 0.5M;//percent
            }
            else
            {
                Thread.Sleep(30000);
            }
        }

        public SignalCheckerType CheckStrategy(List<DoubleEmaMacdSignalCheckingModel> list, int step)
        {

            SetParams(step);

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
            if (LastCandle.DateTime == new DateTime(2022, 12, 2, 22, 30, 0))
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


            list = list.OrderByDescending(p => p.DateTime).ToList();
            list = list.OrderBy(p => p.DateTime).ToList();




            var mode = CheckForGettingSignal();
            if (mode.HasTriggred)
            {
                _SignalType = SignalType.DoubleEmaMacd;

            }

            if (/*mode1.HasTriggred || mode2.HasTriggred ||*/ mode.HasTriggred)
            {

                return Buy();
            }

            return SignalCheckerType.NotHappendInBuyChecker;

        }
        public SignalCheckerResult CheckForGettingSignal()
        {
            bool cond1 = false, cond2 = false;
            var listBars = AllList.TakeLast(6).ToList();

            if (LastCandle.Ema20 > LastCandle.Ema200)
                _TradeType = TradeType.Long;
            else if (LastCandle.Ema20 < LastCandle.Ema200)
                _TradeType = TradeType.Short;
            else
                _TradeType = TradeType.Unkouwn;

            if (SignalExtensions.SelectTopOfBodyCandle(new BaseSignalCheckingModel()
            {
                Open = LastCandle.Open,
                High = LastCandle.High,
                Low = LastCandle.Low,
                Close = LastCandle.Close,
                DateTime = LastCandle.DateTime,
                Volume = LastCandle.Volume
            }) < (decimal)LastCandle.Ema20 ||
             SignalExtensions.SelectBottomOfBodyCandle(new BaseSignalCheckingModel()
             {
                 Open = LastCandle.Open,
                 High = LastCandle.High,
                 Low = LastCandle.Low,
                 Close = LastCandle.Close,
                 DateTime = LastCandle.DateTime,
                 Volume = LastCandle.Volume
             }) > (decimal)LastCandle.Ema20)
            {
                return new SignalCheckerResult() { HasTriggred = cond1 && cond2, TradeType = _TradeType };
            }

            //var diffMacdLines = (listBars[listBars.Count - 3].MacdLine - listBars[listBars.Count - 3].MacdSignalLine) - (LastCandle.MacdLine - LastCandle.MacdSignalLine);
            //if (Math.Abs(diffMacdLines) < 2.5)
            //    return new SignalCheckerResult() { HasTriggred = false, TradeType = TradeType.Unkouwn };

            //var diffTest = Math.Abs(listBars[listBars.Count - 2].Low - listBars[listBars.Count - 2].High);
            //var diffTest2 = Math.Abs(listBars[listBars.Count - 1].Low - listBars[listBars.Count - 1].High);
            //var condTest1 = SignalExtensions.GetPercent(diffTest, listBars[listBars.Count - 2].Close);
            //var condTest2 = SignalExtensions.GetPercent(diffTest2, listBars[listBars.Count - 1].Close);
            //if (condTest1 > BigCandle && condTest2 > BigCandle)
            //    return new SignalCheckerResult() { HasTriggred = false, TradeType = _TradeType };


            //if (SignalExtensions.GetPercentWithDiffrences(listBars[listBars.Count - 2].Close, Math.Abs(listBars[listBars.Count - 2].Low - listBars[listBars.Count - 2].High)) > BigCandle &&
            //SignalExtensions.GetPercentWithDiffrences(listBars[listBars.Count - 1].Close, Math.Abs(listBars[listBars.Count - 1].Low - listBars[listBars.Count - 1].High)) > BigCandle)
            //    return new SignalCheckerResult() { HasTriggred = false, TradeType = _TradeType };

            if (_TradeType == TradeType.Long && listBars[listBars.Count - 2].MacdLine < LastCandle.MacdLine)
            {
                //color of candle && dont touch ema200
                if (LastCandle.Low > (decimal)LastCandle.Ema200 && LastCandle.Open < LastCandle.Close)
                {
                    cond1 = true;
                }
                else return new SignalCheckerResult() { HasTriggred = cond1 && cond2, TradeType = _TradeType };

                if (listBars[listBars.Count - 2].MacdLine < listBars[listBars.Count - 2].MacdSignalLine && listBars[listBars.Count - 1].MacdLine > listBars[listBars.Count - 1].MacdSignalLine)
                {
                    cond2 = true;
                }
                else
                {
                    var distanceBetweenMacdTrend = listBars[listBars.Count - 1].MacdLine - listBars[listBars.Count - 2].MacdLine;
                    var distanceBetweenMacdSignalTrend = listBars[listBars.Count - 1].MacdSignalLine - listBars[listBars.Count - 2].MacdSignalLine;
                    if (LastCandle.MacdLine < LastCandle.MacdSignalLine && LastCandle.MacdLine + distanceBetweenMacdTrend > LastCandle.MacdSignalLine + distanceBetweenMacdSignalTrend)
                    {
                        cond2 = true;
                    }
                    else return new SignalCheckerResult() { HasTriggred = cond1 && cond2, TradeType = _TradeType };
                }
            }




            else if (_TradeType == TradeType.Short && listBars[listBars.Count - 2].MacdLine > LastCandle.MacdLine)
            {
                //color of candle && dont touch ema200
                if (LastCandle.High < (decimal)LastCandle.Ema200 && LastCandle.Close < LastCandle.Open)
                {
                    cond1 = true;
                }
                else return new SignalCheckerResult() { HasTriggred = cond1 && cond2, TradeType = _TradeType };

                if (listBars[listBars.Count - 2].MacdLine > listBars[listBars.Count - 2].MacdSignalLine && listBars[listBars.Count - 1].MacdLine < listBars[listBars.Count - 1].MacdSignalLine)
                {
                    cond2 = true;
                }
                else
                {
                    var distanceBetweenMacdTrend = listBars[listBars.Count - 1].MacdLine - listBars[listBars.Count - 2].MacdLine;
                    var distanceBetweenMacdSignalTrend = listBars[listBars.Count - 1].MacdSignalLine - listBars[listBars.Count - 2].MacdSignalLine;
                    if (LastCandle.MacdLine > LastCandle.MacdSignalLine && LastCandle.MacdLine + distanceBetweenMacdTrend < LastCandle.MacdSignalLine + distanceBetweenMacdSignalTrend)
                    {
                        cond2 = true;
                    }
                    else return new SignalCheckerResult() { HasTriggred = cond1 && cond2, TradeType = _TradeType };
                }
            }

            return new SignalCheckerResult() { HasTriggred = cond1 && cond2, TradeType = _TradeType };

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
                    lastTrade.DistancePercentFromSma = SignalExtensions.GetPercentWithDiffrences((decimal)LastCandle.Ema20, (decimal)LastCandle.Ema200);
                    lastTrade.Indicator1 = LastCandle.Volume;
                    lastTrade.Indicator2 = SignalExtensions.GetPercentWithDiffrences(AllList[AllList.Count - 2].Open, AllList[AllList.Count - 2].Close);
                    lastTrade.Indicator3 = SignalExtensions.GetPercentWithDiffrences(LastCandle.Open, LastCandle.Close);
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
                    lastTrade.DistancePercentFromSma = SignalExtensions.GetPercentWithDiffrences((decimal)LastCandle.Ema20, (decimal)LastCandle.Ema200);
                    lastTrade.Indicator1 = LastCandle.Volume;
                    lastTrade.Indicator2 = SignalExtensions.GetPercentWithDiffrences(AllList[AllList.Count - 2].Open, AllList[AllList.Count - 2].Close);
                    lastTrade.Indicator3 = SignalExtensions.GetPercentWithDiffrences(LastCandle.Open, LastCandle.Close);
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
                    lastTrade.DistancePercentFromSma = SignalExtensions.GetPercentWithDiffrences((decimal)LastCandle.Ema20, (decimal)LastCandle.Ema200);
                    lastTrade.Indicator1 = LastCandle.Volume;
                    lastTrade.Indicator2 = SignalExtensions.GetPercentWithDiffrences(AllList[AllList.Count - 2].Open, AllList[AllList.Count - 2].Close);
                    lastTrade.Indicator3 = SignalExtensions.GetPercentWithDiffrences(LastCandle.Open, LastCandle.Close);
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
                    lastTrade.DistancePercentFromSma = SignalExtensions.GetPercentWithDiffrences((decimal)LastCandle.Ema20, (decimal)LastCandle.Ema200);
                    lastTrade.Indicator1 = LastCandle.Volume;
                    lastTrade.Indicator2 = SignalExtensions.GetPercentWithDiffrences(AllList[AllList.Count - 2].Open, AllList[AllList.Count - 2].Close);
                    lastTrade.Indicator3 = SignalExtensions.GetPercentWithDiffrences(LastCandle.Open, LastCandle.Close);
                    lastSignal.GeneralStatus = _GeneralStaus.Value;
                    lastSignal.Profit = 0;
                    lastSignal.Loss = 0;
                    lastSignal.Symbol = _Symbol;
                    _context.SaveChanges();

                    return;//selll

                }
            }
        }
        public ProfitLossModel CalculateProfitLess()
        {
            ProfitLess = new ProfitLossModel();
            if (_TradeType == TradeType.Long)
            {
                var diffPrice = SignalExtensions.GetPercentOfValue(LastCandle.Low, DistanceFromResistence);
                var loss = SignalExtensions.GetPercentWithDiffrences(LastCandle.Close, LastCandle.Low - diffPrice);
                loss = loss < MinLoss ? MinLoss : loss;
                loss = loss > MaxLoss ? MaxLoss : loss;
                ProfitLess.Loss = LastCandle.Close - SignalExtensions.GetPercentOfValue(LastCandle.Close, loss);
                var diffPricePercentRR = SignalExtensions.GetPercentWithDiffrences(ProfitLess.Loss, LastCandle.Close) * RiskToReward;
                ProfitLess.Profit = LastCandle.Close + SignalExtensions.GetPercentOfValue(LastCandle.Close, diffPricePercentRR);
            }
            else if (_TradeType == TradeType.Short)
            {
                var diffPrice = SignalExtensions.GetPercentOfValue(LastCandle.High, DistanceFromResistence);
                var loss = SignalExtensions.GetPercentWithDiffrences(LastCandle.Close, LastCandle.High + diffPrice);
                loss = loss < MinLoss ? MinLoss : loss;
                loss = loss > MaxLoss ? MaxLoss : loss;

                ProfitLess.Loss = LastCandle.Close + SignalExtensions.GetPercentOfValue(LastCandle.Close, loss);

                var diffPricePercentRR = SignalExtensions.GetPercentWithDiffrences(ProfitLess.Loss, LastCandle.Close);
                ProfitLess.Profit = LastCandle.Close - SignalExtensions.GetPercentOfValue(LastCandle.Close, diffPricePercentRR) * 2;
            }
            return ProfitLess;
        }
        public void SetLimitationOnProfitLoss(bool enableRR)
        {
            bool isProfitGreaterThanPrice = ProfitLess.Profit > LastCandle.Close;
            var profitToPricePercent = SignalExtensions.GetPercentWithDiffrences(ProfitLess.Profit, LastCandle.Close);
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

            if (model.Profit > model.Loss)
                model.Loss = LastCandle.Close - (LastCandle.Close / 100) * (diffLost);
            else
                model.Loss = LastCandle.Close + (LastCandle.Close / 100) * (diffLost);


        }
        public SignalCheckerType Buy()
        {
            if (_GeneralStaus == GeneralStaus.Empty || _GeneralStaus == null || _GeneralStaus == GeneralStaus.Null)
            {
                if (_SignalType == SignalType.DoubleEmaMacd)
                {
                    CalculateProfitLess();
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
                        SignalType = SignalType.DoubleEmaMacd,
                        DistancePercentFromSma = DistancePercentFromSma,
                        Indicator1 = (decimal)LastCandle.Ema20,
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


    }



}
