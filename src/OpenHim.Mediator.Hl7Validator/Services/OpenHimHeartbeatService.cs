using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenHim.Mediator.Hl7Validator.Configuration;
using OpenHim.Mediator.Hl7Validator.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Services
{
    public class OpenHimHeartbeatService : IHostedService, IAsyncDisposable
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

        public async ValueTask DisposeAsync()
        {
            if(_timer != null)
                await _timer.DisposeAsync();
        }
    }
}
