using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    internal class PlaceOrderRequest
    {
        public string category { get; set; }
        public string symbol { get; set; }
        public int isLeverage { get; set; }
        public string side { get; set; }
        public string orderType { get; set; }
        public string qty { get; set; }
        public string price { get; set; }
        public object triggerPrice { get; set; }
        public object triggerDirection { get; set; }
        public object triggerBy { get; set; }
        public object orderFilter { get; set; }
        public object orderIv { get; set; }
        public string timeInForce { get; set; }
        public int positionIdx { get; set; }
        public string orderLinkId { get; set; }
        public object takeProfit { get; set; }
        public object stopLoss { get; set; }
        public object tpTriggerBy { get; set; }
        public object slTriggerBy { get; set; }
        public bool reduceOnly { get; set; }
        public bool closeOnTrigger { get; set; }
        public object mmp { get; set; }
    }



}
