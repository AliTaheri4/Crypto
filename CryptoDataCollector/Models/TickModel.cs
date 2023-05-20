using CryptoDataCollector.Enums;

namespace MyProject.Models
{
    public class TickModel
    {
            public Symbol Symbol { get; set; }
            public DateTime DateTime { get; set; }
            public decimal Price { get; set; }
    }
}
