using CryptoDataCollector.Enums;

namespace CryptoDataCollector.Models
{
    public class PivotsModel
    {
        public Dictionary<int, decimal> Ph { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> Pl { get; set; } = new Dictionary<int, decimal>();
    }

    public class PivotModel
    {
        public HighLowType HighLowType { get; set; }
        public decimal Price { get; set; }

    }
}
