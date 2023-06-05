using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector.Models
{
    public class AdaSignalCheckingModel
    {
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public double Fisher { get; set; }
        public double Stoch { get; set; }
        public double PSar { get; set; }
        public double TalibPSar { get; set; }
        public bool PSarIsReversel { get; set; }
        public int CdlHammer { get; set; }
        public int SomeThing { get; set; }
        public int CdlEngulfing { get; set; }
        public decimal Macd { get; set; }

    }
}
