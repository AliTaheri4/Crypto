using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using CryptoDataCollector.HostedServices;
using CryptoDataCollector.CheckForSignall;
using MediatR;
using MyProject.Handlers;

namespace CryptoDataCollector;
public class Service : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    public Service(IServiceScopeFactory scopeFactory, IMediator mediator, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _mediator = mediator;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var doGetTicksData = _configuration.GetValue<bool>("DoGetTicksData");
        if (doGetTicksData)
        {
            await _mediator.Send(new StreamCommand());
        }
        using var scope = _scopeFactory.CreateScope();

        var sp = scope.ServiceProvider.GetRequiredService<SaveToDbBackgroundIInvokable>();   await sp.Invoke();

        //  var sp = scope.ServiceProvider.GetRequiredService<LastTwoBigCandlesBackground>();
        // var sp = scope.ServiceProvider.GetRequiredService<LastByLuckBackground>();


            


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
