using CryptoDataCollector.Data;
using CryptoDataCollector.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProject.Models;
using RestSharp;
using Services.HostedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Services.MainServices
{
    public class TradeServices
    {
        public ApplicationDbContext _context;
        public IMemoryCache _cache;
        private readonly SpFilteringFillerService _spFilteringFiller;

        public TradeServices(ApplicationDbContext context, IMemoryCache cache, SpFilteringFillerService spFilteringFiller)
        {
            _context = context;
            _cache = cache;
            _spFilteringFiller = spFilteringFiller;
        }


        public async Task<bool> SpFilteringValidation(TradeIndexModel model)
        {
            async Task<List<SpFilteringModel>> GetOldAndNewSpFilterResult(TradeIndexModel model)
            {
                var list = new List<SpFilteringModel>();
                switch (model.SignalType)
                {
                    case SignalType.SmaCross:
                        break;
                    case SignalType.SmaDistance:
                        break;
                    case SignalType.SmaTouch:
                        break;
                    case SignalType.CciCrossLines:
                        break;
                    case SignalType.Fsp:
                        break;
                    case SignalType.DoubleEmaMacd:
                        break;
                    case SignalType.DivergenceCandleStickSr:
                        break;
                    case SignalType.LastTwoBigCandles:
                        list.Add(_cache.Get<SpFilteringModel>(@$"{SignalType.LastTwoBigCandles}{model.Symbol}{model.TimeFrameType.GetEnumDescription()}"));
                        list.Add(await _spFilteringFiller.GetSpFilteringLastTwoBigCandlesBTC30mResult());
                        break;
                    case SignalType.ByLuck:
                        break;
                    default:
                        break;
                }
                return list;
            }
            var oldAndNewSpFilters = await GetOldAndNewSpFilterResult(model);
            if (oldAndNewSpFilters[0].Trades != oldAndNewSpFilters[1].Trades)
                return true;

            return false;
        }

        public async Task<bool> CheckingForHaveBeforeBySameStratgyParameters(TradeIndexModel model)
        {
            var existSameTradeAsOpenPosition = await _context.Trades
                .AnyAsync(p => p.Symbol == model.Symbol && p.SignalType == model.SignalType && model.TimeFrameType == p.TimeFrameType && p.TradeResultType == TradeResultType.Pending);
            if (existSameTradeAsOpenPosition == true)
                return false;

            return true;
        }
        public async Task<decimal> PriceManagement()
        {
            return 0;
        }

        public async Task<bool> StartBuyProcessing(TradeIndexModel model)
        {
            var checking1 = await SpFilteringValidation(model);
            if (checking1 == false)
                return false;

            var checking2 = await CheckingForHaveBeforeBySameStratgyParameters(model);
            if (checking2 == false)
                return false;

            return true;
        }
    }
}
