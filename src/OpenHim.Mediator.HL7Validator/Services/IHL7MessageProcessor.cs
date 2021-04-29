using System.Threading.Tasks;

namespace OpenHim.Mediator.HL7Validator.Services
{
    public interface IHL7MessageProcessor
    {
        Task<string> ParseAndReturnEncodedAck(string hl7Message, string parseError = default);
    }
}
