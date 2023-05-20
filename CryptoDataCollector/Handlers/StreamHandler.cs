//using Bybit.Net.Clients;
//using Bybit.Net.Objects.Models.Socket;
//using Bybit.Net.Objects.Models.V5;
//using CryptoDataCollector.Data;
//using CryptoExchange.Net.Sockets;
//using MediatR;
//using MediatR.Wrappers;
//using System.Collections.Generic;
//using System.Threading;

//namespace MyProject.Handlers
//{
//    public class StreamCommand : IRequest
//    {

//        public StreamCommand()
//        {
//        }

//    }

//    public class StreamHandle : AsyncRequestHandler<StreamCommand>
//    {

//        public StreamHandle()
//        {
//        }
//        protected override async Task Handle(StreamCommand request, CancellationToken cancellationToken)
//        {
//        }



//    }
//    public class TestStream
//    {
//        private readonly IMediator _mediator;
//        public int count = 0;
//        public TestStream(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        public async Task Init()
//        {
//            var client = new BybitSocketClient();
//            dynamic test;
//            var result = await client.V5SpotStreams.SubscribeToTickerUpdatesAsync(new List<string>() { "BTCUSDT" ,"ETHUSDT","BNBUSDT","XRPUSDT","ADAUSDT"}, async (p) => await GetTicker(p));
//       //     var result = await client.V5LinearStreams.SubscribeToTradeUpdatesAsync("BTCUSDT", async (p) => await GetTrade(p));
//      //      var result = await client.V5LinearStreams.SubscribeToKlineUpdatesAsync("BTCUSDT", Bybit.Net.Enums.KlineInterval.OneMinute, async (p) =>await GetData(p));

//        }
//        public async Task GetTicker(DataEvent<BybitSpotTickerUpdate> model)
//        {
//            //      Thread.Sleep(5000);
//            count++;
//                  dynamic test;
//                test = model;

//            Console.WriteLine($@"{model.Data.Symbol} at: {model.Timestamp.AddMinutes(210)} | price: {model.Data.LastPrice}");  
//        }
//        public async Task GetTrade(DataEvent<IEnumerable<Bybit.Net.Objects.Models.V5.BybitTrade>> model)
//        {
//                Thread.Sleep(5000);
//                dynamic test;
//                test = model; 
//                Console.WriteLine($@"BTCUSDT at: {model.Timestamp} | price: {model.Data.First().Price}");  
//        }
//        public async Task GetData(DataEvent<IEnumerable<Bybit.Net.Objects.Models.V5.BybitKlineUpdate>> model)
//        {
//                Thread.Sleep(5000);
//                dynamic test;
//                test = model;
//                Console.WriteLine($@"BTCUSDT at: {model.Timestamp} | price: {model.Data.First().ClosePrice}");
//        }
//    }
//}