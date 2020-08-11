using System;
using System.Collections.Generic;
using System.Text;

namespace FirewallUtilities
{
    public class Profile
    {
        public string Name { get; set; }
        public string LogFileName { get; set; } = string.Empty;
        public string PolicyStore { get; set; } = string.Empty;
        public string DisabledInterfaceAliases { get; set; }
        public Enumerations.GpoBoolen AllowInboundRules { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen AllowLocalFirewallRules { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen AllowLocalIPsecRules { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen AllowUserApps { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen AllowUserPorts { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.Action DefaultInboundAction { get; set; } = Enumerations.Action.NotConfigured;
        public Enumerations.Action DefaultOutboundAction { get; set; } = Enumerations.Action.NotConfigured;
        public Enumerations.GpoBoolen EnableStealthModeForIPsec { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen Enabled { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen LogAllowed { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen LogBlocked { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen LogIgnored { get; set; } = Enumerations.GpoBoolen.NotConfigured;
        public Enumerations.GpoBoolen NotifyOnListen { get; set; } = Enumerations.GpoBoolen.NotConfigured;
    }
}
