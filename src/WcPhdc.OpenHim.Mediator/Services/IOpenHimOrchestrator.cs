using System.Threading.Tasks;
using WcPhdc.OpenHim.Mediator.Models;

namespace WcPhdc.OpenHim.Mediator.Services
{
    public interface IOpenHimOrchestrator
    {
        Task<OpenHimResponse> Do(string requestContent, Response primaryOpenHimConsumerResponse, bool primaryOperationSuccessful = true);
    }
}