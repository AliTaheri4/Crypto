using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Bybit
{
    public class GetAllCoinsBalanceResponse: BaseBybitResponse
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public GetAllCoinsBalanceResult result { get; set; }
        public RetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }

    public class Balance
    {
        public string coin { get; set; }
        public string transferBalance { get; set; }
        public string walletBalance { get; set; }
        public string bonus { get; set; }
    }

    public class GetAllCoinsBalanceResult
    {
        public string memberId { get; set; }
        public string accountType { get; set; }
        public List<Balance> balance { get; set; }
    }

   
}

