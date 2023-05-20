using Skender.Stock.Indicators;

namespace CryptoDataCollector.BussinesExtensions.Helper
{
    public class TAHelper
    {
        private readonly decimal[]? Lowadj;
        private readonly decimal[]? highAdj;
        private readonly decimal[]? lastAdj;
        private readonly decimal[]? openAdj;
        private readonly decimal[]? volume;
        private readonly Quote[] Quotes;
        public TAHelper(decimal[] lowadj,
            decimal[] highAdj,
            decimal[] lastAdj,
            decimal[] openAdj,
            decimal[] volume,
            System.DateTime[] date)
        {
            Lowadj = lowadj;
            this.highAdj = highAdj;
            this.lastAdj = lastAdj;
            this.openAdj = openAdj;
            this.volume = volume;
            Quotes = new Quote[highAdj.Length];
            for (int i = 0; i < lowadj.Length; i++)
            {
                Quotes[i] = new Quote()
                {
                    Close = lastAdj[i],
                    Date = date[i],
                    High = highAdj[i],
                    Low = lowadj[i],
                    Open = openAdj[i],
                    Volume = volume[i]
                };
            }
        }


        public decimal[] Sar()
        {
            decimal[] SARFinalData = new decimal[Lowadj.Length];
            _ = TALib.Core.Sar(inHigh: highAdj,
                                   inLow: Lowadj,
                                   startIdx: 0,
                                   endIdx: highAdj.Length - 1,
                                   outReal: SARFinalData,
                                   outBegIdx: out var f,
                                   outNbElement: out var e);
            SARFinalData = ShiftData(SARFinalData, f, e);
            return SARFinalData;
        }
        public decimal[] Ad()
        {
            decimal[] ADFinalData = new decimal[Lowadj.Length];
            TALib.Core.Ad(inHigh: highAdj,
                              inLow: Lowadj,
                              inClose: lastAdj,
                              inVolume: volume,
                              startIdx: 0,
                              endIdx: highAdj.Length - 1,
                              outReal: ADFinalData,
                              outBegIdx: out var f,
                              outNbElement: out var e);

            ADFinalData = ShiftData(ADFinalData, f, e);
            return ADFinalData;
        }
        public decimal[] AdOsc()
        {
            decimal[] AdOscFinalData = new decimal[Lowadj.Length];

            TALib.Core.AdOsc(inHigh: highAdj,
                              inLow: Lowadj,
                              inClose: lastAdj,
                              inVolume: volume,
                              startIdx: 0,
                              endIdx: highAdj.Length - 1,
                              outReal: AdOscFinalData,
                              outBegIdx: out var f,
                              outNbElement: out var e);
            AdOscFinalData = ShiftData(AdOscFinalData, f, e);
            return AdOscFinalData;
        }
        public decimal[] Adx()
        {
            decimal[] AdxFinalData = new decimal[Lowadj.Length];

            TALib.Core.Adx(inHigh: highAdj,
                              inLow: Lowadj,
                              inClose: lastAdj,
                              startIdx: 0,
                              endIdx: highAdj.Length - 1,
                              outReal: AdxFinalData,
                              outBegIdx: out var f,
                              outNbElement: out var e);
            AdxFinalData = ShiftData(AdxFinalData, f, e);
            return AdxFinalData;
        }
        public decimal[] Adxr()
        {
            decimal[] AdxrFinalData = new decimal[Lowadj.Length];
            TALib.Core.Adxr(inHigh: highAdj,
                                                 inLow: Lowadj,
                                                 inClose: lastAdj,
                                                 startIdx: 0,
                                                 endIdx: highAdj.Length - 1,
                                                 outReal: AdxrFinalData,
                                                 outBegIdx: out var f,
                                                 outNbElement: out var e);
            AdxrFinalData = ShiftData(AdxrFinalData, f, e);
            return AdxrFinalData;
        }
        public (decimal[] AROONDown, decimal[] AROONUp) Aroon()
        {
            decimal[] AROONDown = new decimal[Lowadj.Length];
            decimal[] AROONUp = new decimal[Lowadj.Length];
            TALib.Core.Aroon(inHigh: highAdj,
                             inLow: Lowadj,
                             startIdx: 0,
                             endIdx: highAdj.Length - 1,
                             AROONDown,
                             AROONUp,
                             outBegIdx: out var f,
                             outNbElement: out var e);
            AROONDown = ShiftData(AROONDown, f, e);
            AROONUp = ShiftData(AROONUp, f, e);
            return (AROONDown, AROONUp);
        }
        public decimal[] AroonOsc()
        {
            decimal[] AroonOscData = new decimal[Lowadj.Length];
            TALib.Core.AroonOsc(inHigh: highAdj,
                 inLow: Lowadj,
                 startIdx: 0,
                 endIdx: highAdj.Length - 1,
                 outReal: AroonOscData,
                 outBegIdx: out var f,
                 outNbElement: out var e);
            AroonOscData = ShiftData(AroonOscData, f, e);

            return AroonOscData;
        }
        public decimal[] Atr()
        {
            decimal[] ATRData = new decimal[Lowadj.Length];
            TALib.Core.Atr(inHigh: highAdj,
                inLow: Lowadj,
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                outReal: ATRData,
                outBegIdx: out var f,
                outNbElement: out var e);
            ATRData = ShiftData(ATRData, f, e);

            return ATRData;
        }
        public decimal[] AvgPrice()
        {
            decimal[] AvgPriceData = new decimal[Lowadj.Length];
            TALib.Core.AvgPrice(openAdj, inHigh: highAdj,
                inLow: Lowadj,
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                outReal: AvgPriceData,
                outBegIdx: out var f,
                outNbElement: out var e);
            AvgPriceData = ShiftData(AvgPriceData, f, e);

            return AvgPriceData;
        }
        public (decimal[] BBandUpper, decimal[] BBandMiddle, decimal[] BBandLower) Bbands()
        {
            decimal[] BBandUpper = new decimal[Lowadj.Length];
            decimal[] BBandMiddle = new decimal[Lowadj.Length];
            decimal[] BBandLower = new decimal[Lowadj.Length];

            TALib.Core.Bbands(lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                BBandUpper,
                BBandMiddle, BBandLower,
                outBegIdx: out var f,
                outNbElement: out var e);
            BBandUpper = ShiftData(BBandUpper, f, e);
            BBandMiddle = ShiftData(BBandMiddle, f, e);
            BBandLower = ShiftData(BBandLower, f, e);

            return (BBandUpper, BBandMiddle, BBandLower);
        }
        public decimal[] Bop()
        {
            decimal[] BopData = new decimal[Lowadj.Length];
            TALib.Core.Bop(
                    openAdj,
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    BopData,
                    outBegIdx: out var f,
                    outNbElement: out var e);
            BopData = ShiftData(BopData, f, e);

            return BopData;
        }
        public decimal[] Cci()
        {
            decimal[] CciData = new decimal[Lowadj.Length];

            var cciResult = CryptoDataCollector.BussinesExtensions.Helper.Core.Cci(
                      highAdj,
                      Lowadj,
                      lastAdj,
                      startIdx: 0,
                      endIdx: highAdj.Length - 1,
                      CciData,
                      outBegIdx: out var f,
                      outNbElement: out var e,
                      20
                      );
            CciData = ShiftData(CciData, f, e);

            return CciData;
        }
        public int[] Cdl2Crows()
        {
            int[] Cdl2CrowsData = new int[Lowadj.Length];
            TALib.Core.Cdl2Crows(
                    openAdj,
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Cdl2CrowsData,
                    outBegIdx: out var f,
                    outNbElement: out var e);
            Cdl2CrowsData = ShiftData(Cdl2CrowsData, f, e);

            return Cdl2CrowsData;
        }
        public int[] Cdl3BlackCrows()
        {
            int[] Cdl3BlackCrowsData = new int[Lowadj.Length];
            TALib.Core.Cdl3BlackCrows(
                    openAdj,
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Cdl3BlackCrowsData,
                    outBegIdx: out var f,
                    outNbElement: out var e);
            Cdl3BlackCrowsData = ShiftData(Cdl3BlackCrowsData, f, e);

            return Cdl3BlackCrowsData;
        }
        public int[] Cdl3Inside()
        {
            int[] Cdl3InsideData = new int[Lowadj.Length];
            TALib.Core.Cdl3Inside(
                    openAdj,
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Cdl3InsideData,
                    outBegIdx: out var f,
                    outNbElement: out var e);
            Cdl3InsideData = ShiftData(Cdl3InsideData, f, e);

            return Cdl3InsideData;
        }
        public int[] Cdl3LineStrike()
        {
            int[] Cdl3LineStrikeData = new int[Lowadj.Length];
            TALib.Core.Cdl3LineStrike(
                    openAdj,
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Cdl3LineStrikeData,
                    outBegIdx: out var f,
                    outNbElement: out var e);
            Cdl3LineStrikeData = ShiftData(Cdl3LineStrikeData, f, e);

            return Cdl3LineStrikeData;
        }
        public int[] Cdl3Outside()
        {
            int[] Cdl3OutsideData = new int[Lowadj.Length];
            TALib.Core.Cdl3Outside(
                    openAdj,
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Cdl3OutsideData,
                    outBegIdx: out var f,
                    outNbElement: out var e);
            Cdl3OutsideData = ShiftData(Cdl3OutsideData, f, e);

            return Cdl3OutsideData;
        }
        public int[] Cdl3WhiteSoldiers()
        {
            int[] Cdl3WhiteSoldiersData = new int[Lowadj.Length];
            TALib.Core.Cdl3WhiteSoldiers(
                                   openAdj,
                                   highAdj,
                                   Lowadj,
                                   lastAdj,
                                   startIdx: 0,
                                   endIdx: highAdj.Length - 1,
                                   Cdl3WhiteSoldiersData,
                                   outBegIdx: out var f,
                                   outNbElement: out var e);
            return Cdl3WhiteSoldiersData = ShiftData(Cdl3WhiteSoldiersData, f, e);
        }
        public int[] Cdl3StarsInSouth()
        {
            int[] Cdl3StarsInSouthData = new int[Lowadj.Length];
            TALib.Core.Cdl3StarsInSouth(
                    openAdj,
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Cdl3StarsInSouthData,
                    outBegIdx: out var f,
                    outNbElement: out var e);
            Cdl3StarsInSouthData = ShiftData(Cdl3StarsInSouthData, f, e);

            return Cdl3StarsInSouthData;
        }
        public int[] CdlAbandonedBaby()
        {
            int[] CdlAbandonedBabyData = new int[Lowadj.Length];

            TALib.Core.CdlAbandonedBaby(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlAbandonedBabyData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlAbandonedBabyData = ShiftData(CdlAbandonedBabyData, f, e);


            return CdlAbandonedBabyData;
        }
        public int[] CdlDarkCloudCover()
        {
            int[] CdlDarkCloudCoverData = new int[Lowadj.Length];

            TALib.Core.CdlDarkCloudCover(
                                  openAdj,
                                  highAdj,
                                  Lowadj,
                                  lastAdj,
                                  startIdx: 0,
                                  endIdx: highAdj.Length - 1,
                                  CdlDarkCloudCoverData,
                                  outBegIdx: out var f,
                                  outNbElement: out var e);
            return ShiftData(CdlDarkCloudCoverData, f, e);
        }
        public int[] CdlAdvanceBlock()
        {

            int[] CdlAdvanceBlockData = new int[Lowadj.Length];

            TALib.Core.CdlAdvanceBlock(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlAdvanceBlockData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlAdvanceBlockData = ShiftData(CdlAdvanceBlockData, f, e);


            return CdlAdvanceBlockData;
        }
        public int[] CdlBeltHold()
        {

            int[] CdlBeltHoldData = new int[Lowadj.Length];

            TALib.Core.CdlBeltHold(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlBeltHoldData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlBeltHoldData = ShiftData(CdlBeltHoldData, f, e);

            return CdlBeltHoldData;
        }
        public int[] CdlBreakaway()
        {

            int[] CdlBreakawayData = new int[Lowadj.Length];

            TALib.Core.CdlBreakaway(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlBreakawayData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlBreakawayData = ShiftData(CdlBreakawayData, f, e);

            return CdlBreakawayData;
        }
        public int[] CdlClosingMarubozu()
        {

            int[] CdlClosingMarubozuData = new int[Lowadj.Length];

            TALib.Core.CdlClosingMarubozu(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlClosingMarubozuData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlClosingMarubozuData = ShiftData(CdlClosingMarubozuData, f, e);

            return CdlClosingMarubozuData;
        }
        public int[] CdlConcealBabysWall()
        {

            int[] CdlConcealBabysWallData = new int[Lowadj.Length];
            TALib.Core.CdlConcealBabysWall(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlConcealBabysWallData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlConcealBabysWallData = ShiftData(CdlConcealBabysWallData, f, e);

            return CdlConcealBabysWallData;
        }
        public int[] CdlCounterAttack()
        {

            int[] CdlCounterAttackData = new int[Lowadj.Length];
            TALib.Core.CdlCounterAttack(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlCounterAttackData,
                   outBegIdx: out var f,
                   outNbElement: out var e);

            CdlCounterAttackData = ShiftData(CdlCounterAttackData, f, e);

            return CdlCounterAttackData;
        }
        //public int[] CdlCounterAttack()
        //{

        //    int[] CdlDarkCloudCoverData = new int[Lowadj.Length];
        //    TALib.Core.CdlDarkCloudCover(
        //           openAdj,
        //           highAdj,
        //           Lowadj,
        //           lastAdj,
        //           startIdx: 0,
        //           endIdx: highAdj.Length - 1,
        //           CdlDarkCloudCoverData,
        //           outBegIdx: out var f,
        //           outNbElement: out var e);
        //    CdlDarkCloudCoverData = ShiftData(CdlDarkCloudCoverData, f, e);

        //    return CdlDarkCloudCoverData;
        //}
        public int[] CdlDoji()
        {

            int[] CdlDojiData = new int[Lowadj.Length];
            TALib.Core.CdlDoji(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlDojiData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlDojiData = ShiftData(CdlDojiData, f, e);


            return CdlDojiData;
        }
        public int[] CdlDojiStar()
        {

            int[] CdlDojiStarData = new int[Lowadj.Length];
            TALib.Core.CdlDojiStar(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlDojiStarData,
                   outBegIdx: out var f,
                   outNbElement: out var e);

            CdlDojiStarData = ShiftData(CdlDojiStarData, f, e);


            return CdlDojiStarData;
        }
        public int[] CdlDragonflyDoji()
        {

            int[] CdlDragonflyDojiData = new int[Lowadj.Length];
            TALib.Core.CdlDragonflyDoji(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlDragonflyDojiData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlDragonflyDojiData = ShiftData(CdlDragonflyDojiData, f, e);



            return CdlDragonflyDojiData;
        }
        public int[] CdlEngulfing()
        {

            int[] CdlEngulfingData = new int[Lowadj.Length];
            TALib.Core.CdlEngulfing(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlEngulfingData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlEngulfingData = ShiftData(CdlEngulfingData, f, e);



            return CdlEngulfingData;
        }
        public int[] CdlEveningDojiStar()
        {


            int[] CdlEveningDojiStarData = new int[Lowadj.Length];
            TALib.Core.CdlEveningDojiStar(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlEveningDojiStarData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlEveningDojiStarData = ShiftData(CdlEveningDojiStarData, f, e);

            return CdlEveningDojiStarData;
        }
        public int[] CdlEveningStar()
        {


            int[] CdlEveningStarData = new int[Lowadj.Length];
            TALib.Core.CdlEveningStar(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlEveningStarData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlEveningStarData = ShiftData(CdlEveningStarData, f, e);


            return CdlEveningStarData;
        }
        public int[] CdlGapSideSideWhite()
        {


            int[] CdlGapSideSideWhiteData = new int[Lowadj.Length];
            TALib.Core.CdlGapSideSideWhite(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlGapSideSideWhiteData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlGapSideSideWhiteData = ShiftData(CdlGapSideSideWhiteData, f, e);

            return CdlGapSideSideWhiteData;
        }
        public int[] CdlGravestoneDoji()
        {
            int[] CdlGravestoneDojiData = new int[Lowadj.Length];
            TALib.Core.CdlGravestoneDoji(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlGravestoneDojiData,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlGravestoneDojiData = ShiftData(CdlGravestoneDojiData, f, e);

            return CdlGravestoneDojiData;
        }
        public int[] CdlHammer()
        {

           // public static RetCode CdlHammer(decimal[] inOpen, decimal[] inHigh, decimal[] inLow, decimal[] inClose, int startIdx, int endIdx, int[] outInteger, out int outBegIdx, out int outNbElement)
            int[] CdlHammer = new int[Lowadj.Length];
            TALib.Core.CdlHammer(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlHammer,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlHammer = ShiftData(CdlHammer, f, e);

            return CdlHammer;
        }
        public int[] CdlHangingMan()
        {
            int[] CdlHangingMan = new int[Lowadj.Length];
            TALib.Core.CdlHangingMan(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlHangingMan,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlHangingMan = ShiftData(CdlHangingMan, f, e);


            return CdlHangingMan;
        }
        public int[] CdlHarami()
        {

            int[] CdlHarami = new int[Lowadj.Length];
            TALib.Core.CdlHarami(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlHarami,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlHarami = ShiftData(CdlHarami, f, e);

            return CdlHarami;
        }
        public int[] CdlHaramiCross()
        {

            int[] CdlHaramiCross = new int[Lowadj.Length];
            TALib.Core.CdlHaramiCross(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlHaramiCross,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlHaramiCross = ShiftData(CdlHaramiCross, f, e);

            return CdlHaramiCross;
        }
        public int[] CdlHighWave()
        {
            int[] CdlHighWave = new int[Lowadj.Length];
            TALib.Core.CdlHighWave(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlHighWave,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlHighWave = ShiftData(CdlHighWave, f, e);

            return CdlHighWave;
        }
        public int[] CdlHikkake()
        {
            int[] CdlHikkake = new int[Lowadj.Length];
            TALib.Core.CdlHikkake(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlHikkake,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlHikkake = ShiftData(CdlHikkake, f, e);

            return CdlHikkake;
        }
        public int[] CdlHomingPigeon()
        {
            int[] CdlHomingPigeon = new int[Lowadj.Length];
            TALib.Core.CdlHomingPigeon(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlHomingPigeon,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlHomingPigeon = ShiftData(CdlHomingPigeon, f, e);

            return CdlHomingPigeon;
        }
        public int[] CdlIdentical3Crows()
        {
            int[] CdlIdentical3Crows = new int[Lowadj.Length];
            TALib.Core.CdlIdentical3Crows(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlIdentical3Crows,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlIdentical3Crows = ShiftData(CdlIdentical3Crows, f, e);

            return CdlIdentical3Crows;
        }
        public int[] CdlInNeck()
        {

            int[] CdlInNeck = new int[Lowadj.Length];
            TALib.Core.CdlInNeck(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlInNeck,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlInNeck = ShiftData(CdlInNeck, f, e);

            return CdlInNeck;
        }
        public int[] CdlInvertedHammer()
        {


            int[] CdlInvertedHammer = new int[Lowadj.Length];
            TALib.Core.CdlInvertedHammer(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlInvertedHammer,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlInvertedHammer = ShiftData(CdlInvertedHammer, f, e);

            return CdlInvertedHammer;
        }
        public int[] CdlKicking()
        {

            int[] CdlKicking = new int[Lowadj.Length];
            TALib.Core.CdlKicking(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlKicking,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlKicking = ShiftData(CdlKicking, f, e);

            return CdlKicking;
        }
        public int[] CdlKickingByLength()
        {

            int[] CdlKickingByLength = new int[Lowadj.Length];
            TALib.Core.CdlKickingByLength(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlKickingByLength,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlKickingByLength = ShiftData(CdlKickingByLength, f, e);


            return CdlKickingByLength;
        }
        public int[] CdlLadderBottom()
        {
            int[] CdlLadderBottom = new int[Lowadj.Length];
            TALib.Core.CdlLadderBottom(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlLadderBottom,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlLadderBottom = ShiftData(CdlLadderBottom, f, e);


            return CdlLadderBottom;
        }
        public int[] CdlLongLeggedDoji()
        {

            int[] CdlLongLeggedDoji = new int[Lowadj.Length];
            TALib.Core.CdlLongLeggedDoji(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlLongLeggedDoji,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlLongLeggedDoji = ShiftData(CdlLongLeggedDoji, f, e);

            return CdlLongLeggedDoji;
        }
        public int[] CdlLongLine()
        {

            int[] CdlLongLine = new int[Lowadj.Length];
            TALib.Core.CdlLongLine(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlLongLine,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlLongLine = ShiftData(CdlLongLine, f, e);

            return CdlLongLine;
        }
        public int[] CdlMarubozu()
        {

            int[] CdlMarubozu = new int[Lowadj.Length];
            TALib.Core.CdlMarubozu(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlMarubozu,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlMarubozu = ShiftData(CdlMarubozu, f, e);


            return CdlMarubozu;
        }
        public int[] CdlMatchingLow()
        {


            int[] CdlMatchingLow = new int[Lowadj.Length];
            TALib.Core.CdlMatchingLow(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlMatchingLow,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlMatchingLow = ShiftData(CdlMatchingLow, f, e);

            return CdlMatchingLow;
        }
        public int[] CdlMatHold()
        {

            int[] CdlMatHold = new int[Lowadj.Length];
            TALib.Core.CdlMatHold(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlMatHold,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlMatHold = ShiftData(CdlMatHold, f, e);


            return CdlMatHold;
        }
        public int[] CdlMorningDojiStar()
        {

            int[] CdlMorningDojiStar = new int[Lowadj.Length];
            TALib.Core.CdlMorningDojiStar(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlMorningDojiStar,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlMorningDojiStar = ShiftData(CdlMorningDojiStar, f, e);


            return CdlMorningDojiStar;
        }
        public int[] CdlMorningStar()
        {

            int[] CdlMorningStar = new int[Lowadj.Length];
            TALib.Core.CdlMorningStar(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlMorningStar,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlMorningStar = ShiftData(CdlMorningStar, f, e);

            return CdlMorningStar;
        }
        public int[] CdlOnNeck()
        {


            int[] CdlOnNeck = new int[Lowadj.Length];
            TALib.Core.CdlOnNeck(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlOnNeck,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlOnNeck = ShiftData(CdlOnNeck, f, e);

            return CdlOnNeck;
        }
        public int[] CdlPiercing()
        {

            int[] CdlPiercing = new int[Lowadj.Length];
            TALib.Core.CdlPiercing(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlPiercing,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlPiercing = ShiftData(CdlPiercing, f, e);


            return CdlPiercing;
        }
        public int[] CdlRickshawMan()
        {


            int[] CdlRickshawMan = new int[Lowadj.Length];
            TALib.Core.CdlRickshawMan(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlRickshawMan,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlRickshawMan = ShiftData(CdlRickshawMan, f, e);

            return CdlRickshawMan;
        }
        public int[] CdlRiseFall3Methods()
        {

            int[] CdlRiseFall3Methods = new int[Lowadj.Length];
            TALib.Core.CdlRiseFall3Methods(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlRiseFall3Methods,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlRiseFall3Methods = ShiftData(CdlRiseFall3Methods, f, e);

            return CdlRiseFall3Methods;
        }
        public int[] CdlSeparatingLines()
        {

            int[] CdlSeparatingLines = new int[Lowadj.Length];
            TALib.Core.CdlSeparatingLines(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlSeparatingLines,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlSeparatingLines = ShiftData(CdlSeparatingLines, f, e);


            return CdlSeparatingLines;
        }
        public int[] CdlShootingStar()
        {
            int[] CdlShootingStar = new int[Lowadj.Length];
            TALib.Core.CdlShootingStar(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlShootingStar,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlShootingStar = ShiftData(CdlShootingStar, f, e);

            return CdlShootingStar;
        }
        public int[] CdlShortLine()
        {

            int[] CdlShortLine = new int[Lowadj.Length];
            TALib.Core.CdlShortLine(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlShortLine,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlShortLine = ShiftData(CdlShortLine, f, e);


            return CdlShortLine;
        }
        public int[] CdlSpinningTop()
        {

            int[] CdlSpinningTop = new int[Lowadj.Length];
            TALib.Core.CdlSpinningTop(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlSpinningTop,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlSpinningTop = ShiftData(CdlSpinningTop, f, e);

            return CdlSpinningTop;
        }
        public int[] CdlStalledPattern()
        {


            int[] CdlStalledPattern = new int[Lowadj.Length];
            TALib.Core.CdlStalledPattern(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlStalledPattern,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlStalledPattern = ShiftData(CdlStalledPattern, f, e);

            return CdlStalledPattern;
        }
        public int[] CdlStickSandwich()
        {


            int[] CdlStickSandwich = new int[Lowadj.Length];
            TALib.Core.CdlStickSandwich(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlStickSandwich,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlStickSandwich = ShiftData(CdlStickSandwich, f, e);


            return CdlStickSandwich;
        }
        public int[] CdlTakuri()
        {

            int[] CdlTakuri = new int[Lowadj.Length];
            TALib.Core.CdlTakuri(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlTakuri,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlTakuri = ShiftData(CdlTakuri, f, e);


            return CdlTakuri;
        }
        public int[] CdlTasukiGap()
        {

            int[] CdlTasukiGap = new int[Lowadj.Length];
            TALib.Core.CdlTasukiGap(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlTasukiGap,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlTasukiGap = ShiftData(CdlTasukiGap, f, e);

            return CdlTasukiGap;
        }
        public int[] CdlThrusting()
        {
            int[] CdlThrusting = new int[Lowadj.Length];
            TALib.Core.CdlThrusting(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlThrusting,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlThrusting = ShiftData(CdlThrusting, f, e);

            return CdlThrusting;
        }
        public int[] CdlTristar()
        {

            int[] CdlTristar = new int[Lowadj.Length];
            TALib.Core.CdlTristar(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlTristar,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlTristar = ShiftData(CdlTristar, f, e);

            return CdlTristar;
        }
        public int[] CdlUnique3River()
        {
            int[] CdlUnique3River = new int[Lowadj.Length];
            TALib.Core.CdlUnique3River(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlUnique3River,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlUnique3River = ShiftData(CdlUnique3River, f, e);

            return CdlUnique3River;
        }
        public int[] CdlUpsideGap2Crows()
        {

            int[] CdlUpsideGap2Crows = new int[Lowadj.Length];
            TALib.Core.CdlUpsideGap2Crows(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlUpsideGap2Crows,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlUpsideGap2Crows = ShiftData(CdlUpsideGap2Crows, f, e);

            return CdlUpsideGap2Crows;
        }
        public int[] CdlXSideGap3Methods()
        {

            int[] CdlXSideGap3Methods = new int[Lowadj.Length];
            TALib.Core.CdlXSideGap3Methods(
                   openAdj,
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   CdlXSideGap3Methods,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            CdlXSideGap3Methods = ShiftData(CdlXSideGap3Methods, f, e);

            return CdlXSideGap3Methods;
        }
        public decimal[] Cmo()
        {

            decimal[] Cmo = new decimal[Lowadj.Length];
            TALib.Core.Cmo(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Cmo,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            Cmo = ShiftData(Cmo, f, e);


            return Cmo;
        }
        public decimal[] Dema()
        {
            decimal[] Dema = new decimal[Lowadj.Length];
            TALib.Core.Dema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Dema,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            Dema = ShiftData(Dema, f, e);

            return Dema;
        }
        public decimal[] Dx()
        {
            decimal[] DX = new decimal[Lowadj.Length];
            TALib.Core.Dx(
                   highAdj,
                   Lowadj,
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   DX,
                   outBegIdx: out var f,
                   outNbElement: out var e);
            DX = ShiftData(DX, f, e);


            return DX;
        }
        public decimal[] Ema5()
        {
            decimal[] Ema5 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema5,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   5
                   );
            Ema5 = ShiftData(Ema5, f, e);


            return Ema5;
        }
        public decimal[] Ema9()
        {
            decimal[] Ema9 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema9,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   9
                   );
            Ema9 = ShiftData(Ema9, f, e);


            return Ema9;
        }
        public decimal[] Ema10()
        {
            decimal[] Ema10 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema10,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   10
                   );
            Ema10 = ShiftData(Ema10, f, e);

            return Ema10;
        }
        public decimal[] Ema12()
        {

            decimal[] Ema12 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema12,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   12
                   );
            Ema12 = ShiftData(Ema12, f, e);


            return Ema12;
        }
        public decimal[] Ema13()

        {

            decimal[] Ema13 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema13,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   13
                   );
            Ema13 = ShiftData(Ema13, f, e);

            return Ema13;
        }
        public decimal[] Ema20()

        {

            decimal[] Ema20 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema20,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   20
                   );
            Ema20 = ShiftData(Ema20, f, e);

            return Ema20;
        }
        public decimal[] Ema30()
        {

            decimal[] Ema30 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema30,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   30
                   );
            Ema30 = ShiftData(Ema30, f, e);

            return Ema30;
        }
        public decimal[] Ema26()
        {
            decimal[] Ema26 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema26,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   26
                   );
            Ema26 = ShiftData(Ema26, f, e);

            return Ema26;
        }
        public decimal[] Ema50()
        {
            decimal[] Ema50 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema50,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   50
                   );
            Ema50 = ShiftData(Ema50, f, e);

            return Ema50;
        }
        public decimal[] Ema100()
        {

            decimal[] Ema100 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Ema100,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   100
                   );
            Ema100 = ShiftData(Ema100, f, e);


            return Ema100;
        }
        public decimal[] Ema200()
        {
            decimal[] Ema200 = new decimal[Lowadj.Length];
            TALib.Core.Ema(
                  lastAdj,
                  startIdx: 0,
                  endIdx: highAdj.Length - 1,
                  Ema200,
                  outBegIdx: out var f,
                  outNbElement: out var e,
                  200
                  );
            Ema200 = ShiftData(Ema200, f, e);

            return Ema200;
        }
        //public decimal[] Ema200()
        //{
        //    decimal[] Ema200 = new decimal[lastAdj.Length];
        //    TALib.Core.Ema(
        //          lastAdj,
        //          startIdx: 0,
        //          endIdx: lastAdj.Length - 1,
        //          Ema200,
        //          outBegIdx: out var f,
        //          outNbElement: out var e,
        //          200
        //          );
        //    Ema200 = ShiftData(Ema200, f, e);

        //    return Ema200;
        //}
        public decimal[] Kama()
        {

            decimal[] Kama = new decimal[Lowadj.Length];
            TALib.Core.Kama(
                  lastAdj,
                  startIdx: 0,
                  endIdx: highAdj.Length - 1,
                  Kama,
                  outBegIdx: out var f,
                  outNbElement: out var e,
                  200
                  );
            Kama = ShiftData(Kama, f, e);

            return Kama;
        }
        public (decimal[] Macd, decimal[] MacdSignal, decimal[] MacdHist) Macd()
        {
            decimal[] Macd = new decimal[Lowadj.Length];
            decimal[] MacdSignal = new decimal[Lowadj.Length];
            decimal[] MacdHist = new decimal[Lowadj.Length];
            TALib.Core.Macd(
                  lastAdj,
                  startIdx: 0,
                  endIdx: highAdj.Length - 1,
                  Macd,
                  MacdSignal,
                  MacdHist,
                  outBegIdx: out var f,
                  outNbElement: out var e
                  );
            Macd = ShiftData(Macd, f, e);
            MacdSignal = ShiftData(MacdSignal, f, e);
            MacdHist = ShiftData(MacdHist, f, e);

            return (Macd, MacdSignal, MacdHist);
        }
        public (decimal[] MacdExt, decimal[] MacdExtSignal, decimal[] MacdExtHist) MacdExt()
        {
            decimal[] MacdExt = new decimal[Lowadj.Length];
            decimal[] MacdExtSignal = new decimal[Lowadj.Length];
            decimal[] MacdExtHist = new decimal[Lowadj.Length];

            TALib.Core.MacdExt(
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                MacdExt,
                MacdExtSignal,
                MacdExtHist,
                outBegIdx: out var f,
                outNbElement: out var e
                );
            MacdExt = ShiftData(MacdExt, f, e);
            MacdExtSignal = ShiftData(MacdExtSignal, f, e);
            MacdExtHist = ShiftData(MacdExtHist, f, e);

            return (MacdExt, MacdExtSignal, MacdExtHist);
        }
        public (decimal[] MacdFix, decimal[] MacdFixSignal, decimal[] MacdFixHist) MacdFix()
        {
            decimal[] MacdFix = new decimal[Lowadj.Length];
            decimal[] MacdFixSignal = new decimal[Lowadj.Length];
            decimal[] MacdFixHist = new decimal[Lowadj.Length];

            TALib.Core.MacdFix(
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                MacdFix,
                MacdFixSignal,
                MacdFixHist,
                outBegIdx: out var f,
                outNbElement: out var e
                );
            MacdFix = ShiftData(MacdFix, f, e);
            MacdFixSignal = ShiftData(MacdFixSignal, f, e);
            MacdFixHist = ShiftData(MacdFixHist, f, e);

            return (MacdFix, MacdFixSignal, MacdFixHist);
        }
        public (decimal[] Mama, decimal[] Fama) Mama()
        {

            decimal[] Mama = new decimal[Lowadj.Length];
            decimal[] Fama = new decimal[Lowadj.Length];
            TALib.Core.Mama(
    lastAdj,
    startIdx: 0,
    endIdx: highAdj.Length - 1,
    Mama,
    Fama,
    outBegIdx: out var f,
    outNbElement: out var e
    );
            Mama = ShiftData(Mama, f, e);
            Fama = ShiftData(Fama, f, e);

            return (Mama, Fama);
        }
        public decimal[] Max()
        {
            decimal[] Max = new decimal[Lowadj.Length];

            TALib.Core.Max(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Max,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            Max = ShiftData(Max, f, e);

            return Max;
        }
        public int[] MaxIndex()
        {

            int[] MaxIndex = new int[Lowadj.Length];

            TALib.Core.MaxIndex(
                   inReal: lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   outInteger: MaxIndex,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            MaxIndex = ShiftData(MaxIndex, f, e);

            return MaxIndex;
        }
        public decimal[] MedPrice()
        {
            decimal[] MedPrice = new decimal[Lowadj.Length];

            TALib.Core.MedPrice(
                    inHigh: highAdj,
                    inLow: Lowadj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   outReal: MedPrice,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            MedPrice = ShiftData(MedPrice, f, e);

            return MedPrice;
        }
        public decimal[] Mfi()
        {

            decimal[] Mfi = new decimal[Lowadj.Length];

            TALib.Core.Mfi(
                    inHigh: highAdj,
                    inLow: Lowadj,
                    lastAdj,
                    volume,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   outReal: Mfi,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            Mfi = ShiftData(Mfi, f, e);

            return Mfi;
        }
        public decimal[] MidPoint()
        {
            decimal[] MidPoint = new decimal[Lowadj.Length];

            TALib.Core.MidPoint(
                    lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   outReal: MidPoint,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            MidPoint = ShiftData(MidPoint, f, e);


            return MidPoint;
        }
        public decimal[] MidPrice()
        {
            decimal[] MidPrice = new decimal[Lowadj.Length];

            TALib.Core.MidPrice(
                    highAdj,
                    Lowadj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   outReal: MidPrice,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            MidPrice = ShiftData(MidPrice, f, e);

            return MidPrice;
        }
        public decimal[] Min()
        {
            decimal[] Min = new decimal[Lowadj.Length];

            TALib.Core.Min(
                    lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   outReal: Min,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            Min = ShiftData(Min, f, e);

            return Min;
        }
        public int[] MinIndex()
        {
            int[] MinIndex = new int[Lowadj.Length];

            TALib.Core.MinIndex(
                    lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                    MinIndex,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            MinIndex = ShiftData(MinIndex, f, e);

            return MinIndex;
        }
        public decimal[] MinusDI()
        {
            decimal[] MinusDI = new decimal[Lowadj.Length];
            TALib.Core.MinusDI(
                    highAdj,
                    Lowadj,
                    lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                    MinusDI,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            MinusDI = ShiftData(MinusDI, f, e);


            return MinusDI;
        }
        public decimal[] MinusDM()
        {
            decimal[] MinusDM = new decimal[Lowadj.Length];
            TALib.Core.MinusDM(
                    highAdj,
                    Lowadj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                    MinusDM,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            MinusDM = ShiftData(MinusDM, f, e);

            return MinusDM;
        }
        public decimal[] Mom()
        {
            decimal[] Mom = new decimal[Lowadj.Length];
            TALib.Core.Mom(
                    lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                    Mom,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );

            Mom = ShiftData(Mom, f, e);

            return Mom;
        }
        public decimal[] Natr()
        {
            decimal[] Natr = new decimal[Lowadj.Length];
            TALib.Core.Natr(
                    highAdj,
                    Lowadj,
                    lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                    Natr,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            Natr = ShiftData(Natr, f, e);

            return Natr;
        }
        public decimal[] Obv()
        {
            decimal[] Obv = new decimal[Lowadj.Length];
            TALib.Core.Obv(
                    lastAdj,
                    volume,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                    Obv,
                   outBegIdx: out var f,
                   outNbElement: out var e
                   );
            Obv = ShiftData(Obv, f, e);


            return Obv;
        }
        public decimal[] PlusDI()
        {
            decimal[] PlusDI = new decimal[Lowadj.Length];

            TALib.Core.PlusDI(
                    highAdj,
                    Lowadj,
                 lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                 PlusDI,
                outBegIdx: out var f,
                outNbElement: out var e
                );
            PlusDI = ShiftData(PlusDI, f, e);

            return PlusDI;
        }
        public decimal[] PlusDM()
        {
            decimal[] PlusDM = new decimal[Lowadj.Length];

            TALib.Core.PlusDM(
                    highAdj,
                    Lowadj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                 PlusDM,
                outBegIdx: out var f,
                outNbElement: out var e
                );
            PlusDM = ShiftData(PlusDM, f, e);


            return PlusDM;
        }
        public decimal[] Ppo()
        {

            decimal[] Ppo = new decimal[Lowadj.Length];

            TALib.Core.Ppo(
                    lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                 Ppo,
                outBegIdx: out var f,
                outNbElement: out var e
                );
            Ppo = ShiftData(Ppo, f, e);

            return Ppo;
        }
        public decimal[] Roc()
        {
            decimal[] Roc = new decimal[Lowadj.Length];

            TALib.Core.Roc(
                    lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                 Roc,
                outBegIdx: out var f,
                outNbElement: out var e
                );
            Roc = ShiftData(Roc, f, e);

            return Roc;
        }
        public decimal[] RocP()
        {
            decimal[] RocP = new decimal[Lowadj.Length];

            TALib.Core.RocP(
                    lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                 RocP,
                outBegIdx: out var f,
                outNbElement: out var e
                );
            RocP = ShiftData(RocP, f, e);

            return RocP;
        }
        public decimal[] RocR()
        {
            decimal[] RocR = new decimal[Lowadj.Length];

            TALib.Core.RocR(
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                RocR,
                outBegIdx: out var f,
                outNbElement: out var e
            );

            RocR = ShiftData(RocR, f, e);

            return RocR;
        }
        public decimal[] RocR100()
        {
            decimal[] RocR100 = new decimal[Lowadj.Length];

            TALib.Core.RocR100(
                   lastAdj,
               startIdx: 0,
               endIdx: highAdj.Length - 1,
                RocR100,
               outBegIdx: out var f,
               outNbElement: out var e
               );
            RocR100 = ShiftData(RocR100, f, e);

            return RocR100;
        }
        public decimal[] Rsi()
        {
            decimal[] RSI = new decimal[Lowadj.Length];

            TALib.Core.Rsi(
                   lastAdj,
               startIdx: 0,
               endIdx: highAdj.Length - 1,
                RSI,
               outBegIdx: out var f,
               outNbElement: out var e
               );
            RSI = ShiftData(RSI, f, e);

            return RSI;
        }
        public decimal[] SarExt()
        {
            decimal[] SarExt = new decimal[Lowadj.Length];

            TALib.Core.SarExt(
                   highAdj,
                   Lowadj,
               startIdx: 0,
               endIdx: highAdj.Length - 1,
                SarExt,
               outBegIdx: out var f,
               outNbElement: out var e
               );
            SarExt = ShiftData(SarExt, f, e);

            return SarExt;
        }
        public decimal[] Sma5()
        {
            decimal[] Sma5 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                Sma5,
                outBegIdx: out var f,
                outNbElement: out var e,
                5
            );

            Sma5 = ShiftData(Sma5, f, e);

            return Sma5;
        }
        public decimal[] Sma9()
        {
            decimal[] Sma9 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                Sma9,
                outBegIdx: out var f,
                outNbElement: out var e,
                9
            );

            Sma9 = ShiftData(Sma9, f, e);

            return Sma9;
        }
        public decimal[] Sma10()
        {
            decimal[] Sma10 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                Sma10,
                outBegIdx: out var f,
                outNbElement: out var e,
                10
            );
            Sma10 = ShiftData(Sma10, f, e);

            return Sma10;
        }
        public decimal[] Sma12()
        {
            decimal[] Sma12 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Sma12,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   12
                   );
            Sma12 = ShiftData(Sma12, f, e);

            return Sma12;
        }
        public decimal[] Sma13()
        {
            decimal[] Sma13 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Sma13,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   13
                   );
            Sma13 = ShiftData(Sma13, f, e);

            return Sma13;
        }
        public decimal[] Sma20()
        {
            decimal[] Sma20 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Sma20,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   20
                   );
            Sma20 = ShiftData(Sma20, f, e);

            return Sma20;
        }
        public decimal[] Sma30()
        {
            decimal[] Sma30 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Sma30,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   30
                   );
            Sma30 = ShiftData(Sma30, f, e);

            return Sma30;
        }
        public decimal[] Sma26()
        {
            decimal[] Sma26 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Sma26,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   26
                   );
            Sma26 = ShiftData(Sma26, f, e);

            return Sma26;
        }
        public decimal[] Sma50()
        {
            decimal[] Sma50 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                   Sma50,
                   outBegIdx: out var f,
                   outNbElement: out var e,
                   50
                   );
            Sma50 = ShiftData(Sma50, f, e);

            return Sma50;
        }
        public decimal[] Sma100()
        {
            decimal[] Sma100 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                   lastAdj,
                   startIdx: 0,
                   endIdx: highAdj.Length - 1,
                  Sma100,
                   outBegIdx: out var
                   f,
                   outNbElement: out var e,
                   100
                   );
            Sma100 = ShiftData(Sma100, f, e);

            return Sma100;
        }
        public decimal[] Sma200()
        {
            decimal[] Sma200 = new decimal[Lowadj.Length];
            TALib.Core.Sma(
                  lastAdj,
                  startIdx: 0,
                  endIdx: highAdj.Length - 1,
                  Sma200,
                  outBegIdx: out var f,
                  outNbElement: out var e,
                  200
                  );
            Sma200 = ShiftData(Sma200, f, e);

            return Sma200;
        }
        public decimal[] StdDev()
        {
            decimal[] StdDev = new decimal[Lowadj.Length];
            TALib.Core.StdDev(
                  lastAdj,
                  startIdx: 0,
                  endIdx: highAdj.Length - 1,
                  StdDev,
                  outBegIdx: out var f,
                  outNbElement: out var e
                  );
            StdDev = ShiftData(StdDev, f, e);

            return StdDev;
        }
        public (decimal[] StochSlow, decimal[] StochSlowD) Stoch()
        {
            decimal[] StochSlow = new decimal[Lowadj.Length];
            decimal[] StochSlowD = new decimal[Lowadj.Length];
            TALib.Core.Stoch(
                  highAdj,
                  Lowadj,
                  lastAdj,
                  startIdx: 0,
                  endIdx: highAdj.Length - 1,
                  StochSlow,
                  StochSlowD,
                  outBegIdx: out var f,
                  outNbElement: out var e
                  );
            StochSlow = ShiftData(StochSlow, f, e);
            StochSlowD = ShiftData(StochSlowD, f, e);

            return (StochSlow, StochSlowD);
        }
        public (decimal[] StochFK, decimal[] StochFD) StochF()
        {
            decimal[] StochFK = new decimal[Lowadj.Length];
            decimal[] StochFD = new decimal[Lowadj.Length];
            TALib.Core.StochF(
                highAdj,
                Lowadj,
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                StochFK,
                StochFD,
                outBegIdx: out var f,
                outNbElement: out var e
            );
            StochFK = ShiftData(StochFK, f, e);
            StochFD = ShiftData(StochFD, f, e);

            return (StochFK, StochFD);
        }
        public (decimal[] StochRsiFK, decimal[] StochRsiFD) StochRsi()
        {
            decimal[] StochRsiFK = new decimal[Lowadj.Length];
            decimal[] StochRsiFD = new decimal[Lowadj.Length];
            TALib.Core.StochRsi(
                lastAdj,
                startIdx: 0,
                endIdx: highAdj.Length - 1,
                StochRsiFK,
                StochRsiFD,
                outBegIdx: out var f,
                outNbElement: out var e
            );
            StochRsiFK = ShiftData(StochRsiFK, f, e);
            StochRsiFD = ShiftData(StochRsiFD, f, e);

            return (StochRsiFK, StochRsiFD);
        }
        public decimal[] Sum()
        {
            decimal[] Sum = new decimal[Lowadj.Length];
            TALib.Core.Sum(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Sum,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            Sum = ShiftData(Sum, f, e);

            return Sum;
        }
        public decimal[] T3()
        {
            decimal[] T3 = new decimal[Lowadj.Length];
            TALib.Core.T3(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    T3,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            return ShiftData(T3, f, e);
        }
        public decimal[] Tema()
        {
            decimal[] Tema = new decimal[Lowadj.Length];
            TALib.Core.Tema(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Tema,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            Tema = ShiftData(Tema, f, e);

            return Tema;
        }
        public decimal[] TRange()
        {

            decimal[] TRange = new decimal[Lowadj.Length];
            TALib.Core.TRange(
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    TRange,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            TRange = ShiftData(TRange, f, e);

            return TRange;
        }
        public decimal[] Trima()
        {
            decimal[] Trima = new decimal[Lowadj.Length];
            TALib.Core.Trima(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Trima,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            Trima = ShiftData(Trima, f, e);

            return Trima;
        }
        public decimal[] Trix()
        {
            decimal[] Trix = new decimal[Lowadj.Length];
            TALib.Core.Trix(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Trix,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            Trix = ShiftData(Trix, f, e);

            return Trix;
        }
        public decimal[] Tsf()
        {
            decimal[] Tsf = new decimal[Lowadj.Length];
            TALib.Core.Tsf(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Tsf,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            Tsf = ShiftData(Tsf, f, e);

            return Tsf;
        }
        public decimal[] TypPrice()
        {
            decimal[] TypPrice = new decimal[Lowadj.Length];
            TALib.Core.TypPrice(
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    TypPrice,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            TypPrice = ShiftData(TypPrice, f, e);

            return TypPrice;
        }
        public decimal[] UltOsc()
        {
            decimal[] UltOsc = new decimal[Lowadj.Length];
            TALib.Core.UltOsc(
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    UltOsc,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            UltOsc = ShiftData(UltOsc, f, e);

            return UltOsc;
        }
        public decimal[] Var()
        {
            decimal[] Var = new decimal[Lowadj.Length];
            TALib.Core.Var(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Var,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            Var = ShiftData(Var, f, e);

            return Var;
        }
        public decimal[] WclPrice()
        {
            decimal[] WclPrice = new decimal[Lowadj.Length];
            TALib.Core.WclPrice(
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    WclPrice,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            WclPrice = ShiftData(WclPrice, f, e);

            return WclPrice;
        }
        public decimal[] WillR()
        {
            decimal[] WillR = new decimal[Lowadj.Length];
            TALib.Core.WillR(
                    highAdj,
                    Lowadj,
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    WillR,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );
            WillR = ShiftData(WillR, f, e);


            return WillR;
        }
        public decimal[] Wma()
        {
            decimal[] Wma = new decimal[Lowadj.Length];
            TALib.Core.Wma(
                    lastAdj,
                    startIdx: 0,
                    endIdx: highAdj.Length - 1,
                    Wma,
                    outBegIdx: out var f,
                    outNbElement: out var e
                );

            Wma = ShiftData(Wma, f, e);
            return Wma;
        }
        public System.Collections.Generic.IEnumerable<KeltnerResult> KeltnerChannel()
        {
            try
            {
                return Quotes.GetKeltner();

            }
            catch (System.Exception)
            {
                return Enumerable.Range(0, Lowadj.Length).Select(x => new KeltnerResult(System.DateTime.Now));
            }
        }
        public System.Collections.Generic.IEnumerable<CmfResult> ChaikinMoneyFlow()
        {
            try
            {
                return Quotes.GetCmf();

            }
            catch (System.Exception)
            {

                return Enumerable.Range(0, Lowadj.Length).Select(x => new CmfResult(System.DateTime.Now));

            }
        }
        public System.Collections.Generic.IEnumerable<DonchianResult> Donchian()
        {
            try
            {
                return Quotes.GetDonchian();

            }
            catch (System.Exception)
            {

                return Enumerable.Range(0, Lowadj.Length).Select(x => new DonchianResult(System.DateTime.Now));
            }
        }
        public System.Collections.Generic.IEnumerable<HmaResult> HMA()
        {
            try
            {
                return Quotes.GetHma(9);

            }
            catch (System.Exception)
            {

                return Enumerable.Range(0, Lowadj.Length).Select(x => new HmaResult(System.DateTime.Now));
            }
        }
        public System.Collections.Generic.IEnumerable<AwesomeResult> AwesomeOscillator()
        {
            try
            {
                return Quotes.GetAwesome();

            }
            catch (System.Exception)
            {

                return Enumerable.Range(0, Lowadj.Length).Select(x => new AwesomeResult(System.DateTime.Now));
            }
        }
        public System.Collections.Generic.IEnumerable<UltimateResult> UltimateOscillator()
        {
            try
            {
                return Quotes.GetUltimate();

            }
            catch (System.Exception)
            {

                return Enumerable.Range(0, Lowadj.Length).Select(x => new UltimateResult(System.DateTime.Now));
            }
        }
        public System.Collections.Generic.IEnumerable<VwapResult> Vwap()
        {
            try
            {
                return Quotes.GetVwap();

            }
            catch (System.Exception)
            {

                return Enumerable.Range(0, Lowadj.Length).Select(x => new VwapResult(System.DateTime.Now));
            }
        }
        public System.Collections.Generic.IEnumerable<IchimokuResult> Ichimoku()
        {
            try
            {
                return Quotes.GetIchimoku();

            }
            catch (System.Exception)
            {

                return Enumerable.Range(0, Lowadj.Length).Select(x => new IchimokuResult(System.DateTime.Now));
            }
        }
        private decimal[] ShiftData(decimal[] oldData, int f, int e)
        {
            var newData = new decimal[oldData.Length];
            for (var i = 0; i < e; i++)
            {
                newData[f + i] = oldData[i];
            }
            return newData;
        }


        private int[] ShiftData(int[] oldData, int f, int e)
        {
            var newData = new int[oldData.Length];
            for (var i = 0; i < e; i++)
            {
                newData[f + i] = oldData[i];
            }
            return newData;
        }
    }
}
