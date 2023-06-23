using System;

namespace CryptoDataCollector.Models
{
    public class BaseRichCandleStickModel
    {
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public double Pivot { get; set; }
        public double Changable { get; set; }

    }
}
