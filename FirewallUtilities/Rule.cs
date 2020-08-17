#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FirewallUtilities
{
    public class Rule
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? DisplayGroup { get; set; } = string.Empty;
        public string? InstanceID { get; set; }

        public string[]? PolicyStoreSource { get; set; }

        public bool IsNew { get; set; } = false;

        public Enumerations.Action Action { get; set; } = Enumerations.Action.NotConfigured;
        public Enumerations.Enabled Enabled { get; set; } = Enumerations.Enabled.Enabled;

        public Enumerations.Direction Direction { get; set; } = Enumerations.Direction.Inbound;

        public Enumerations.EdgeTraversalPolicy EdgeTraversalPolicy { get; set; } =
            Enumerations.EdgeTraversalPolicy.Block;
        public Enumerations.PolicyStoreSourceType PolicyStoreSourceType { get; set; }
        public Enumerations.PrimaryStatus PrimaryStatus { get; set; } = Enumerations.PrimaryStatus.Unknown;
        public Enumerations.InterfaceType InterfaceType { get; set; } = Enumerations.InterfaceType.Any;
        public Enumerations.Profile Profiles { get; set; } = Enumerations.Profile.Any;

        //=====
        public string Program { get; set; } = string.Empty;

        public string LocalAddress { get; set; } = "Any";
        public string LocalPort { get; set; } = "Any";

        public string RemoteAddress { get; set; } = "Any";
        public string RemotePort { get; set; } = "Any";

        public Enumerations.Protocol Protocol { get; set; }

        public async Task GetAdditionalInfo()
        {
            Utilities util = new Utilities();
            await util.SetExecutionPolicy();
            var pObj = await util.GetNetFirewallPortFilter(this.DisplayName);
            var addObj = await util.GetNetFirewallAddressFilter(this.DisplayName);
            var aObj = await util.GetNetFirewallApplicationFilter(this.DisplayName);
            var interObj = await util.GetNetFirewallInterfaceTypeFilter(this.DisplayName);

            if (pObj.Count > 0)
            {
                LocalPort = (pObj[0].Properties["LocalPort"].Value != null)
                    ? string.Join(",", (string[])pObj[0].Properties["LocalPort"].Value)
                    : "Any";
                RemotePort = (pObj[0].Properties["RemotePort"].Value != null)
                    ? string.Join(",", (string[])pObj[0].Properties["RemotePort"].Value)
                    : "Any";
                if (pObj[0].Properties["Protocol"].Value != null)
                    Protocol = (pObj[0].Properties["Protocol"].Value != null)
                        ? Enum.Parse<Enumerations.Protocol>(pObj[0].Properties["Protocol"].Value.ToString())
                        : Enumerations.Protocol.Any;
            }
            if (addObj.Count > 0)
            {
                LocalAddress = (addObj[0].Properties["LocalAddress"].Value != null)
                    ? string.Join(",", (string[])addObj[0].Properties["LocalAddress"].Value)
                    : "Any";
                RemoteAddress = (addObj[0].Properties["RemoteAddress"].Value != null)
                    ? string.Join(",", (string[])addObj[0].Properties["RemoteAddress"].Value)
                    : "Any";
            }
            if (aObj.Count > 0)
            {
                Program = aObj[0].Properties["AppPath"].Value != null
                    ? aObj[0].Properties["AppPath"].Value.ToString()
                    : string.Empty;
            }

            if (interObj.Count > 0)
            {
                if (interObj[0].Properties["InterfaceType"].Value != null)
                    this.InterfaceType =
                        Enum.Parse<Enumerations.InterfaceType>(interObj[0].Properties["InterfaceType"].Value.ToString());

            }
        }

    }
}
