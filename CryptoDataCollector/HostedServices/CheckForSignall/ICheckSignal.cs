using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;

namespace MyProject.HostedServices.CheckForSignall
{
    public interface ICheckSignal<T>
    {
        SignalCheckerType CheckStrategy(List<T> list, int step, TimeFrameType timeFrame);

    }
}
