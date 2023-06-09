using Coravel.Invocable;
using CryptoDataCollector.BussinesExtensions.Helper;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Domain.Data;
using Newtonsoft.Json;
using Quartz;
using Skender.Stock.Indicators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDataCollector.HostedServices
{
    public class FSPBackgroundJob : IInvocable
    {
        public ApplicationDbContext _context { get; set; }
        public int _symbol { get; set; } = (int)Symbol.BNB;
        public DateTime _to { get; set; } = new DateTime(2023, 1, 31, 0, 0, 0);

        public FSPBackgroundJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Invoke()
        {
            //await Console.Out.WriteLineAsync("Executing background job");

            int whileNumber = 0;
            bool start = true;
            int sleep = 120 * 1000;
            var now = DateTime.Now;
            //var from = SecondsFromDate(now.AddMinutes(-1000));
            var to = SecondsFromDate(_to);
            //to = to - 12600;
            var from = to - (86400 * 2);

            var startFrom = from;

            int skip = 0;
            int cond = 4 - 4;
            while (start)
            {
                try
                {
                    Console.WriteLine("Try Number:" + whileNumber.ToString());

                    whileNumber++;

                    if (whileNumber < skip)
                        continue;

                    skip++;
                    var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
                    var res = (long)now.Subtract(baseDate).TotalSeconds;



                    var chart = new Chart();
                    int triedNumber = 1;

                    var candles = _context.Candles.Where(p => p.Ticks >= from && p.Ticks <= to && p.Symbol == _symbol).OrderBy(p => p.Ticks).ToList();
                    chart.o = new List<decimal>();
                    chart.h = new List<decimal>();
                    chart.l = new List<decimal>();
                    chart.c = new List<decimal>();
                    chart.v = new List<decimal>();
                    chart.t = new List<long>();
                    chart.o.AddRange(candles.Select(p => p.Open).ToList());
                    chart.h.AddRange(candles.Select(p => p.High).ToList());
                    chart.l.AddRange(candles.Select(p => p.Low).ToList());
                    chart.c.AddRange(candles.Select(p => p.Close).ToList());
                    chart.v.AddRange(candles.Select(p => p.Volume).ToList());
                    chart.t.AddRange(candles.Select(p => p.Ticks).ToList());


                    //       }

                    //Console.WriteLine("end at:" + now.ToString());

                    var dates = new List<DateTime>();
                    int index = 0;
                    Console.WriteLine("from of chart: " + DateFromTicks(chart.t[0]) + " and to of chart: " + DateFromTicks(chart.t.Last()).AddMinutes(210));


                    var dateSpecific = new DateTime(2022, 12, 22, 18, 55, 0);
                    var ticksToAsDate = dateSpecific;

                    if (DateFromTicks(chart.t.Last()) == ticksToAsDate)
                    {

                    }

                    List<Quote> quotesList = CreateQuotes(chart);


                    List<StochResult> stochResultsList = quotesList.GetStoch(14, 4, 4).ToList();
                    List<FisherTransformResult> fisherResultsList = quotesList.GetFisherTransform(15).ToList();
                    List<ParabolicSarResult> psarResultsList = quotesList.GetParabolicSar(0.02, 0.2, 0.02).ToList();


                    var list = new List<AdaSignalCheckingModel>();
                    int quotesListCount = quotesList.Count;
                    var lastDatetime = DateFromTicks(to);
                    double[] SARFinalData = new double[chart.l.Count];
                    _ = TALib.Core.Sar(inHigh: chart.h.Select(p => (double)p).ToArray(),
                                           inLow: chart.l.Select(p => (double)p).ToArray(),
                                           startIdx: 0,
                                           endIdx: chart.h.Count - 1,
                                           outReal: SARFinalData,
                                           outBegIdx: out var f,
                                           outNbElement: out var e);
                    SARFinalData = ShiftData(SARFinalData, f, e);



                    var ta = new UsingTaIndicators();
                    var indicators = ta.Calculate(chart.o.Select(p => p).ToArray(),
                        chart.h.Select(p => p).ToArray(),
chart.l.Select(p => p).ToArray(),
                         chart.c.Select(p => p).ToArray(),
                         chart.v.Select(p => p).ToArray(),
                         chart.t.Select(p => DateFromTicks(p)).ToArray());


                    double[] CdlHammer = new double[chart.l.Count];

                    for (int i = 1; i < quotesList.Count; i++)
                    {
                        if (DateFromTicks(chart.t[i - 1]).AddMinutes(210) > lastDatetime)
                            break;
                        //Quote q = quotesList[i];

                        //       quotesList[i - 1].Date = quotesList[i - 1].Date.AddMinutes(-210);
                        StochResult stochE = stochResultsList[i - 1];   // evaluation period


                        FisherTransformResult fisherE = fisherResultsList[i];   // evaluation period


                        ParabolicSarResult psarE = psarResultsList[i - 1];   // evaluation period

                        if (i <= quotesListCount)
                            list.Add(new AdaSignalCheckingModel()
                            {
                                Open = chart.o[i - 1],
                                High = chart.h[i - 1],
                                Low = chart.l[i - 1],
                                Close = chart.c[i - 1],
                                DateTime = DateFromTicks(chart.t[i - 1]).AddMinutes(210),
                                Fisher = fisherE.Trigger ?? 0,
                                TalibPSar = SARFinalData[i - 1],
                                PSar = psarE.Sar ?? 0,
                                PSarIsReversel = psarE.IsReversal ?? false,
                                Stoch = stochE.K ?? 0,
                                Volume = chart.v[i - 1],
                                CdlHammer = indicators.CdlHammer[i - 1],
                                SomeThing = indicators.CdlClosingMarubozuData[i - 1],
                                CdlEngulfing = indicators.CdlEngulfingData[i - 1],
                                Macd=indicators.Macd[i - 1]
                            });
                    }



                    //return SARFinalData;

                    foreach (var item in list)
                    {
                        if (item.CdlHammer != 0)
                        {

                        }
                        if (item.SomeThing != 0)
                        {

                        }
                        if (item.CdlEngulfing != 0)
                        {

                        }


                        Console.WriteLine(@$"dt:{item.DateTime} - open: {item.Open}  - high: {item.High}  - low: {item.Low}  - close: {item.Close}  - hammmer: {item.CdlHammer} - enfulfing: {item.CdlEngulfing} - st: {item.SomeThing}");

                    }
                    var checkAdaSingnal = new CheckAdaSingnal(_context);
                    var isSuccess = checkAdaSingnal.CheckAdaStrategy(list);
                    if (isSuccess)
                    {
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");

                        for (int i = 0; i < 2; i++)
                        {
                            var nowBuy = DateTime.Now;
                            start = true;
                            Console.WriteLine("=================================");
                            Console.WriteLine("Buy at:" + list.OrderByDescending(p => p.DateTime).First().DateTime);
                            Console.WriteLine("=================================");
                        }

                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");


                    }

                    from = from + 86400;
                    to = to + 86400;

                    //      for gathering data          from = from + 86400 - 12600;
                    //  for gathering data       to = to +  86400 - 12600;



                    //var datesAcs = dates.OrderByDescending(p => p).ToArray();
                    //var dateTest1 = DateFromTicks(1671792360);
                    //var dateTest2 = DateFromTicks(1671715985);
                    //var dateTest3 = DateFromTicks(1671819185);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(sleep);
                }

            }

            //await Task.Delay(10000, stoppingToken);

            return;
        }

        public static DateTime DateFromTicks(long ticks)
        {
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return baseDate.AddMilliseconds(ticks * 1000);
        }

        public static long SecondsFromDate(DateTime date)
        {
            //date = date.Date;
            var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return (long)date.Subtract(baseDate).TotalSeconds;
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

        public static List<Quote> CreateQuotes(Chart chart)
        {
            var quotes = new List<Quote>();
            int indexChartQuote = 0;
            foreach (var item in chart.t)
            {

                quotes.Add(new Quote()
                {
                    Open = chart.o[indexChartQuote],
                    High = chart.h[indexChartQuote],
                    Low = chart.l[indexChartQuote],
                    Close = chart.c[indexChartQuote],
                    Date = DateFromTicks(chart.t[indexChartQuote]),
                    Volume = chart.v[indexChartQuote],
                });
                indexChartQuote++;
            }
            return quotes;
        }
    }
}
