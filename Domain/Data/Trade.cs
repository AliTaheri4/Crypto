using CryptoDataCollector.Enums;

namespace CryptoDataCollector.Data
{
    public class Trade:BaseEntity
    {
        public int Id { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }
        public bool IsEmotional{ get; set; }
        public TimeFrameType TimeFrameType{ get; set; }
        public Symbol Symbol { get; set; }
        public string SymbolName { get; set; }
        public TradeType TradeType{ get; set; }
        public SignalType SignalType{ get; set; }
        public TradeResultType TradeResultType{ get; set; }
        public DateTime BuyTime { get; set; }
        public DateTime? SellTime { get; set;}
        public decimal DistancePercentFromSma { get; set; }
        public decimal Indicator1 { get; set; }
        public decimal Indicator2 { get; set; }
        public decimal Indicator3 { get; set; }
        public int Leverage { get; set; }
        public int? NeedingInRangeCandles{ get; set; }
        public int? CountGreenCandles{ get; set; }
        public int? CountRedCandles{ get; set; }
        public int? CountGrayCandles{ get; set; }
        public decimal ThirdLastCandleVolume { get; set; }
        public decimal ForthLastCandleVolume { get; set; }
        public decimal Profit{ get; set; }
        public decimal Loss{ get; set; }
        public decimal SignalCandleClosePrice { get; set; }
        public string Description{ get; set; }
        public decimal OpenThird{ get; set; }
        public decimal HighThird { get; set; }
        public decimal LowThird { get; set; }
        public decimal CloseThird { get; set; }
        
        public decimal OpenForth { get; set; }
        public decimal HighForth { get; set; }
        public decimal LowForth { get; set; }
        public decimal CloseForth { get; set; }
        
        public decimal OpenLast{ get; set; }
        public decimal HighLast { get; set; }
        public decimal LowLast { get; set; }
        public decimal CloseLast { get; set; }
        public decimal OpenCurrent{ get; set; }
        public decimal HighCurrent { get; set; }
        public decimal LowCurrent { get; set; }
        public decimal CloseCurrent { get; set; }
        public decimal Sma21 { get; set; }
        public decimal Sma50 { get; set; }
        public decimal Sma100 { get; set; }
        public decimal Sma200 { get; set; }
        public decimal Ema21{ get; set; }
        public decimal Ema50{ get; set; }
        public decimal Ema100{ get; set; }
        public decimal Ema200{ get; set; }
    }
}
