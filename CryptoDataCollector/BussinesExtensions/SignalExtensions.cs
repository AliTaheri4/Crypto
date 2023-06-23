using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;

namespace CryptoDataCollector.BussinesExtensions
{
    public static class SignalExtensions
    {
        public static ChartProcessType GetChartProcessType(List<BaseRichCandleStickModel> list, int countLasts)
        {
            var lastCandles = list.TakeLast(countLasts).ToList();
            if (lastCandles[0].Pivot + ((lastCandles[0].Pivot / 10000) * 5) < lastCandles[countLasts - 1].Pivot)
            {
                return ChartProcessType.Ascending;
            }
            else if (lastCandles[0].Pivot - ((lastCandles[0].Pivot / 10000) * 5) > lastCandles[countLasts - 1].Pivot)
            {
                return ChartProcessType.Descending;
            }
            else
                return ChartProcessType.Flat;
        }

        public static decimal SelectBottomOfBodyCandle(BaseRichCandleStickModel model)
        {

            if (model.Open < model.Close)
                return model.Open;

            return model.Close;
        }
        public static decimal SelectTopOfBodyCandle(BaseRichCandleStickModel model)
        {

            if (model.Open > model.Close)
                return model.Open;

            return model.Close;
        }

        // 100, 98 => 2
        //100, 2 => 98
        public static decimal GetPercentWithDiffrences(decimal great, decimal less)
        {

            if (great < less)
            {
                var temp = great;
                great = less;
                less = temp;
            }
            var percent = ((great - less) / great) * 100;
            // var percent = ((great - less) / less) * 100;
            return percent;
        }
        // 100, 98 => 98
        // 100, 2 => 2
        public static decimal GetPercentOfValue(decimal val, decimal percent)
        {


            var value = val / 100 * percent;
            // var percent = ((great - less) / less) * 100;
            return value;
        }

        //100, 98 => 98
        //100,2 => 2
        public static decimal GetPercent(decimal great, decimal less)
        {

            if (great < less)
            {
                var temp = great;
                great = less;
                less = temp;
            }
            var percent = less / great * 100;
            // var percent = ((great - less) / less) * 100;
            return percent;
        }
        public static CandleType GetCandleType(BaseRichCandleStickModel candle)
        {
            if (candle.Open > candle.Close)
                return CandleType.Red;
            else if (candle.Open < candle.Close)
                return CandleType.Green;
            else
                return CandleType.Gray;
        }


        public static int GetLeverage(decimal diff)
        {
            if (diff < 0.4M)
                return 15;
            else if (diff < 0.6M)
                return 10;
            else if (diff < 0.8M)
                return 8;
            else
                return 8;
        }
        public static List<Candle> GroupedByTimeFrame(List<Candle> CandleList, int result)
        {
            var groupebBy = CandleList.GroupBy(p => GroupedByMinute(p.DateTime, 1));

            if (result == 1)
                groupebBy = CandleList.GroupBy(p => GroupedByMinute(p.DateTime, 1));
            else if (result == 5)
                groupebBy = CandleList.GroupBy(p => GroupedByMinute(p.DateTime, 5));
            else if (result == 15)
                groupebBy = CandleList.GroupBy(p => GroupedByMinute(p.DateTime, 15));
            else if (result == 30)
                groupebBy = CandleList.GroupBy(p => GroupedByMinute(p.DateTime, 30));
            else if (result == 60)
                groupebBy = CandleList.GroupBy(p => GroupedByHour(p.DateTime, 1));
            else if (result == 180)
                groupebBy = CandleList.GroupBy(p => GroupedByHour(p.DateTime, 3));

            var res = groupebBy.Select(p => new Candle
            {
                DateTime = p.OrderBy(o => o.Ticks).FirstOrDefault().DateTime,
                Open = p.OrderBy(o => o.Ticks).FirstOrDefault().Open,
                Close = p.OrderBy(o => o.Ticks).LastOrDefault().Close,
                High = p.OrderBy(o => o.Ticks).Max(m => m.High),
                Low = p.OrderBy(o => o.Ticks).Min(m => m.Low),
                Volume = p.OrderBy(o => o.Ticks).Sum(s => s.Volume),
                Ticks = p.OrderBy(o => o.Ticks).FirstOrDefault().Ticks,
                Symbol = p.OrderBy(o => o.Ticks).FirstOrDefault().Symbol,
                SymbolName = p.OrderBy(o => o.Ticks).FirstOrDefault().SymbolName,
            }).ToList();
            return res;
        }

