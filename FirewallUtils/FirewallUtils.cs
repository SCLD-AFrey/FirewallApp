using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.IO;

namespace FirewallUtils
{
    public class FirewallUtils
    {
        public async Task<bool> SetExecutionPolicy()
        {
            var result = await RunPsTask("Set-ExecutionPolicy RemoteSigned");
            return result.IsSuccess;
        }

        public async Task<Collection<Rule>> GetAllRules()
        {
            var result = await RunPsTask("Get-NetFirewallRule");
            return GetRuleCollection(result.psObjs);
        }

        public async Task SetEnabled(string p_DisplayName, bool p_enabled)
        {
            if (p_enabled)
            {
                await EnableRule(p_DisplayName);
            }
            else
            {
                await DisableRule(p_DisplayName);
            }
        }

        public async Task<bool> EnableRule(string p_DisplayName)
        {
            var result = await RunPsTask(
                "Enable-NetFirewallRule -DisplayName '" + p_DisplayName + "'"
            );
            return result.IsSuccess;
        }

        public async Task<bool> DisableRule(string p_DisplayName)
        {
            var result = await RunPsTask(
                "Disable-NetFirewallRule -DisplayName '" + p_DisplayName + "'"
            );
            return result.IsSuccess;
        }

        public async Task<Collection<Rule>> GetNetPortFilter(string p_DisplayName)
        {
            var result = await RunPsTask("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallPortFilter");

            foreach (var obj in result.psObjs)
            {
                foreach (var p in obj.Properties)
                {
                    if(p.Value != null)
                        Debug.WriteLine(p.Name + ": " + p.Value.ToString());
                    if (p.Name == "LocalPort")
                    {
                        var _val = (string[]) p.Value;
                        foreach (var v in _val)
                        {
                            Debug.WriteLine("----------------- " + v.ToString());
                        }
                    }
                }
            }




            return GetRuleCollection(result.psObjs);
        }
        
        ///---------------------------------------------
        ///
        ///
        ///
        ///
        ///
        /// 

        private Collection<Rule> GetRuleCollection(Collection<PSObject> p_psObjects)
        {
            var fCollection = new Collection<Rule>();

            foreach (var obj in p_psObjects)
            {
                var props = obj.Properties;
                var rule = new Rule()
                {

                    DisplayName = props["DisplayName"].Value.ToString(),
                    Description = (props["Description"].Value != null) ? props["Description"].Value.ToString() : String.Empty
                };


                rule.Direction = (Direction) Enum.Parse<Direction>(props["Direction"].Value.ToString());
                rule.Enabled = (ushort.Parse(props["Enabled"].Value.ToString()) == 1);

                fCollection.Add(rule);



            }

            return fCollection;
        }

        private async Task<(Collection<PSObject> psObjs, bool IsSuccess)> RunPsTask(string script)
        {
            var resultObjs = new Collection<PSObject>();
            try
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript(script);
                    Console.WriteLine(script);
                    Debug.WriteLine(script);
                    var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);

                    foreach (var obj in pipelineObjects)
                    {
                        resultObjs.Add(obj);
                    }

                    return (resultObjs, true);
                }
            }
            catch (Exception e)
            {

                return (resultObjs, false);
            }

        }
    }
}
