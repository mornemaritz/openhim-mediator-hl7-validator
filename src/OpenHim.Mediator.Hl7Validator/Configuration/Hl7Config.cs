using System.Text.Json.Serialization;

namespace OpenHim.Mediator.Hl7Validator.Configuration
{
    public class Hl7Config
    {
        [JsonPropertyName("application")]
        public string Application { get; set; }
        [JsonPropertyName("facility")]
        public string Facility { get; set; }
    }
}
