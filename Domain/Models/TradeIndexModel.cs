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
}
