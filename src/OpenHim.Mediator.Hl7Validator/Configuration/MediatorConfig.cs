using OpenHim.Mediator.Hl7Validator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenHim.Mediator.Hl7Validator.Configuration
{
    public class MediatorConfig
    {
        [JsonPropertyName("openHimAuth")]
        public OpenHimAuth OpenHimAuth { get; set; }

        [JsonPropertyName("mediatorCore")]
        public MediatorCore MediatorCore { get; set; }

        [JsonPropertyName("mediatorSetup")]
        public MediatorSetup MediatorSetup { get; set; }

        [JsonPropertyName("orchestrations"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Orchestration> Orchestrations { get; set; } = new List<Orchestration>();

        public bool HasOrchestrations()
        {
            return Orchestrations != null && Orchestrations.Any();
        }
    }

    public class OpenHimAuth
    {
        [JsonPropertyName("coreUsername")]
        public string CoreUsername { get; set; }

        [JsonPropertyName("corePassword")]
        public string CorePassword { get; set; }

        [JsonPropertyName("apiClientName")]
        public string ApiClientName { get; set; }

        [JsonPropertyName("apiClientPassword")]
        public string ApiClientPassword { get; set; }

        [JsonPropertyName("trustSelfSigned")]
        public bool TrustSelfSigned { get; set; }

        [JsonPropertyName("ignoreOutgoingOpenHimAuthFailures")]
        public bool IgnoreOutgoingOpenHimAuthFailures { get; set; }
    }

    public class MediatorCore
    {

        [JsonPropertyName("openHimCoreHost")]
        public string OpenHimCoreHost { get; set; }

        [JsonPropertyName("openHimCoreAuthPath")]
        public string OpenHimCoreAuthPath { get; set; }

        [JsonPropertyName("openHimRegisterMediatorPath")]
        public string OpenHimRegisterMediatorPath { get; set; }

        [JsonPropertyName("openHimHeartbeatpath")]
        public string OpenHimHeartbeatPath { get; set; }

        [JsonPropertyName("heartbeatInterval")]
        public int HeartbeatInterval { get; set; }

        [JsonPropertyName("isHeartbeatDisabled")]
        public bool IsHeartbeatDisabled { get; set; }
    }

    public class MediatorSetup
    {
        [JsonPropertyName("urn")]
        public string Urn { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("defaultChannelConfig")]
        public ChannelConfig[] DefaultChannelConfig { get; set; }

        [JsonPropertyName("endpoints")]
        public Location[] Endpoints { get; set; }
    }

    public class ChannelConfig
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("urlPattern")]
        public string UrlPattern { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("routes")]
        public Location[] Routes { get; set; }

        [JsonPropertyName("allow")]
        public string[] Allow { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("host")]
        public string Host { get; set; }

        [JsonPropertyName("port")]
        public string Port { get; set; }

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}
