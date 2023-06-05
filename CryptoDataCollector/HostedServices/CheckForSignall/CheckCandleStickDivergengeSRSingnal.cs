using Accord;
using Accord.Math.Optimization.Losses;
using CryptoDataCollector.BussinesExtensions;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
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
    public class CheckCandleStickDivergengeSRSingnal
    {

        public List<CandleStichDivergengeSRSignalCheckingModel> AllList { get; set; }
        public CandleStichDivergengeSRSignalCheckingModel LastCandle { get; set; }
        public TradeType _TradeType { get; set; }
        public SignalType? _SignalType { get; set; }
        public ChartProcessType? _ProcessType { get; set; }
        public FutureSlopeType? _FutureSlopeType { get; set; }
        public PriceToMovingAverageType? _PriceToSmaType { get; set; }
        public bool _IsEmotional { get; set; }
        public ProfitLossModel? ProfitLess { get; set; }
        public GeneralStaus? _GeneralStatus { get; set; }
        public Trade LastTrade { get; set; }
        public Symbol _Symbol { get; set; }
        public TimeFrameType _TimeFrameType { get; set; } = TimeFrameType.Hour1;

        public ProfitLossModel? LastTradedProfitLess { get; set; } = new ProfitLossModel();

        public ApplicationDbContext _context { get; set; }

        public readonly IDbConnection _dbConnection;

        public decimal StopLoss { get; set; }
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
        public int LeftSr { get; set; } = 200;
        public int RightSr { get; set; } = 10;

        public CheckCandleStickDivergengeSRSingnal(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
            LastTrade = _context.Trades.AsNoTracking().OrderByDescending(p => p.Id).FirstOrDefault();
        }


        public void SetParams(int step)
        {

            if (step == 1)
            {
                ////timeframe 5 min
                _Symbol = Symbol.Bnb;
                StopLoss = 0.25M;
                CountDecimal = 1;
                RR = 1.5M;
            }
            else if (step == 2)
            {
                _Symbol = Symbol.Ada;
                StopLoss = 0.4M;
                CountDecimal = 4;
                RR = 1.5M;
            }
            else if (step == 3)
            {

                _Symbol = Symbol.Atom;

            }
            else if (step == 4)
            {
                _Symbol = Symbol.Xrp;
                StopLoss = 0.48M;//percent
                CountDecimal = 4;
                RR = 1.5M;
            }
            else if (step == 5)
            {
                _Symbol = Symbol.Sol;
                StopLoss = 0.4M;//percent
                CountDecimal = 2;
                RR = 1.5M;
            }
            else if (step == 6)
            {

                _Symbol = Symbol.Eth;
                StopLoss = 0.32M;//percent
                CountDecimal = 2;
                RR = 1.5M;

            }
            else if (step == 7)
            {
                ////timeframe 5 min
                _Symbol = Symbol.Btc;
                //StopLoss = 0.25M;//percent
                CountDecimal = 2;
                //RR = 1.5M;


                ////timeframe 1 hour
                StopLoss = 1.5m; //1.5M;//percent
                RR = 1M;
            }
            else
            {
                Thread.Sleep(30000);
            }



        }

        public SignalCheckerType CheckStrategy(List<CandleStichDivergengeSRSignalCheckingModel> list, int step)
        {
            SetParams(step);
            //   return SignalCheckerType.HaveBefore;
            _Symbol = (Symbol)step;
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
                var pl = CalculateProfitLess();
                tradePending.Profit = pl.Profit;
                tradePending.Loss = pl.Loss;
                tradePending.SignalCandleClosePrice = LastCandle.Open;
                _context.SaveChanges();
            }

            CheckForSell();


            _ProcessType = SignalExtensions.GetChartProcessType(GetBaseSignalCheckingList(list), 40);
            _PriceToSmaType = (double)LastCandle.Close > LastCandle.Cci ? PriceToMovingAverageType.Over : PriceToMovingAverageType.Under;
            _TradeType = GetTradeType();


            if (LastCandle.DateTime == new DateTime(2023, 3, 24, 22, 15, 0))
            {

            }

            if (LastCandle.DateTime == new DateTime(2023, 3, 30, 19, 50, 0))
            {

            }

            var mode1 = ChckForLastestCandleColors();
            var mode2 = ChckForExistingCandleStick();
       //     var mode3 = ChckForSR();
            var mode4 = ChckForExistingDivergence();
            if (mode1.HasTriggred && mode2.HasTriggred /*&& mode3.HasTriggred*/ && mode4.HasTriggred)
            {
                var divergences = GetExistingDivergences();

                _SignalType = SignalType.DivergenceCandleStickSr;
                AverageOf5LatestVolume = AllList.TakeLast(5).Average(p => p.Volume);
                AverageOf10LatestVolume = AllList.TakeLast(10).Average(p => p.Volume);
                return Buy();
            }

            return SignalCheckerType.NotHappendInBuyChecker;

        }

        public SignalCheckerResult ChckForLastestCandleColors()
        {

            var lastBars = AllList.TakeLast(4).ToList().Take(3).ToList();
            int count = 0;

            if (_TradeType == TradeType.Short)
            {
                foreach (var item in lastBars)
                    if (SignalExtensions.GetCandleType(GetBaseSignalCheckingModel(item)) != CandleType.Red)
                        count++;

                if (count >= 2)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
            }

            else if (_TradeType == TradeType.Long)
            {
                foreach (var item in lastBars)
                    if (SignalExtensions.GetCandleType(GetBaseSignalCheckingModel(item)) == CandleType.Red)
                        count++;

                if (count >= 2)
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
            }
            return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };
        }



        public SignalCheckerResult ChckForExistingCandleStick()
        {

            var lastBars = AllList.TakeLast(4).ToList();
            var candleSticks = new List<CandleStickType>();

            if (_TradeType == TradeType.Short)
            {
                for (int i = 0; i < 3; i++)
                {
                    //if (SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = lastBars[i].Close, Open = lastBars[i].Open }) == CandleType.Gray ||
                    //    SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = lastBars[i + 1].Close, Open = lastBars[i + 1].Open }) == CandleType.Gray)
                    //    continue;

                    candleSticks.AddRange(SignalExtensions.GetCandleStick(lastBars.Skip(i).Take(2).Select(p => new Candle() { Open = p.Open, High = p.High, Low = p.Low, Close = p.Close, DateTime = p.DateTime }).ToList()));
                }
                if (candleSticks.Contains(CandleStickType.BearishHarami) || candleSticks.Contains(CandleStickType.BearishEngulfing))
                {
                    CandleTypeInt = (int)candleSticks.First();
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
                }

            }

            else if (_TradeType == TradeType.Long)
            {
                for (int i = 0; i < 3; i++)
                {
                    //if (SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = lastBars[i].Close, Open = lastBars[i].Open }) == CandleType.Gray ||
                    //    SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = lastBars[i + 1].Close, Open = lastBars[i + 1].Open }) == CandleType.Gray)
                    //    continue;

                    candleSticks.AddRange(SignalExtensions.GetCandleStick(lastBars.Skip(i).Take(2).Select(p => new Candle() { Open = p.Open, High = p.High, Low = p.Low, Close = p.Close, DateTime = p.DateTime }).ToList()));
                }
                if (candleSticks.Contains(CandleStickType.BulishHarami) || candleSticks.Contains(CandleStickType.BulishEngulfing))
                {
                    CandleTypeInt = (int)candleSticks.First();
                    return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
                }
            }
            return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };
        }



        public SignalCheckerResult ChckForExistingDivergence()
        {

            var divergences = GetExistingDivergences();
            if (divergences == null || divergences.Count == 0)
                return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };

            var test = LastCandle.DateTime;
            var testBars = AllList.OrderByDescending(p => p.DateTime).ToList();
            if (divergences.Count > 0)
                return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };

            return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };
        }


        public List<IndicatorNameType> GetExistingDivergences()
        {
            var divergences = new List<IndicatorNameType>();


            var pivot = IsPivot();
            if (pivot is null)
                return null;

            var pivots = GetLastPivots(PivotPeriod);


            var lastCandleIndex = AllList.Count - 2;
            LastPivotVolume = (int)AllList[lastCandleIndex].Volume;
            for (int i = 0; i < CountOfLastestPP; i++)
            {

                if (pivot.HighLowType == HighLowType.High)
                {
                    int lastPivotPointHighIndex = pivots.Ph.ElementAt(i).Key;
                    var lastPivotPointHigh = pivots.Ph.ElementAt(i).Value;

                    DistanceBetweenPivots = lastCandleIndex - lastPivotPointHighIndex;

                    if (!IsGoodSlopeForVirtualLine(lastPivotPointHighIndex, lastCandleIndex, lastPivotPointHigh, pivot.Price))
                        continue;

                    if (lastPivotPointHigh > pivot.Price)
                    {


                        if (AllList[lastPivotPointHighIndex].Rsi < AllList[lastCandleIndex].Rsi && VirtualLineDontBeBroken(AllList.Select(p => p.Rsi).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Rsi, LastCandle.Rsi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Rsi);

                        if (AllList[lastPivotPointHighIndex].Macd < AllList[lastCandleIndex].Macd && VirtualLineDontBeBroken(AllList.Select(p => p.Macd).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Macd, LastCandle.Macd, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Macd);

                        if (AllList[lastPivotPointHighIndex].MacdHist < AllList[lastCandleIndex].MacdHist && VirtualLineDontBeBroken(AllList.Select(p => p.MacdHist).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].MacdHist, LastCandle.MacdHist, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.MacdHist);

                        if (AllList[lastPivotPointHighIndex].Mom < AllList[lastCandleIndex].Mom && VirtualLineDontBeBroken(AllList.Select(p => p.Mom).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Mom, LastCandle.Mom, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mom);

                        if (AllList[lastPivotPointHighIndex].Cci < AllList[lastCandleIndex].Cci && VirtualLineDontBeBroken(AllList.Select(p => p.Cci).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Cci, LastCandle.Cci, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cci);

                        if (AllList[lastPivotPointHighIndex].Stoch < AllList[lastCandleIndex].Stoch && VirtualLineDontBeBroken(AllList.Select(p => p.Stoch).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Stoch, LastCandle.Stoch, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Stoch);

                        if (AllList[lastPivotPointHighIndex].Cmf < AllList[lastCandleIndex].Cmf && VirtualLineDontBeBroken(AllList.Select(p => p.Cmf).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Cmf, LastCandle.Cmf, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cmf);

                        if (AllList[lastPivotPointHighIndex].Mfi < AllList[lastCandleIndex].Mfi && VirtualLineDontBeBroken(AllList.Select(p => p.Mfi).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Mfi, LastCandle.Mfi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mfi);

                    }
                    if (lastPivotPointHigh < pivot.Price)
                    {

                        if (AllList[lastPivotPointHighIndex].Rsi > AllList[lastCandleIndex].Rsi && VirtualLineDontBeBroken(AllList.Select(p => p.Rsi).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Rsi, LastCandle.Rsi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Rsi);

                        if (AllList[lastPivotPointHighIndex].Macd > AllList[lastCandleIndex].Macd && VirtualLineDontBeBroken(AllList.Select(p => p.Macd).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Macd, LastCandle.Macd, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Macd);

                        if (AllList[lastPivotPointHighIndex].MacdHist > AllList[lastCandleIndex].MacdHist && VirtualLineDontBeBroken(AllList.Select(p => p.MacdHist).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].MacdHist, LastCandle.MacdHist, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.MacdHist);

                        if (AllList[lastPivotPointHighIndex].Mom > AllList[lastCandleIndex].Mom && VirtualLineDontBeBroken(AllList.Select(p => p.Mom).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Mom, LastCandle.Mom, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mom);

                        if (AllList[lastPivotPointHighIndex].Cci > AllList[lastCandleIndex].Cci && VirtualLineDontBeBroken(AllList.Select(p => p.Cci).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Cci, LastCandle.Cci, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cci);

                        if (AllList[lastPivotPointHighIndex].Stoch > AllList[lastCandleIndex].Stoch && VirtualLineDontBeBroken(AllList.Select(p => p.Stoch).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Stoch, LastCandle.Stoch, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Stoch);

                        if (AllList[lastPivotPointHighIndex].Cmf > AllList[lastCandleIndex].Cmf && VirtualLineDontBeBroken(AllList.Select(p => p.Cmf).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Cmf, LastCandle.Cmf, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cmf);

                        if (AllList[lastPivotPointHighIndex].Mfi > AllList[lastCandleIndex].Mfi && VirtualLineDontBeBroken(AllList.Select(p => p.Mfi).ToList(), lastPivotPointHighIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointHighIndex].Mfi, LastCandle.Mfi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mfi);

                    }
                }

                if (pivot.HighLowType == HighLowType.Low)
                {
                    int lastPivotPointLowIndex = pivots.Pl.ElementAt(i).Key;
                    var lastPivotPointLow = pivots.Pl.ElementAt(i).Value;
                    DistanceBetweenPivots = lastCandleIndex - lastPivotPointLowIndex;
                    if (!IsGoodSlopeForVirtualLine(lastPivotPointLowIndex, lastCandleIndex, lastPivotPointLow, pivot.Price))
                        continue;


                    if (lastPivotPointLow > pivot.Price)
                    {

                        if (AllList[lastPivotPointLowIndex].Rsi < AllList[lastCandleIndex].Rsi && VirtualLineDontBeBroken(AllList.Select(p => p.Rsi).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Rsi, LastCandle.Rsi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Rsi);

                        if (AllList[lastPivotPointLowIndex].Macd < AllList[lastCandleIndex].Macd && VirtualLineDontBeBroken(AllList.Select(p => p.Macd).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Macd, LastCandle.Macd, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Macd);

                        if (AllList[lastPivotPointLowIndex].MacdHist < AllList[lastCandleIndex].MacdHist && VirtualLineDontBeBroken(AllList.Select(p => p.MacdHist).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].MacdHist, LastCandle.MacdHist, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.MacdHist);

                        if (AllList[lastPivotPointLowIndex].Mom < AllList[lastCandleIndex].Mom && VirtualLineDontBeBroken(AllList.Select(p => p.Mom).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Mom, LastCandle.Mom, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mom);

                        if (AllList[lastPivotPointLowIndex].Cci < AllList[lastCandleIndex].Cci && VirtualLineDontBeBroken(AllList.Select(p => p.Cci).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Cci, LastCandle.Cci, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cci);

                        if (AllList[lastPivotPointLowIndex].Stoch < AllList[lastCandleIndex].Stoch && VirtualLineDontBeBroken(AllList.Select(p => p.Stoch).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Stoch, LastCandle.Stoch, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Stoch);

                        if (AllList[lastPivotPointLowIndex].Cmf < AllList[lastCandleIndex].Cmf && VirtualLineDontBeBroken(AllList.Select(p => p.Cmf).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Cmf, LastCandle.Cmf, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cmf);

                        if (AllList[lastPivotPointLowIndex].Mfi < AllList[lastCandleIndex].Mfi && VirtualLineDontBeBroken(AllList.Select(p => p.Mfi).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Mfi, LastCandle.Mfi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mfi);


                    }
                    if (lastPivotPointLow < pivot.Price)
                    {

                        if (AllList[lastPivotPointLowIndex].Rsi > AllList[lastCandleIndex].Rsi && VirtualLineDontBeBroken(AllList.Select(p => p.Rsi).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Rsi, LastCandle.Rsi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Rsi);

                        if (AllList[lastPivotPointLowIndex].Macd > AllList[lastCandleIndex].Macd && VirtualLineDontBeBroken(AllList.Select(p => p.Macd).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Macd, LastCandle.Macd, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Macd);


                        if (AllList[lastPivotPointLowIndex].MacdHist > AllList[lastCandleIndex].MacdHist && VirtualLineDontBeBroken(AllList.Select(p => p.MacdHist).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].MacdHist, LastCandle.MacdHist, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.MacdHist);

                        if (AllList[lastPivotPointLowIndex].Mom > AllList[lastCandleIndex].Mom && VirtualLineDontBeBroken(AllList.Select(p => p.Mom).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Mom, LastCandle.Mom, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mom);

                        if (AllList[lastPivotPointLowIndex].Cci > AllList[lastCandleIndex].Cci && VirtualLineDontBeBroken(AllList.Select(p => p.Cci).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Cci, LastCandle.Cci, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cci);

                        if (AllList[lastPivotPointLowIndex].Stoch > AllList[lastCandleIndex].Stoch && VirtualLineDontBeBroken(AllList.Select(p => p.Stoch).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Stoch, LastCandle.Stoch, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Stoch);

                        if (AllList[lastPivotPointLowIndex].Cmf > AllList[lastCandleIndex].Cmf && VirtualLineDontBeBroken(AllList.Select(p => p.Cmf).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Cmf, LastCandle.Cmf, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Cmf);

                        if (AllList[lastPivotPointLowIndex].Mfi > AllList[lastCandleIndex].Mfi && VirtualLineDontBeBroken(AllList.Select(p => p.Mfi).ToList(), lastPivotPointLowIndex, lastCandleIndex, pivot.HighLowType))
                            if (IsConfirmed(AllList[lastPivotPointLowIndex].Mfi, LastCandle.Mfi, pivot.HighLowType))
                                divergences.Add(IndicatorNameType.Mfi);

                    }
                }

                if (divergences.Any())
                    break;

            }

            var sb = new StringBuilder();
            if (divergences.Any())
            {
                foreach (var item in divergences)
                {
                    sb.Append(item.ToString() + "---");
                }
                sb.Remove(sb.Length - 3, 3);
            }
            Description = sb.ToString();
            return divergences;
        }

        public bool IsGoodSlopeForVirtualLine(int lastPpIndex, int lastCheckIndex, decimal firstVal, decimal secondVal)
        {
            var diff = SignalExtensions.GetPercentWithDiffrences(firstVal, secondVal);
            var diffDistance = lastCheckIndex - lastPpIndex;
            var slopePercent = Math.Round((diff / diffDistance) * PivotPeriod, 2);
            if (slopePercent < SlopeVirtualLine)
                return false;

            SlopePricePercent = slopePercent;
            return true;

        }

        public bool VirtualLineDontBeBroken(List<double> indicatorValues, int lastPpIndex, int lastCheckIndex, HighLowType highLowType)
        {
            var closes = AllList.Select(p => p.Close).ToList();
            if (highLowType == HighLowType.High)
            {
                var slopePrice = (closes[lastCheckIndex] - closes[lastPpIndex]) / (lastCheckIndex - lastPpIndex);
                var slopeIndicator = (indicatorValues[lastCheckIndex] - indicatorValues[lastPpIndex]) / (lastCheckIndex - lastPpIndex);

                if (closes[lastPpIndex] > closes[lastCheckIndex])
                {
                    slopePrice = -1 * Math.Abs(slopePrice);
                    slopeIndicator = -1 * Math.Abs(slopeIndicator);
                }
                else
                {
                    slopePrice = Math.Abs(slopePrice);
                    slopeIndicator = Math.Abs(slopeIndicator);
                }

                decimal slopePriceDynamic = 0;
                double slopeIndicatorDynamic = 0;
                for (int i = 0; i < (lastCheckIndex - lastPpIndex); i++)
                {
                    slopePriceDynamic = slopePrice * i;
                    slopeIndicatorDynamic = slopeIndicator * i;

                    if (closes[lastPpIndex] + slopePriceDynamic < closes[lastPpIndex + i])
                    {
                        return false;
                    }
                    if (indicatorValues[lastPpIndex] - slopeIndicatorDynamic < indicatorValues[lastPpIndex + i])
                    {
                        return false;
                    }
                }

            }
            if (highLowType == HighLowType.Low)
            {

                var slopePrice = (closes[lastCheckIndex] - closes[lastPpIndex]) / (lastCheckIndex - lastPpIndex);
                var slopeIndicator = (indicatorValues[lastCheckIndex] - indicatorValues[lastPpIndex]) / (lastCheckIndex - lastPpIndex);
                if (closes[lastPpIndex] > closes[lastCheckIndex])
                {
                    slopePrice = -1 * Math.Abs(slopePrice);
                    slopeIndicator = -1 * Math.Abs(slopeIndicator);
                }
                else
                {
                    slopePrice = Math.Abs(slopePrice);
                    slopeIndicator = Math.Abs(slopeIndicator);
                }

                decimal slopePriceDynamic = 0;
                double slopeIndicatorDynamic = 0;
                for (int i = 0; i < (lastCheckIndex - lastPpIndex); i++)
                {
                    slopePriceDynamic = slopePrice * i;
                    slopeIndicatorDynamic = slopeIndicator * i;

                    if (closes[lastPpIndex] + slopePriceDynamic > closes[lastPpIndex + i])
                    {
                        return false;
                    }
                    if (indicatorValues[lastPpIndex] - slopeIndicatorDynamic > indicatorValues[lastPpIndex + i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsConfirmed(double lastIndicator, double currentIndicator, HighLowType highLowType)
        {
            if (highLowType == HighLowType.High)
                if (SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Open = LastCandle.Open, Close = LastCandle.Close }) == CandleType.Red &&
                SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = AllList[AllList.Count - 2].Close, Open = AllList[AllList.Count - 2].Open }) != CandleType.Red
                    || currentIndicator < lastIndicator)
                    return true;
                else
                    return false;
            if (highLowType == HighLowType.Low)
                if (SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Open = LastCandle.Open, Close = LastCandle.Close }) != CandleType.Red &&
                SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = AllList[AllList.Count - 2].Close, Open = AllList[AllList.Count - 2].Open }) == CandleType.Red
                || currentIndicator > lastIndicator)
                    return true;
                else
                    return false;

            return false;

        }



        public SignalCheckerResult ChckForSR()
        {
            var sr = GetSupportResistence();
            if (_TradeType == TradeType.Long)
            {
                if (SignalExtensions.SelectTopOfBodyCandle(new BaseSignalCheckingModel() { Open = LastCandle.Open, Close = LastCandle.Close }) >= sr.Support)
                {
                    if (LastCandle.Low <= sr.Support || SignalExtensions.GetPercentWithDiffrences(LastCandle.Low, sr.Support) <= 0.1M)
                    {
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
                    }
                }
            }
            else if (_TradeType == TradeType.Short)
            {
                if (SignalExtensions.SelectBottomOfBodyCandle(new BaseSignalCheckingModel() { Open = LastCandle.Open, Close = LastCandle.Close }) <= sr.Resistence)
                {
                    if (LastCandle.High >= sr.Resistence || SignalExtensions.GetPercentWithDiffrences(sr.Resistence, LastCandle.High) <= 0.1M)
                    {
                        return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = true };
                    }
                }
            }
            return new SignalCheckerResult() { TradeType = _TradeType, HasTriggred = false };
        }


        public SupportResistenceModel GetSupportResistence()
        {
            try
            {


                var srModel = new SupportResistenceModel();
                var res = GetSRPivots(LeftSr, RightSr);
                if (res.Pl is null || res.Pl.Count == 0)
                    res.Pl.Add(AllList.Count - RightSr - 1, LastCandle.Low / 2);

                if (res.Ph is null || res.Ph.Count == 0)
                    res.Ph.Add(AllList.Count - RightSr - 1, LastCandle.High * 2);

                srModel.Support = res.Pl.ElementAt(0).Value;
                srModel.Resistence = res.Ph.ElementAt(0).Value;
                ResisetanceSupportDiff = SignalExtensions.GetPercentWithDiffrences(srModel.Support, srModel.Resistence);
                return srModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public PivotsModel GetLastPivots(int pivotPeriod)
        {
            var pivots = new PivotsModel();

            //fiding high
            for (int i = AllList.Count - pivotPeriod - 2; i > pivotPeriod; i--)
            {
                if (pivots.Ph.Count > CountOfLastestPP)
                    break;
                int count = 0;
                int countItself = 0;
                for (int j = i - pivotPeriod; j <= i + pivotPeriod; j++)
                {

                    if (AllList[i].Close > AllList[j].Close)
                        count++;
                    else if (countItself == 0 && AllList[i].Close == AllList[j].Close)
                        countItself++;
                    else
                        break;

                    if (count == pivotPeriod * 2)
                    {
                        pivots.Ph.Add(i, AllList[i].Close);
                        break;
                    }
                }
            }

            //fiding low
            for (int i = AllList.Count - pivotPeriod - 2; i > pivotPeriod; i--)
            {
                if (pivots.Pl.Count > CountOfLastestPP)
                    break;
                int count = 0;
                int countItself = 0;
                for (int j = i - pivotPeriod; j <= i + pivotPeriod; j++)
                {

                    if (AllList[i].Close < AllList[j].Close)
                        count++;
                    else if (countItself == 0 && AllList[i].Close == AllList[j].Close)
                        countItself++;
                    else
                        break;

                    if (count == pivotPeriod * 2)
                    {
                        pivots.Pl.Add(i, AllList[i].Close);
                        break;
                    }
                }
            }

            return pivots;
        }


        public PivotsModel GetSRPivots(int left, int right)
        {
            try
            {


                var pivots = new PivotsModel();
                //fiding high
                for (int i = AllList.Count - right - 1; i > left; i--)
                {
                    if (pivots.Ph.Count > CountOfLastestPP)
                        break;
                    int count = 0;
                    int countItself = 0;
                    for (int j = i - left; j <= i + right; j++)
                    {

                        if (AllList[i].High > AllList[j].High)
                            count++;
                        else if (countItself == 0 && AllList[i].High == AllList[j].High)
                            countItself++;
                        else
                            break;

                        if (count == left + right && !pivots.Ph.Where(p => p.Value == AllList[i].High).Any())
                        {
                            pivots.Ph.Add(i, AllList[i].High);
                            break;
                        }
                    }
                }

                //fiding low
                for (int i = AllList.Count - right - 1; i > left; i--)
                {
                    if (pivots.Pl.Count > CountOfLastestPP)
                        break;
                    int count = 0;
                    int countItself = 0;
                    for (int j = i - left; j <= i + right; j++)
                    {

                        if (AllList[i].Low < AllList[j].Low)
                            count++;
                        else if (countItself == 0 && AllList[i].Low == AllList[j].Low)
                            countItself++;
                        else
                            break;

                        if (count == left + right && !pivots.Pl.Where(p => p.Value == AllList[i].Low).Any())
                        {
                            pivots.Pl.Add(i, AllList[i].Low);
                            break;
                        }
                    }
                }

                return pivots;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public PivotModel IsPivot()
        {
            int pivotPeriod = PivotPeriod;
            var pivot = new PivotModel();

            var price = AllList[AllList.Count - 2].Close;

            int count = 0;
            //is high?
            if (SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = AllList[AllList.Count - 2].Close, Open = AllList[AllList.Count - 2].Open }) != CandleType.Red &&
                SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = AllList[AllList.Count - 1].Close, Open = AllList[AllList.Count - 1].Open }) == CandleType.Red)
            {
                for (int i = AllList.Count - pivotPeriod - 1; i < AllList.Count - 1; i++)
                {
                    if (price >= AllList[i].Close)
                        count++;
                }
                if (count == pivotPeriod)
                {
                    pivot.HighLowType = HighLowType.High;
                    _TradeType = TradeType.Short;
                    pivot.Price = price;
                    return pivot;
                }
            }
            count = 0;
            //is low?
            if (SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = AllList[AllList.Count - 2].Close, Open = AllList[AllList.Count - 2].Open }) == CandleType.Red &&
                SignalExtensions.GetCandleType(new BaseSignalCheckingModel() { Close = AllList[AllList.Count - 1].Close, Open = AllList[AllList.Count - 1].Open }) != CandleType.Red)
            {
                for (int i = AllList.Count - pivotPeriod - 1; i < AllList.Count - 1; i++)
                {
                    if (price <= AllList[i].Close)
                        count++;
                }
                if (count == pivotPeriod)
                {
                    pivot.HighLowType = HighLowType.Low;
                    _TradeType = TradeType.Long;
                    pivot.Price = price;
                    return pivot;
                }
            }
            _TradeType = TradeType.Unkouwn;
            return null;
        }

        public TradeType GetTradeType()
        {
            IsPivot();
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
                CalculateProfitLess();

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
                    SignalType = SignalType.DivergenceCandleStickSr,
                    CountRedCandles = DistanceBetweenPivots,
                    CountGrayCandles = (int)LastCandle.Volume,
                    CountGreenCandles = LastPivotVolume,
                    ThirdLastCandleVolume = AllList.OrderBy(p => p.DateTime).TakeLast(3).First().Volume,
                    ForthLastCandleVolume = AllList.OrderBy(p => p.DateTime).TakeLast(4).First().Volume,
                    DistancePercentFromSma = ResisetanceSupportDiff.Value,
                    Indicator1 = SlopePricePercent.Value,
                    Indicator2 = AverageOf5LatestVolume.Value,
                    Indicator3 = AverageOf10LatestVolume.Value,
                    SignalCandleClosePrice = 0,
                    Profit = 0,
                    Loss = 0,
                    Description = Description,
                    Leverage = CandleTypeInt,
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
                Console.WriteLine("==========>        pl.profit: " + ProfitLess.Profit.ToString() + " -----  pl.loss: " + ProfitLess.Loss.ToString());
                return SignalCheckerType.Buy;
            }
            return SignalCheckerType.HaveBefore;
        }


        public ProfitLossModel CalculateProfitLess()
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
