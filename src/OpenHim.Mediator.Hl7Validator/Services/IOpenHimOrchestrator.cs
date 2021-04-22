using OpenHim.Mediator.Hl7Validator.Models;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Services
{
    public interface IOpenHimOrchestrator
    {
        Task<OpenHimResponse> Do(string requestContent, Response primaryResponse, bool primaryOperationSuccessful = true);
    }
}