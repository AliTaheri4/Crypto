using System.ComponentModel;

namespace CryptoDataCollector.Enums
{
    public enum CandleStickType
    {
        [Description("Bulish Engulfing")]
        BulishEngulfing=1,
        [Description("Bearish Engulfing")]
        BearishEngulfing = 2,

        [Description("Bulish Harami")]
        BulishHarami = 3,
        [Description("Bearish Harami")]
        BearishHarami = 4,
    }

    public enum HighLowType
    {
        High = 1,
        Low = 2, 
    }

}
