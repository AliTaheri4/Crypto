using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector.Models
{
    public class SmaSignalCheckingModel
    {
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public double Sma14 { get; set; }
        public double Sma200 { get; set; }
        public double Ema100 { get; set; }
        public double Ema200 { get; set; }
    }
}
