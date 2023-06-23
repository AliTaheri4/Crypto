using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyProject.HostedServices.CheckForSignall
{
    public class BaseCheckSignal<T> //where T:  BaseSignalCheckingModel, ICheckSignal<T>
    {
        private ICheckSignal<T> checkSignal;
        public void SetStrategy(ICheckSignal<T> checkSignal)
        {
            this.checkSignal = checkSignal;
        }

        public void ExecuteStrategy(List<T> list, int step, TimeFrameType timeFrame)
        {
            checkSignal.CheckStrategy(list, step, timeFrame);
        }
        public SignalCheckerType CheckStrategy(List<T> list, int step, TimeFrameType timeFrame)
        {
            return checkSignal.CheckStrategy(list, step, timeFrame);
        }
    }
}
