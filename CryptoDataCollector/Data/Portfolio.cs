namespace CryptoDataCollector.Data
{
    public class Candle
    {
        public int Id{ get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public long Ticks { get; set; }
        public DateTime DateTime { get; set; }
        public int Symbol { get; set; }
        public string SymbolName { get; set; }
    }
}
