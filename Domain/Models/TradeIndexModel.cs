using CryptoDataCollector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class TradeIndexModel
    {
        public Symbol Symbol { get; set; }
        public SignalType   SignalType{ get; set; }
        public TimeFrameType TimeFrameType { get; set; }
        public DateTime BuyTime{ get; set; }
    }



    public class StratgiesDetailsModel
    {
        public SignalType SignalType { get; set; }
        public TimeFrameType TimeFrameType { get; set; }
        public Symbol Symbol { get; set; }
        public int CountDecimal { get; set; }
        public int RR { get; set; }
        public decimal? BigCandle { get; set; }
        public decimal? StopLoss { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