        public static int GetTimeFrameType(TimeFrameType _timeFrame)
        {
            var minutes = 0;
            var timeFrameType = TimeFrameType.Day;
            int result = (int)_timeFrame;
            if (_timeFrame == TimeFrameType.Minute1)
            {
                minutes = 1;
                timeFrameType = TimeFrameType.Minute1;
            }
            else if (_timeFrame == TimeFrameType.Minute5)
            {
                minutes = 5;
                timeFrameType = TimeFrameType.Minute5;
            }
            else if (_timeFrame == TimeFrameType.Minute10)
            {
                minutes = 10;
                timeFrameType = TimeFrameType.Minute10;
            }
            else if (_timeFrame == TimeFrameType.Minute15)
            {
                minutes = 15;
                timeFrameType = TimeFrameType.Minute15;
            }
            else if (_timeFrame == TimeFrameType.Minute30)
            {
                minutes = 30;
                timeFrameType = TimeFrameType.Minute30;
            }
            else if (_timeFrame == TimeFrameType.Minute45)
            {
                minutes = 45;
                timeFrameType = TimeFrameType.Minute45;
            }
            else if (_timeFrame == TimeFrameType.Hour1)
            {
                minutes = 60;
                timeFrameType = TimeFrameType.Hour1;
            }

            return result;
        }

        public static DateTime GroupedByMinute(DateTime stamp, int byMinute)
        {
            stamp = stamp.AddMinutes(-(stamp.Minute % byMinute));
            stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
            return stamp;
        }
        public static DateTime GroupedByHour(DateTime stamp, int byHour)
        {
            stamp = stamp.AddHours(-(stamp.Hour % byHour));
            stamp = stamp.AddMinutes(-stamp.Minute);
            stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
            return stamp;
        }
        public static decimal[] ShiftData(decimal[] oldData, int f, int e)
        {
            var newData = new decimal[oldData.Length];
            for (var i = 0; i < e; i++)
            {
                newData[f + i] = oldData[i];
            }
            return newData;
        }
        public static double[] ShiftData(double[] oldData, int f, int e)
        {
            var newData = new double[oldData.Length];
            for (var i = 0; i < e; i++)
            {
                newData[f + i] = oldData[i];
            }
            return newData;
        }
        public static int[] ShiftData(int[] oldData, int f, int e)
        {
            var newData = new int[oldData.Length];
            for (var i = 0; i < e; i++)
            {
                newData[f + i] = oldData[i];
            }
            return newData;
        }

        public static void IsDivergent(double[] closes, double[] indicator, int pivotPeriod, int maximumBarsToChck)
        {
            // Check if the input arrays are valid
            if (closes == null || indicator == null || closes.Length != indicator.Length)
            {
                throw new ArgumentException("Invalid input arrays.");
            }

            // Check if the pivot period is valid
            if (pivotPeriod <= 0 || pivotPeriod >= closes.Length)
            {
                throw new ArgumentException("Invalid pivot period.");
            }

            for (int i = 0; i < closes.Length; i++)
            {
                if (i < pivotPeriod)
                    continue;


            }

        }


        public static List<CandleStickType> GetCandleStick(List<Candle> candles)
        {
            var current = candles.Last();
            var currentColor = GetCandleType(new BaseRichCandleStickModel() { Open = current.Open, High = current.High, Low = current.Low, Close = current.Close });
            var before = candles.TakeLast(2).First();
            var beforeColor = GetCandleType(new BaseRichCandleStickModel() { Open = before.Open, High = before.High, Low = before.Low, Close = before.Close });

            var res = new List<CandleStickType>();

            //engulfings
            if (current.Close <= before.Open && current.Open >= before.Close && beforeColor != CandleType.Red && currentColor == CandleType.Red && (current.Close != before.Open || current.Open != before.Close) && (current.Open != current.Close && before.Open != before.Close))
            {
                res.Add(CandleStickType.BearishEngulfing);
            }
            else if (current.Open >= before.Close && current.Close <= before.Open && beforeColor == CandleType.Red && currentColor != CandleType.Red && (current.Close != before.Open || current.Open != before.Close) && (current.Open != current.Close && before.Open != before.Close))
            {
                res.Add(CandleStickType.BulishEngulfing);
            }
            //haramies
            else if (before.Close >= current.Open && before.Open <= current.Close && beforeColor != CandleType.Red && currentColor == CandleType.Red && (current.Close != before.Open || current.Open != before.Close) && (current.Open != current.Close && before.Open != before.Close))
            {
                res.Add(CandleStickType.BearishHarami);
            }
            else if (before.Close <= current.Open && before.Open >= current.Close && beforeColor == CandleType.Red && currentColor != CandleType.Red && (current.Close != before.Open || current.Open != before.Close) && (current.Open != current.Close && before.Open != before.Close))
            {
                res.Add(CandleStickType.BulishHarami);
            }
            return res;
        }

        public static DateTime RemoveSecondTicks(this DateTime dateTime, bool toPersianDt = false)
        {
            var dt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
            if (toPersianDt)
                dt = dt.AddMinutes(-210);

            return dt;
        }
    }
}
