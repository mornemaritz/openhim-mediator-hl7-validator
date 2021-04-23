using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WcPhdc.OpenHim.Mediator.Configuration;
using WcPhdc.OpenHim.Mediator.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WcPhdc.OpenHim.Mediator.Services
{
    public class OpenHimHeartbeatService : IHostedService, IDisposable
    {
        private readonly IOpenHimCoreClient _openHimCoreClient;
        private readonly int _heartbeatIntervalSeconds;
        private Timer _timer;

        /// <summary>
        /// Service that POSTs a heartbeat request to the OpenHim core system.
        /// Based on:
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio#timed-background-tasks
        /// </summary>
        /// <param name="openHimCoreClient"></param>
        /// <param name="mediatorConfigOptions"></param>
        public OpenHimHeartbeatService(IOpenHimCoreClient openHimCoreClient, IOptions<MediatorConfig> mediatorConfigOptions)
        {
            _openHimCoreClient = openHimCoreClient;
            _heartbeatIntervalSeconds = mediatorConfigOptions.Value.MediatorCore.HeartbeatInterval;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_openHimCoreClient.PingOpenHim, null, TimeSpan.FromSeconds(_heartbeatIntervalSeconds), TimeSpan.FromSeconds(_heartbeatIntervalSeconds));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }


        public void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
        }
    }
}
