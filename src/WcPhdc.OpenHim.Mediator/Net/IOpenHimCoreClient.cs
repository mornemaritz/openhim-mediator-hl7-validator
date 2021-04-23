using System.Threading;
using System.Threading.Tasks;

namespace WcPhdc.OpenHim.Mediator.Net
{
    public interface IOpenHimCoreClient
    {
        Task RegisterAsync(CancellationToken cancellationToken);
        void PingOpenHim(object state);
    }
}