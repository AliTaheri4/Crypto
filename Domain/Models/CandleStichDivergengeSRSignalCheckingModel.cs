using System;

namespace CryptoDataCollector.Models
{
    public class CandleStichDivergengeSRSignalCheckingModel
    {
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }


        public double Macd { get; set; }
        public double MacdHist { get; set; }
        public double Rsi { get; set; }
        public double Stoch { get; set; }
        public double Mfi { get; set; }
        public double Cmf { get; set; }
        public double Cci { get; set; }
        public double Mom { get; set; }

        public double Sma21 { get; set; }
        public double Sma50 { get; set; }
        public double Sma100 { get; set; }
        public double Sma200 { get; set; }
        public double Ema21 { get; set; }
        public double Ema50 { get; set; }
        public double Ema100 { get; set; }
        public double Ema200 { get; set; }
        

    }
}
