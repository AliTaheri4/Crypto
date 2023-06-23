using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using CryptoDataCollector.HostedServices;
using CryptoDataCollector.CheckForSignall;
using MediatR;
using MyProject.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Accord.Math.Geometry;
using Accord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using MyProject.Models;
using CryptoDataCollector.Data;
using Services.HostedServices;
using Domain.Data;
using Services.MainServices;
using Services.BybitServices;
using Domain.Models.Bybit;
using CryptoDataCollector.Enums;
using System.Linq;

namespace CryptoDataCollector;
public class Service : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly SpFilteringFillerService _spFilteringFiller;
    private readonly BybitFacade _bybitFacade;
    public Service(IServiceScopeFactory scopeFactory, IMediator mediator, IConfiguration configuration, IMemoryCache cache, ApplicationDbContext context, SpFilteringFillerService spFilteringFiller, BybitFacade bybitFacade)
    {
        _scopeFactory = scopeFactory;
        _mediator = mediator;
        _configuration = configuration;
        _cache = cache;
        _context = context;
        _spFilteringFiller = spFilteringFiller;
        _bybitFacade = bybitFacade;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
     //   await Init();


        var doGetTicksData = _configuration.GetValue<bool>("DoGetTicksData");
        if (doGetTicksData)
        {
            await _mediator.Send(new StreamCommand());
        }
        using var scope = _scopeFactory.CreateScope();

         // var sp = scope.ServiceProvider.GetRequiredService<SaveToDbBackgroundIInvokable>();   await sp.Invoke();

     //  var sp = scope.ServiceProvider.GetRequiredService<LastTwoBigCandlesBackground>(); await sp.Invoke();
      //    var sp = scope.ServiceProvider.GetRequiredService<LastByLuckBackground>(); await sp.Invoke();
      
        
        await _mediator.Send(new StratgyRunnerCommand());


        //  await _context.Database.ExecuteSqlRawAsync("GetByLuckSpFilteringByLuckBTC1mResult");


        await Task.CompletedTask;
    }

    private async Task Init()
    {
        await _spFilteringFiller.Initializer();
        var newAssets = await _bybitFacade.GetAllCoinsBalance(new GetAllCoinsBalanceRequest());
        var oldAssets = await _context.Assets.ToListAsync();
        var assets = new List<Asset>();

        for (int i = 0; i < newAssets.result.balance.Count; i++)
        {
            var coin = newAssets.result.balance[i].coin.Trim();// Enum.Parse<Symbol>(newAssets.result.balance[i].coin);
            coin = coin.Length > 5 ? coin.Replace("USDT", "") : coin;
            assets.Add(new Asset()
            {
                CreatedName = "Init",
                CreatedTime = DateTime.Now,
                ModifiedName = "Init",
                ModifiedTime = DateTime.Now,
                Quantity = decimal.Parse(newAssets.result.balance[i].walletBalance),
                Symbol = Enum.Parse<Symbol>(coin),
                TradeType=TradeType.Unkouwn
            });
        }
        await _context.Assets.AddRangeAsync(assets);
        _context.Assets.RemoveRange(oldAssets);
        await _context.SaveChangesAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
    }
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

}//end class
