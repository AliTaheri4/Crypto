using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Skender.Stock.Indicators;

namespace CryptoDataCollector.BussinesExtensions.Helper
{
    public partial class UsingTaIndicators
    {
        public IndicatorsModel Calculate(decimal[] openAdj, decimal[] highAdj, decimal[] lowAdj, decimal[] lastAdj, decimal[] volume, DateTime[] date)
        {
            var ta = new TAHelper(lowAdj, highAdj, lastAdj, openAdj, volume, date);
           var  model = CalculateTa(ta);

            return model;


        }
    }
}
