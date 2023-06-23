using System;

namespace CryptoDataCollector.Models
{
    public class RichCandleStickModel: BaseRichCandleStickModel
    {
     


        public double Macd { get; set; } = 0;
        public double MacdHist { get; set; } = 0;
        public double Rsi { get; set; } = 0;
        public double Stoch { get; set; } = 0;
        public double Mfi { get; set; } = 0;
        public double Cmf { get; set; } = 0;
        public double Cci { get; set; } = 0;
        public double Mom { get; set; } = 0;

        public double Sma21 { get; set; } = 0;
        public double Sma50 { get; set; } = 0;
        public double Sma100 { get; set; } = 0;
        public double Sma200 { get; set; } = 0;
        public double Ema21 { get; set; } = 0;
        public double Ema50 { get; set; } = 0;
        public double Ema100 { get; set; } = 0;
        public double Ema200 { get; set; } = 0;
        public double Ema200IndicatorHelp { get; set; } = 0;


    }
}
