using Bybit.Net.Clients;
using Bybit.Net.Objects.Models.V5;
using CryptoDataCollector.HostedServices;
using CryptoExchange.Net.Sockets;
using MediatR;
using MyProject.Models;
using Symbol = CryptoDataCollector.Enums.Symbol;

namespace MyProject.Handlers
{
    public class StreamCommand : IRequest
    {

        public StreamCommand()
        {
        }

    }

    public class StreamHandler : AsyncRequestHandler<StreamCommand>
    {
        public List<string> Symbols { get; set; }
        public List<TickModel> Prices { get; set; }

        public StreamHandler()
        {
            Symbols = GetAllSymbols();
        }
        protected override async Task Handle(StreamCommand request, CancellationToken cancellationToken)
        {
            var client = new BybitSocketClient();
            var result = await client.V5SpotStreams.SubscribeToTickerUpdatesAsync(Symbols, async (p) => await GetTicker(p));
        }

        public async Task GetTicker(DataEvent<BybitSpotTickerUpdate> model)
        {
            var enumStr = PascalCase(model.Data.Symbol.Replace("USDT", ""));
            SaveCandlesHandler.AllList.Add(new TickModel()
            {
                DateTime = model.Timestamp,
                Price = model.Data.LastPrice,
                Symbol = Enum.Parse<Symbol>(enumStr)
            });
      //      Console.WriteLine($@"{model.Data.Symbol} at: {model.Timestamp.AddMinutes(210)} | price: {model.Data.LastPrice}");
        }
        public string PascalCase(string word)
        {
            return string.Join("", word.Split('_')
                         .Select(w => w.Trim())
                         .Where(w => w.Length > 0)
                         .Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1).ToLower()));
        }
        public List<string> GetAllSymbols()
        {
            var values = Enum.GetValues(typeof(Symbol));
            var list = new List<string>();
            foreach (var item in values)
            {
                list.Add($"{item}USDT".ToUpper());
            }
            return list;
        }
    }
}