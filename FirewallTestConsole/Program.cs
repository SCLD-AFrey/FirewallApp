using System;
using FirewallEngine;
using System.Threading.Tasks;

namespace FirewallTestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FirewallUtilities util =  new FirewallUtilities();


            await util.SetExecutionPolicy();

            var rule_objs = await util.GetFirewallRule("-- Test Rule Inbound");
            
            var rule_obj = rule_objs[0];

            var rule = await util.ConvertToRule(rule_obj);

            await rule.GetAdditionalInfo();

        }
    }
}
