using System.Text.Json.Serialization;

namespace OpenHim.Mediator.HL7Validator.Configuration
{
    public class HL7Config
    {
        [JsonPropertyName("application")]
        public string Application { get; set; }
        [JsonPropertyName("facility")]
        public string Facility { get; set; }
    }
}
