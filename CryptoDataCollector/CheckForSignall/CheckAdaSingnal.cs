using CryptoDataCollector.BussinesExtensions;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector
{
    public class CheckAdaSingnal
    {
        public int GapBetweenSignalls { get; set; }
        public double DiffereceBetweenFishers { get; set; }
        public double MinCrossFishers { get; set; }
        public double DiffereceBetweenStoches { get; set; }
        public double MinCrossStoches { get; set; }
        public double AboveLineFisher { get; set; }
        public double DownLineFisher { get; set; }
        public double AboveLineStoch { get; set; }
        public double DownLineStoch { get; set; }

        public double MinFisher { get; set; }
        public double MaxFisher { get; set; }
        public double MinStoch { get; set; }
        public double MaxStoch { get; set; }
        public int GapBetweenPSarSignal { get; set; }
        public TradeType _TradeType { get; set; }
        public ApplicationDbContext _context { get; set; }
        public GeneralStaus? _GeneralStaus { get; set; }
        public ProfitLossModel? LastTradedProfitLess { get; set; } = new ProfitLossModel();
        public List<AdaSignalCheckingModel> AllList { get; set; }
        public AdaSignalCheckingModel LastCandle { get; set; }
        public Trade LastTrade { get; set; }

        public CheckAdaSingnal(ApplicationDbContext context)
        {
            _context = context;
            GapBetweenSignalls = 5;
            AboveLineFisher = 2.5; //2.45
            DownLineFisher = -2.5; // -2.45
            AboveLineStoch = 80; //82
            DownLineStoch = 20;//18   - 19* -
            MinFisher = -0.7; //-1.5
            MaxFisher = 0.7;// 1.5
            MinStoch = 50;//39
            MaxStoch = 50;
            GapBetweenPSarSignal = 5;
            DiffereceBetweenFishers = 0.1;
            DiffereceBetweenStoches = 1;
            MinCrossFishers = 0;// 0.13;
            MinCrossStoches = 0;// 1;
        }
       
        public bool CheckAdaStrategy(List<AdaSignalCheckingModel> list)
        {
            var lastSignal = _context.Signals.Where(p => p.Symbol == (Symbol)2).AsNoTracking().OrderByDescending(p => p.Id).LastOrDefault();
            LastTrade = _context.Trades.Where(p => p.Symbol == (Symbol)2).OrderByDescending(p => p.Id).FirstOrDefault();
            LastTradedProfitLess = new ProfitLossModel() { Loss = LastTrade is null ? 0 : LastTrade.Loss, Profit = LastTrade is null ? 0 : LastTrade.Profit };

            _GeneralStaus =LastTrade?.Sell==0 ? GeneralStaus.Hold:GeneralStaus.Empty;
            list = list.OrderBy(p => p.DateTime).ToList();
            AllList = list;
            LastCandle = list.Last();

            CheckForSell();
            var tradePending = _context.Trades.Where(p => p.Symbol == (Symbol)2 && p.TradeResultType == TradeResultType.Pending && p.BuyTime == DateTime.MinValue).FirstOrDefault();
            if (tradePending is not null)
            {
                tradePending.Buy = list.Last().Open;
                tradePending.TradeResultType = TradeResultType.Pending;
                tradePending.BuyTime = list.Last().DateTime;
                _context.SaveChanges();
            }
            var allList = list.OrderBy(p => p.DateTime).ToList();
            list = list.OrderByDescending(p => p.DateTime).Take(GapBetweenSignalls + 1).ToList();
            list = list.OrderBy(p => p.DateTime).ToList();

            var results = new List<SignalCheckerResult>();
            results.Add(FisherSignallTriggring(list));
            results.Add(StochSignallTriggring(list));
            results.Add(PSarSignallTriggring(list));
            _TradeType = results[0].TradeType;
            if (results.Count(p => p.HasTriggred == true) == 3 && (results.Count(p => p.TradeType == TradeType.Long) == 3 || results.Count(p => p.TradeType == TradeType.Short) == 3))
            {
                var pl= CalculateProfitLess(allList);
                _context.Trades.Add(new Trade()
                {
                    Buy = 0,
                    Sell = 0,
                    IsEmotional = false,
                    Symbol = (Symbol)2,
                    SymbolName = ((Symbol)2).GetEnumDescription(),
                    TradeResultType = TradeResultType.Pending,
                    TradeType = _TradeType,
                    SignalType = SignalType.SmaDistance,
                    CountRedCandles = 0,
                    CountGrayCandles = 0,
                    CountGreenCandles = 0,
                    NeedingInRangeCandles = 0,
                    DistancePercentFromSma = 0,
                    Indicator1 = 0,
                    SignalCandleClosePrice = list.Last().Close,
                    Profit = pl.Profit,
                    Loss = pl.Loss,
                    Description = string.Empty,
                    Leverage = 10

                });
                _context.SaveChanges();
                return true;
            }
            return false;
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
                    var lastTrade = _context.Trades.Where(p => p.Symbol == (Symbol)2).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameProfit;
                    lastTrade.Sell = lastTrade.Profit;
                    lastTrade.SellTime = LastCandle.DateTime;


                    //_context.Signals.Remove(lastSignal);
                    _context.SaveChanges();

                    return;//selll
                }
                else if (LastCandle.Low <= LastTradedProfitLess.Loss)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ (((!!!! Loss !!!!)))  ------------------- ");
                    Console.WriteLine("\n");
                    _GeneralStaus = GeneralStaus.Empty;
                    var lastTrade = _context.Trades.Where(p => p.Symbol == (Symbol)2).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameLoss;
                    lastTrade.Sell = lastTrade.Loss;
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
                    _GeneralStaus = GeneralStaus.Empty;
                    var lastTrade = _context.Trades.Where(p => p.Symbol == (Symbol)2).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameProfit;
                    lastTrade.Sell = lastTrade.Profit;
                    lastTrade.SellTime = LastCandle.DateTime;

                    _context.SaveChanges();

                    return;//selll

                }
                else if (LastCandle.High >= LastTradedProfitLess.Loss)
                {
                    Console.WriteLine(@$"-------------------  Sell at: {LastCandle.DateTime.ToString()} +++ (((!!!! Loss !!!!)))  ------------------- ");
                    Console.WriteLine("\n \n");
                    _GeneralStaus = GeneralStaus.Empty;
                    var lastTrade = _context.Trades.Where(p => p.Symbol == (Symbol)2).OrderByDescending(p => p.Id).FirstOrDefault();
                    lastTrade.TradeResultType = TradeResultType.BecameLoss;
                    lastTrade.Sell = lastTrade.Loss;
                    lastTrade.SellTime = LastCandle.DateTime;

                    _context.SaveChanges();

                    return;//selll

                }
            }
        }

        public ProfitLossModel CalculateProfitLess(List<AdaSignalCheckingModel> list)
        {
            var lastBars = list.TakeLast(100).ToList();
            int supportResistenceCandleDistance = 3;
            var srList = new List<decimal>();
            var lastBar = lastBars.Last();
            int count = 0;
            for (int i = supportResistenceCandleDistance; i < lastBars.Count - supportResistenceCandleDistance; i++)
            {
                count = 0;
                for (int j = i - supportResistenceCandleDistance; j < i + supportResistenceCandleDistance; j++)
                {
                    if (lastBars[i].High >= lastBars[j].High)
                        count++;
                }
                if (count >= supportResistenceCandleDistance * 2)
                    srList.Add(lastBars[i].High);

                count = 0;
                for (int j = i - supportResistenceCandleDistance; j < i + supportResistenceCandleDistance; j++)
                {
                    if (lastBars[i].Low <= lastBars[j].Low)
                        count++;
                }
                if (count >= supportResistenceCandleDistance * 2)
                    srList.Add(lastBars[i].Low);

            }

            var maxDiff = (lastBar.Close / 100) * 1.3M;
            var minDiff = (lastBar.Close / 100) * 0.5M;

            srList.Reverse();
            decimal profitOrLoss = 0;
            foreach (var item in srList)
            {
                var diff = SignalExtensions.GetPercentWithDiffrences(item, lastBar.Close);
                if (diff < maxDiff && diff > minDiff)
                {
                    profitOrLoss = diff;
                    break;
                }
                else
                    continue;
            }
            profitOrLoss = profitOrLoss == 0 ? (SignalExtensions.GetPercentOfValue(lastBar.Close, 0.75M)) : profitOrLoss;
            var pl = new ProfitLossModel();
            if (_TradeType == TradeType.Long)
            {
                pl.Profit = lastBar.Close + profitOrLoss;
                pl.Loss = lastBar.Close - profitOrLoss;
            }
            else
            {
                pl.Loss = lastBar.Close + profitOrLoss;
                pl.Profit = lastBar.Close - profitOrLoss;

            }
            return pl;
        }
        public SignalCheckerResult FisherSignallTriggring(List<AdaSignalCheckingModel> list)
        {
            var adaSignalIndicator = new SignalCheckerResult();
            adaSignalIndicator.TradeType = list[5].Fisher > MaxFisher ? TradeType.Short : list[5].Fisher < MinFisher ? TradeType.Long : TradeType.Unkouwn;

            if (adaSignalIndicator.TradeType == TradeType.Long &&
            (list[0].Fisher < DownLineFisher - MinCrossFishers ||
            list[1].Fisher < DownLineFisher - MinCrossFishers ||
            list[2].Fisher < DownLineFisher - MinCrossFishers ||
            list[3].Fisher < DownLineFisher - MinCrossFishers ||
            list[4].Fisher < DownLineFisher - MinCrossFishers) &&
            (list[1].Fisher < MinFisher && list[2].Fisher < MinFisher && list[3].Fisher < MinFisher && list[4].Fisher < MinFisher && list[5].Fisher < MinFisher))

            //if (adaSignalIndicator.TradeTypeType == TradeTypeType.Long)
            {
                if (list[0].Fisher < DownLineFisher - MinCrossFishers && list[1].Fisher > DownLineFisher && list[0].Fisher + DiffereceBetweenFishers < list[1].Fisher)
                    adaSignalIndicator.HasTriggred = true;

                if (list[1].Fisher < DownLineFisher - MinCrossFishers && list[2].Fisher > DownLineFisher && list[1].Fisher + DiffereceBetweenFishers < list[2].Fisher)
                    adaSignalIndicator.HasTriggred = true;

                if (list[2].Fisher < DownLineFisher - MinCrossFishers && list[3].Fisher > DownLineFisher && list[2].Fisher + DiffereceBetweenFishers < list[3].Fisher)
                    adaSignalIndicator.HasTriggred = true;

                if (list[3].Fisher < DownLineFisher - MinCrossFishers && list[4].Fisher > DownLineFisher && list[3].Fisher + DiffereceBetweenFishers < list[4].Fisher)
                    adaSignalIndicator.HasTriggred = true;

                if (list[4].Fisher < DownLineFisher - MinCrossFishers && list[5].Fisher > DownLineFisher && list[4].Fisher + DiffereceBetweenFishers < list[5].Fisher)
                    adaSignalIndicator.HasTriggred = true;

            }

            if (adaSignalIndicator.TradeType == TradeType.Short &&
      (list[0].Fisher > AboveLineFisher + MinCrossFishers ||
      list[1].Fisher > AboveLineFisher + MinCrossFishers ||
      list[2].Fisher > AboveLineFisher + MinCrossFishers ||
      list[3].Fisher > AboveLineFisher + MinCrossFishers ||
      list[4].Fisher > AboveLineFisher + MinCrossFishers) &&
      (list[1].Fisher > MaxFisher && list[2].Fisher > MaxFisher && list[3].Fisher > MaxFisher && list[4].Fisher > MaxFisher && list[5].Fisher > MaxFisher))

            //if (adaSignalIndicator.TradeTypeType == TradeTypeType.Short)
            {
                if (list[0].Fisher > AboveLineFisher + MinCrossFishers && list[1].Fisher < AboveLineFisher && list[0].Fisher > list[1].Fisher + DiffereceBetweenFishers)
                    adaSignalIndicator.HasTriggred = true;

                if (list[1].Fisher > AboveLineFisher + MinCrossFishers && list[2].Fisher < AboveLineFisher && list[1].Fisher > list[2].Fisher + DiffereceBetweenFishers)
                    adaSignalIndicator.HasTriggred = true;

                if (list[2].Fisher > AboveLineFisher + MinCrossFishers && list[3].Fisher < AboveLineFisher && list[2].Fisher > list[3].Fisher + DiffereceBetweenFishers)
                    adaSignalIndicator.HasTriggred = true;

                if (list[3].Fisher > AboveLineFisher + MinCrossFishers && list[4].Fisher < AboveLineFisher && list[3].Fisher > list[4].Fisher + DiffereceBetweenFishers)
                    adaSignalIndicator.HasTriggred = true;

                if (list[4].Fisher > AboveLineFisher + MinCrossFishers && list[5].Fisher < AboveLineFisher && list[4].Fisher > list[5].Fisher + DiffereceBetweenFishers)
                    adaSignalIndicator.HasTriggred = true;

            }

            ////duplicate
            //if (adaSignalIndicator.HasTriggred)
            //{
            //    adaSignalIndicator.HasTriggred = false;

            //    if (adaSignalIndicator.TradeTypeType == TradeTypeType.Long)
            //        if (list[3].Fisher > DownLineFisher && list[3].Fisher > list[2].Fisher + DiffereceBetweenFishers)
            //            adaSignalIndicator.HasTriggred = true;

            //    if (adaSignalIndicator.TradeTypeType == TradeTypeType.Short)
            //        if (list[3].Fisher < AboveLineFisher && list[3].Fisher + DiffereceBetweenFishers < list[2].Fisher)
            //            adaSignalIndicator.HasTriggred = true;

            //}

            return adaSignalIndicator;
        }


        public SignalCheckerResult StochSignallTriggring(List<AdaSignalCheckingModel> list)
        {
            var adaSignalIndicator = new SignalCheckerResult();
            adaSignalIndicator.TradeType = list[5].Stoch > MaxStoch ? TradeType.Short : list[5].Stoch < MinStoch ? TradeType.Long : TradeType.Unkouwn;

            if (adaSignalIndicator.TradeType == TradeType.Long &&
                        (list[0].Stoch < DownLineStoch - MinCrossStoches ||
                        list[1].Stoch < DownLineStoch - MinCrossStoches ||
                        list[2].Stoch < DownLineStoch - MinCrossStoches ||
                        list[3].Stoch < DownLineStoch - MinCrossStoches ||
                        list[4].Stoch < DownLineStoch - MinCrossStoches) &&
                        (list[1].Stoch < MinStoch && list[2].Stoch < MinStoch && list[3].Stoch < MinStoch && list[4].Stoch < MinStoch && list[5].Stoch < MinStoch))

            //      if (adaSignalIndicator.TradeTypeType == TradeTypeType.Long)
            {

                if (list[0].Stoch < DownLineStoch - MinCrossStoches && list[1].Stoch > DownLineStoch && list[0].Stoch + DiffereceBetweenStoches < list[1].Stoch)
                    adaSignalIndicator.HasTriggred = true;

                if (list[1].Stoch < DownLineStoch - MinCrossStoches && list[2].Stoch > DownLineStoch && list[1].Stoch + DiffereceBetweenStoches < list[2].Stoch)
                    adaSignalIndicator.HasTriggred = true;

                if (list[2].Stoch < DownLineStoch - MinCrossStoches && list[3].Stoch > DownLineStoch && list[2].Stoch + DiffereceBetweenStoches < list[3].Stoch)
                    adaSignalIndicator.HasTriggred = true;

                if (list[3].Stoch < DownLineStoch - MinCrossStoches && list[4].Stoch > DownLineStoch && list[3].Stoch + DiffereceBetweenStoches < list[4].Stoch)
                    adaSignalIndicator.HasTriggred = true;

                if (list[4].Stoch < DownLineStoch - MinCrossStoches && list[5].Stoch > DownLineStoch && list[4].Stoch + DiffereceBetweenStoches < list[5].Stoch)
                    adaSignalIndicator.HasTriggred = true;

            }

            if (adaSignalIndicator.TradeType == TradeType.Short &&
(list[0].Stoch > AboveLineStoch + MinCrossStoches ||
list[1].Stoch > AboveLineStoch + MinCrossStoches ||
list[2].Stoch > AboveLineStoch + MinCrossStoches ||
list[3].Stoch > AboveLineStoch + MinCrossStoches ||
list[4].Stoch > AboveLineStoch + MinCrossStoches) &&
(list[1].Stoch > MaxStoch && list[2].Stoch > MaxStoch && list[3].Stoch > MaxStoch && list[4].Stoch > MaxStoch && list[5].Stoch > MaxStoch))
            //if (adaSignalIndicator.TradeTypeType == TradeTypeType.Short)
            {
                if (list[0].Stoch > AboveLineStoch + MinCrossStoches && list[1].Stoch < AboveLineStoch && list[0].Stoch > list[1].Stoch + DiffereceBetweenStoches)
                    adaSignalIndicator.HasTriggred = true;

                if (list[1].Stoch > AboveLineStoch + MinCrossStoches && list[2].Stoch < AboveLineStoch && list[1].Stoch > list[2].Stoch + DiffereceBetweenStoches)
                    adaSignalIndicator.HasTriggred = true;

                if (list[2].Stoch > AboveLineStoch + MinCrossStoches && list[3].Stoch < AboveLineStoch && list[2].Stoch > list[3].Stoch + DiffereceBetweenStoches)
                    adaSignalIndicator.HasTriggred = true;

                if (list[3].Stoch > AboveLineStoch + MinCrossStoches && list[4].Stoch < AboveLineStoch && list[3].Stoch > list[4].Stoch + DiffereceBetweenStoches)
                    adaSignalIndicator.HasTriggred = true;

                if (list[4].Stoch > AboveLineStoch + MinCrossStoches && list[5].Stoch < AboveLineStoch && list[4].Stoch > list[5].Stoch + DiffereceBetweenStoches)
                    adaSignalIndicator.HasTriggred = true;

            }


            //if (adaSignalIndicator.HasTriggred)
            //{
            //    adaSignalIndicator.HasTriggred = false;

            //    if (adaSignalIndicator.TradeTypeType == TradeTypeType.Long)
            //        if (list[3].Stoch > DownLineStoch && list[3].Stoch > list[2].Stoch + DiffereceBetweenStoches)
            //            adaSignalIndicator.HasTriggred = true;

            //    if (adaSignalIndicator.TradeTypeType == TradeTypeType.Short)
            //        if (list[3].Stoch < AboveLineStoch && list[3].Stoch + DiffereceBetweenStoches < list[2].Stoch)
            //            adaSignalIndicator.HasTriggred = true;

            //}

            return adaSignalIndicator;
        }


        public SignalCheckerResult PSarSignallTriggring(List<AdaSignalCheckingModel> list)
        {
            var adaSignalIndicator = new SignalCheckerResult();
            var pSarIsReverselCount = list.Where(p => p.PSarIsReversel == true).Count();
            if (pSarIsReverselCount == 1)
                adaSignalIndicator.HasTriggred = true;

            // if (list[1].PSarIsReversel == true || list[2].PSarIsReversel == true || list[3].PSarIsReversel == true)
            //    adaSignalIndicator.HasTriggred = true;

            if (adaSignalIndicator.HasTriggred && list[5].PSar > list[4].PSar)
                adaSignalIndicator.TradeType = TradeType.Short;

            if (adaSignalIndicator.HasTriggred && list[5].PSar < list[4].PSar)
                adaSignalIndicator.TradeType = TradeType.Long;

            return adaSignalIndicator;
        }
    }



}
