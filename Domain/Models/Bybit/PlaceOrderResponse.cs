using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Bybit
{

    public class PlaceOrderResponse: BaseBybitResponse
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public PlaceOrderResult result { get; set; }
        public RetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }


    public class PlaceOrderResult
    {
        public string orderId { get; set; }
        public string orderLinkId { get; set; }
    }


}
