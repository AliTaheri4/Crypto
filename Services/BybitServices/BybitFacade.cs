using Domain.Models.Bybit;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BybitServices
{
    public class BybitFacade
    {
        public readonly IConfiguration _configuration;
        public BybitFacade(IConfiguration configuration) { _configuration = configuration; }

        public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest reqModel)
        {
            var baseUrl = _configuration.GetValue<string>("Bybit:Url");
            var client = new RestClient(new Uri(baseUrl + "/v5/order/create"));
            //client.Timeout = -1;
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("X-BAPI-API-KEY", "JNLEWNAQHLQKIEBBYT");
            request.AddHeader("X-BAPI-TIMESTAMP", "1685957197103");
            request.AddHeader("X-BAPI-RECV-WINDOW", "20000");
            var body = @"{" + "\n" +
            @"  ""category"": ""linear""," + "\n" +
            @"  ""symbol"": ""ETHUSDT""," + "\n" +
            @"  ""isLeverage"": 0," + "\n" +
            @"  ""side"": ""Buy""," + "\n" +
            @"  ""orderType"": ""Limit""," + "\n" +
            @"  ""qty"": ""1""," + "\n" +
            @"  ""price"": ""1000""," + "\n" +
            @"  ""triggerPrice"": null," + "\n" +
            @"  ""triggerDirection"": null," + "\n" +
            @"  ""triggerBy"": null," + "\n" +
            @"  ""orderFilter"": null," + "\n" +
            @"  ""orderIv"": null," + "\n" +
            @"  ""timeInForce"": ""GTC""," + "\n" +
            @"  ""positionIdx"": 0," + "\n" +
            @"  ""orderLinkId"": ""test-xx1""," + "\n" +
            @"  ""takeProfit"": null," + "\n" +
            @"  ""stopLoss"": null," + "\n" +
            @"  ""tpTriggerBy"": null," + "\n" +
            @"  ""slTriggerBy"": null," + "\n" +
            @"  ""reduceOnly"": false," + "\n" +
            @"  ""closeOnTrigger"": false," + "\n" +
            @"  ""mmp"": null" + "\n" +
            @"}";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            return new PlaceOrderResponse();
        }


        public async Task<GetAllCoinsBalanceResponse> GetAllCoinsBalance(GetAllCoinsBalanceRequest reqModel)
        {
            var baseUrl = _configuration.GetValue<string>("Bybit:Url");
            var client = new RestClient(new Uri(baseUrl + "/v5/order/create"));
            //client.Timeout = -1;
            var request = new RestRequest();
            request.Method = Method.Post;

            request.AddHeader("X-BAPI-API-KEY", "JNLEWNAQHLQKIEBBYT");
            request.AddHeader("X-BAPI-TIMESTAMP", "1685958848732");
            request.AddHeader("X-BAPI-RECV-WINDOW", "20000");
            request.AddHeader("X-BAPI-SIGN", "e9c13d14bff016491ee44c5e941f4df98d4dee9835eec953a4a8a23fcc5119d6");
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            return new GetAllCoinsBalanceResponse();
        }
    }
}
