using Microsoft.Extensions.Hosting;
using OpenHim.Mediator.Hl7Validator.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Services
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
