using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WcPhdc.OpenHim.Mediator.Net;

namespace WcPhdc.OpenHim.Mediator.Services
{
    public class OpenHimRegistrationService : IHostedService
    {
        private readonly IOpenHimCoreClient _openHimCoreClient;

        public OpenHimRegistrationService(IOpenHimCoreClient openHimCoreClient)
        {
            _openHimCoreClient = openHimCoreClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken) => 
            await _openHimCoreClient.RegisterAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
