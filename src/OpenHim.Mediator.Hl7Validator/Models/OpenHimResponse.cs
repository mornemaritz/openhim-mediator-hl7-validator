using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenHim.Mediator.Hl7Validator.Models
{
    public class OpenHimResponse
    {
        [JsonPropertyName("x-mediator-urn")]
        public string MediatorUrn { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("response")]
        public Response Response { get; set; }

        [JsonPropertyName("orchestrations"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Orchestration> Orchestrations { get; set; } = new List<Orchestration>();

        [JsonPropertyName("properties"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Error Error { get; set; }

        public void AddOrchestration(Orchestration orchestration)
        {
            if (Orchestrations == null) Orchestrations = new List<Orchestration>();

            Orchestrations.Add(orchestration);
        }
    }

    public class Response
    {
        [JsonPropertyName("status")]
        public short Status { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }

    public class Error
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("stack")]
        public string Stack { get; set; }
    }

    public class Orchestration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("request")]
        public Request Request { get; set; }

        [JsonPropertyName("response")]
        public Response Response { get; set; }
    }

    public class Request
    {
        [JsonPropertyName("host")]
        public string Host { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("querystring")]
        public string Querystring { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}
