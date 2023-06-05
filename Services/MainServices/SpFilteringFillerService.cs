using CryptoDataCollector.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MyProject.Models;
using System.Data;

namespace Services.HostedServices
{
    public class SpFilteringFillerService
    {
        public ApplicationDbContext _context;
        public IConfiguration _configuration;
        public IMemoryCache _cache;
        public SpFilteringFillerService(ApplicationDbContext context,IConfiguration configuration, IMemoryCache cache)
        {
            _context = context;
            _configuration = configuration;
            _cache = cache;
        } 
        public async Task Initializer()
        {
            var test = new SpFilteringModel();
            var res = await GetSpFilteringByLuckBTC1mResult();
            if (res?.Trades > 0)
            {
                _cache.Set("ByLuckBTC1m", res);
            }
            test = _cache.Get<SpFilteringModel>("ByLuckBTC1m");


            res = await GetSpFilteringLastTwoBigCandlesBTC30mResult();
            if (res?.Trades > 0)
            {
                _cache.Set("LastTwoBigCandlesBTC30m", res);
            }
            test = _cache.Get<SpFilteringModel>("LastTwoBigCandlesBTC30m");


            res = await GetSpFilteringLastTwoBigCandlesBNB30mResult();
            if (res?.Trades > 0)
            {
                _cache.Set("LastTwoBigCandlesBNB30m", res);
            }
            test = _cache.Get<SpFilteringModel>("LastTwoBigCandlesBNB30m");
        }
        public async Task<SpFilteringModel> GetSpFilteringByLuckBTC1mResult()
        {
            try
            {
                var connection = _configuration.GetConnectionString("Crypto");
                var result = new SpFilteringModel();
                var con = new SqlConnection(connection);

                using (var cmd = new SqlCommand("[GetSpFilteringByLuckBTC1mResult]", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        result = new SpFilteringModel()
                        {
                            WinRate = decimal.Parse(reader["WinRate"].ToString()),
                            Trades = int.Parse(reader["Trades"].ToString()),
                            Tp = int.Parse(reader["Tp"].ToString()),
                            Sl = int.Parse(reader["Sl"].ToString())
                        };
                    }
                    con.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<SpFilteringModel> GetSpFilteringLastTwoBigCandlesBTC30mResult()
        {
            try
            {
                var connection = _configuration.GetConnectionString("Crypto");
                var result = new SpFilteringModel();
                var con = new SqlConnection(connection);

                using (var cmd = new SqlCommand("[GetSpFilteringLastTwoBigCandlesBTC30mResult]", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        result = new SpFilteringModel()
                        {
                            WinRate = decimal.Parse(reader["WinRate"].ToString()),
                            Trades = int.Parse(reader["Trades"].ToString()),
                            Tp = int.Parse(reader["Tp"].ToString()),
                            Sl = int.Parse(reader["Sl"].ToString())
                        };
                    }
                    con.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<SpFilteringModel> GetSpFilteringLastTwoBigCandlesBNB30mResult()
        {
            try
            {
                var connection = _configuration.GetConnectionString("Crypto");
                var result = new SpFilteringModel();
                var con = new SqlConnection(connection);

                using (var cmd = new SqlCommand("[GetSpFilteringLastTwoBigCandlesBNB30mResult]", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        result = new SpFilteringModel()
                        {
                            WinRate = decimal.Parse(reader["WinRate"].ToString()),
                            Trades = int.Parse(reader["Trades"].ToString()),
                            Tp = int.Parse(reader["Tp"].ToString()),
                            Sl = int.Parse(reader["Sl"].ToString())
                        };
                    }
                    con.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
