using OpenHim.Mediator.Hl7Validator.Models;
using System.Threading.Tasks;

namespace OpenHim.Mediator.Hl7Validator.Services
{
    public interface IOpenHimResponseGenerator
    {
        Task<OpenHimResponse> PrimaryResponse(string body);
    }
}
