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
using Newtonsoft.Json;
using RepoDb;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Data;

namespace CryptoDataCollector.HostedServices
{
    public class SaveCandlesBackgroundInvokable : IInvocable

    {
        public ApplicationDbContext _context { get; set; }
        public IMediator _mediator { get; set; }

        public SaveCandlesBackgroundInvokable(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Invoke()
        {
            var now = DateTime.Now;
            var nowSecond = now.Second * 1000 + now.Microsecond;
            if (nowSecond >= 50000 && nowSecond < 59000)
                Console.WriteLine(now.ToString() + " " + now.Microsecond+"======"+ nowSecond.ToString());
            // await _mediator.Send(new SaveCandlesCommand());
        }

    }
}
