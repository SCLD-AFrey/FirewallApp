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
        public Enumerations.GpoBoolean AllowInboundRules { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean AllowLocalFirewallRules { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean AllowLocalIPsecRules { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean AllowUserApps { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean AllowUserPorts { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.Action DefaultInboundAction { get; set; } = Enumerations.Action.NotConfigured;
        public Enumerations.Action DefaultOutboundAction { get; set; } = Enumerations.Action.NotConfigured;
        public Enumerations.GpoBoolean EnableStealthModeForIPsec { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean Enabled { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean LogAllowed { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean LogBlocked { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean LogIgnored { get; set; } = Enumerations.GpoBoolean.NotConfigured;
        public Enumerations.GpoBoolean NotifyOnListen { get; set; } = Enumerations.GpoBoolean.NotConfigured;
    }
}
