using CryptoDataCollector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector.Models
{
    public class SignalCheckerResult
    {
        public TradeType TradeType { get; set; } = TradeType.Unkouwn;
        public bool HasTriggred { get; set; } = false;
    }
}
