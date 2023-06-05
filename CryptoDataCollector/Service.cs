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
using MyProject.HostedServices;

namespace CryptoDataCollector;
public class Service : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly SpFilteringFillerService _spFilteringFiller;
    public Service(IServiceScopeFactory scopeFactory, IMediator mediator, IConfiguration configuration, IMemoryCache cache, ApplicationDbContext context, SpFilteringFillerService spFilteringFiller)
    {
        _scopeFactory = scopeFactory;
        _mediator = mediator;
        _configuration = configuration;
        _cache = cache;
        _context = context;
        _spFilteringFiller = spFilteringFiller;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _spFilteringFiller.Initializer();
        var doGetTicksData = _configuration.GetValue<bool>("DoGetTicksData");
        if (doGetTicksData)
        {
            await _mediator.Send(new StreamCommand());
        }
        using var scope = _scopeFactory.CreateScope();

       // var sp = scope.ServiceProvider.GetRequiredService<SaveToDbBackgroundIInvokable>();   await sp.Invoke();

         //    var sp = scope.ServiceProvider.GetRequiredService<LastTwoBigCandlesBackground>(); await sp.Invoke();
       //  var sp = scope.ServiceProvider.GetRequiredService<LastByLuckBackground>(); await sp.Invoke();


     //  await _context.Database.ExecuteSqlRawAsync("GetByLuckSpFilteringByLuckBTC1mResult");

      
        await Task.CompletedTask;
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
