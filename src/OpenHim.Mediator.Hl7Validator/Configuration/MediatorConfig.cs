namespace OpenHim.Mediator.Hl7Validator.Configuration
{
    public class MediatorConfig
    {
        public OpenHimAuth openHimAuth { get; set; }
        public MediatorCore mediatorCore { get; set; }
        public MediatorSetup mediatorSetup { get; set; }
    }

    public class OpenHimAuth
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool trustSelfSigned { get; set; }
    }

    public class MediatorCore
    {
        public string openHimCoreHost { get; set; }
        public string openHimCoreAuthPath { get; set; }
        public string openHimRegisterMediatorPath { get; set; }
        public string openHimheartbeatpath { get; set; }
        public int heartbeatInterval { get; set; }
        public bool isHeartbeatDisabled { get; set; }
    }

    public class MediatorSetup
    {
        public string urn { get; set; }
        public string version { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DefaultChannelConfig[] defaultChannelConfig { get; set; }
        public EndPoint[] endpoints { get; set; }
    }

    public class DefaultChannelConfig
    {
        public string name { get; set; }
        public string urlPattern { get; set; }
        public string type { get; set; }
        public Route[] routes { get; set; }
        public string[] allow { get; set; }
    }

    public class Route
    {
        public string name { get; set; }
        public string host { get; set; }
        public string port { get; set; }
        public bool primary { get; set; }
        public string type { get; set; }
        public string path { get; set; }
    }

    public class EndPoint
    {
        public string name { get; set; }
        public string host { get; set; }
        public string path { get; set; }
        public string port { get; set; }
        public bool primary { get; set; }
        public string type { get; set; }
    }
}
