using Microsoft.Extensions.Configuration;
using RestSharp;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Facades
{
    public class BybitFacade
    {
        public IConfiguration _configuration { get; set; }
        public BybitFacade(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PlaceOrder()
        {
            var baseUrl = _configuration.GetSection("Bybit:Url").ToString();
            var client = new RestClient(baseUrl+"/v5/order/create");
           // client.Timeout = -1;
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            var model = new PlaceOrderRequest() { };
            request.AddBody(model);
            request.AddParameter("application/json", ParameterType.RequestBody);
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
        }


        public async Task GetAssets()
        {
            var baseUrl = _configuration.GetSection("Bybit:Url").ToString();

            var client = new RestClient(baseUrl+"/v5/asset/transfer/query-asset-info");
            //client.Timeout = -1;
            var request = new RestRequest();
            request.Method = Method.Get;
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
