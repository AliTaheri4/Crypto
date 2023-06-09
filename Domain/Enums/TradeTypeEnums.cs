using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector.Enums
{
    public enum TradeType
    {
        Short = 1, //sell

        Long = 2, //buy
        Unkouwn = 3
    }
    public enum SignalCheckerType
    {
        Buy = 1,
        Sell = 2,
        NotHappendInBuyChecker = 3,
        NotHappendInSellChecker = 4,
        ListEmpty = 5,
        HaveBefore = 6,
        SellRecently = 6,
    }
    public enum ChartProcessType
    {
        Ascending = 1,
        Descending = 2,
        Flat = 3
    }

    public enum CandleType
    {
        Green = 1,
        Red = 2,
        Gray = 3
    }
    public enum FutureSlopeType
    {
        Ascending = 1,
        Descending = 2
    }

    public enum SignalType
    {
        SmaCross = 1,
        SmaDistance = 2,
        SmaTouch=3,
        CciCrossLines=4,
        Fsp=5,
        DoubleEmaMacd=6,
        DivergenceCandleStickSr=7,
        LastTwoBigCandles=8,
        ByLuck=9
    }
    public enum PriceToMovingAverageType
    {
        Over = 1,
        Under = 2
    }
    public enum GeneralStaus
    {
        Null = 0,
        Hold = 1,
        Sell = 2,
        Empty = 3,

    }
    public enum TradeResultType
    {
        Holding = 1,
        BecameProfit = 2,
        BecameLoss = 3,
        Conflict = 4,
        Pending = 10,
        ForceStop=11
    }
    public enum Symbol
    {
        BNB = 1,
        ADA = 2,
        ATOM = 3,
        XRP = 4,
        SOL = 5,
        ETH = 6,
        BTC = 7,
        USDT = 100,
    }
    public enum TimeFrameType
    {
        [Description("1m")]
        Minute1 = 1,
        [Description("5m")]
        Minute5 = 5,
        [Description("10m")]
        Minute10 = 10,
        [Description("15m")]
        Minute15 = 15,
        [Description("30m")]
        Minute30 = 30,
        [Description("45m")]
        Minute45 = 45,
        [Description("1h")]
        Hour1 = 60,
        [Description("2h")]
        Hour2 = 120,
        [Description("4h")]
        Hour4 = 240,
        [Description("D")]
        Day = 1440,
        //Week = 31,
        //Month1 = 40,
        //Month3 = 41,
        //Month6 = 42,
        //Year = 50
    }

    public static class EnumExtension
    {
        private static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute, new()
        {
            var type = value.GetType();

            var name = Enum.GetName(type, value);
            if (name == null)
                return new TAttribute();

            var field = type.GetField(name);
            if (field == null)
            {
                return new TAttribute();
            }
            return field.GetCustomAttributes(false)
                        .OfType<TAttribute>()
                        .SingleOrDefault();
        }
        public static string GetEnumDescription(this Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var description = GetAttribute<DescriptionAttribute>(value);
            return description?.Description ?? value.ToString();
        }
    }
}
