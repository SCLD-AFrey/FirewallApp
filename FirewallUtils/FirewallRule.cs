using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FirewallUtils
{
    public class Rule
    {
        public string ElementName = String.Empty;
        public string DisplayName = String.Empty;
        public string Description = String.Empty;
        public Direction Direction;
        public bool Enabled;

        FirewallUtils utils = new FirewallUtils();


        public async Task Enable()
        {
            await utils.EnableRule(this.DisplayName);
        }
        public async Task Disable()
        {
            await utils.DisableRule(this.DisplayName);
        }
    }

    public enum Direction
    {
        Inbound = 1, 
        Outboud = 2
    }

    //------------------- METHODS


}
