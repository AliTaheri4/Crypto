using CryptoDataCollector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector.Models
{
    public  class StatusTable
    {
       public  ProfitLossModel ProfitLoss { get; set; }=new ProfitLossModel();
        public  GeneralStaus GeneralStatus { get; set; }
        public  int CountNowChangeSlope { get; set; }
    }
}
