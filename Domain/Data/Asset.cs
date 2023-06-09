using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;

namespace Domain.Data
{
    public class Asset: BaseEntity
    {
        public int Id { get; set; }
        public Symbol Symbol{ get; set; }
        public decimal Quantity{ get; set; }
        public TradeType TradeType{ get; set; }
    }
}
