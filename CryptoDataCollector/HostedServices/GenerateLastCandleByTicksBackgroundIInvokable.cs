using Coravel.Invocable;
using CryptoDataCollector.BussinesExtensions.Helper;
using CryptoDataCollector.CheckForSignall;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Domain.Data;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyProject.Handlers;
using MyProject.Models;
using Newtonsoft.Json;
using RepoDb;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Data;

namespace CryptoDataCollector.HostedServices
{
    public class GenerateLastCandleByTicksBackgroundIInvokable : IInvocable

    {
        public ApplicationDbContext _context { get; set; }
        public IMediator _mediator { get; set; }
        public readonly IDbConnection _dbConnection;
        public List<TickModel> List { get; set; } = new List<TickModel>();
        public static List<TickModel> AllList { get; set; } = new List<TickModel>();

        public GenerateLastCandleByTicksBackgroundIInvokable(ApplicationDbContext context, IDbConnection dbConnection, IMediator mediator)
        {
            _context = context;
            _dbConnection = dbConnection;
            _mediator = mediator;
        }

        public async Task Invoke()
        {
            Thread.Sleep(50000);
            await Execute();
        }
        public async Task Execute()
        {
            var now = DateTime.Now;
            var lastItemDt = AllList.Last().DateTime;
            var dt = new DateTime(lastItemDt.Year, lastItemDt.Month, lastItemDt.Day, lastItemDt.Hour, lastItemDt.Minute, 0);
            AllList = AllList.Where(p => p.DateTime >= now.AddMinutes(-210).AddSeconds(-now.Second) && p.DateTime < now.AddMinutes(-210).AddSeconds(-now.Second).AddMinutes(1)).OrderBy(p => p.DateTime).ToList();
            var groupby = AllList.GroupBy(p => p.Symbol).Select(p => new Candle()
            {
                Open = p.OrderBy(o => o.DateTime).First().Price,
                High = p.Max(p => p.Price),
                Low = p.Min(p => p.Price),
                Close = p.OrderBy(p => p.DateTime).Last().Price,
                DateTime = dt,
                Symbol = (int)p.Key,
                SymbolName = p.Key.GetEnumDescription(),
                Ticks = SecondsFromDate(dt),
                Volume = 1
            }).ToList();



            await ((SqlConnection)_dbConnection).BulkMergeAsync("dbo.Candles", groupby, x => new { x.Symbol, x.Ticks },
                       bulkCopyTimeout: 60 * 60,
                       batchSize: 5000
                      );

            AllList.Clear();

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

    }
}
