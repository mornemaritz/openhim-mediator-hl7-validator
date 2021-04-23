using WcPhdc.OpenHim.Mediator.Models;
using System.Threading.Tasks;

namespace WcPhdc.OpenHim.Mediator.Services
{
    public interface IOpenHimOrchestrator
    {
        Task<OpenHimResponse> Do(string requestContent, Response primaryOpenHimConsumerResponse, bool primaryOperationSuccessful = true);
    }
}