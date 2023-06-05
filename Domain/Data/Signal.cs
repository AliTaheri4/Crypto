using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector.Data
{
    public class Signal:BaseEntity
    {
        public int Id{ get; set; }
        public decimal Profit { get; set; }
        public decimal Loss { get; set; }
        public GeneralStaus GeneralStatus { get; set; }
        public Symbol Symbol{ get; set; }

    }
}
