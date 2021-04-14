using System.Threading;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Net
{
    public interface IOpenHimCoreClient
    {
        Task RegisterAsync(CancellationToken cancellationToken);
        void PingOpenHim(object state);
    }
}