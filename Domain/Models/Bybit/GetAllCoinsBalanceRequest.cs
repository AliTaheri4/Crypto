using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Bybit
{
    public class GetAllCoinsBalanceRequest
    {
        public string memberId { get; set; }
        public string accountType { get; set; }
        public string coin { get; set; }
        public int withBonus { get; set; }
        public int withTransferSafeAmount { get; set; }
    }
}
