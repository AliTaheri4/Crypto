using Coravel.Invocable;
using CryptoDataCollector.CheckForSignall;
using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using CryptoDataCollector.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RepoDb;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Data;

namespace CryptoDataCollector.HostedServices
{
    public class CalculationProsConsBackgroundIInvokable : IInvocable

    {
        public ApplicationDbContext _context { get; set; }
        public readonly IDbConnection _dbConnection;
        public List<SmaSignalCheckingModel> List { get; set; } = new List<SmaSignalCheckingModel>();
        public DateTime LastRequestDateTime { get; set; } = DateTime.Now;
        public int _symbol { get; set; } = 1;
        public DateTime _to { get; set; } = new DateTime(2023, 1,13, 0, 0, 0);
        public CalculationProsConsBackgroundIInvokable(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        public async Task Invoke()
        {

            double wallet = 10;
            var trades = _context.Trades.Where(p => p.Id > 0 && p.Id < 10000).ToList();
            for (int i = 0; i < trades.Count; i++)
            {
                if (i == 100)
                {

                }
            
                double percent = 0;
                if (trades[i].TradeResultType == TradeResultType.BecameLoss)
                    percent = -3.8;
                else if (trades[i].TradeResultType == TradeResultType.BecameProfit)
                    percent = 10.3;

                wallet = (percent / 100) * wallet + wallet;
            }

            return;
        }
    }
}
