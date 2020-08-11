using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace FirewallUtilities
{
    public class Utilities
    {

        public async Task<bool> SetExecutionPolicy()
        {
            var result = await PowershellUtilities.RunScript("Set-ExecutionPolicy RemoteSigned");
            return result.IsSuccess;
        }

        public async Task<(PSDataCollection<PSObject> psObjs, bool IsSuccess)> ExecScriptTask(string scriptText)
        {
            return await PowershellUtilities.RunScript(scriptText);
        }

        //--------------


        public async Task<PSDataCollection<PSObject>> GetFirewallProfile()
        {
            var script = "Get-NetFirewallProfile";
            var result = await PowershellUtilities.RunScript(script);
            return result.psObjs;
        }

        public async Task<PSDataCollection<PSObject>> GetFirewallProfile(string p_Name)
        {
            var script = "Get-NetFirewallProfile";
            if (p_Name != null)
            {
                script += " -Name '" + p_Name + "'";
            }
            var result = await PowershellUtilities.RunScript(script);
            return result.psObjs;
        }


        //--------------
        public async Task<PSDataCollection<PSObject>> GetFirewallRule()
        {
            var script = "Get-NetFirewallRule";
            var result = await PowershellUtilities.RunScript(script);
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetFirewallRule(string p_DisplayName)
        {
            var script = "Get-NetFirewallRule";
            if (p_DisplayName != null)
            {
                script += " -DisplayName '" + p_DisplayName + "'";
            }
            var result = await PowershellUtilities.RunScript(script);
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallPortFilter(string p_DisplayName)
        {
            var result = await PowershellUtilities.RunScript("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallPortFilter");
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallApplicationFilter(string p_DisplayName)
        {
            var result = await PowershellUtilities.RunScript("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallApplicationFilter");
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallAddressFilter(string p_DisplayName)
        {
            var result = await PowershellUtilities.RunScript("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallAddressFilter ");
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallInterfaceTypeFilter(string p_DisplayName)
        {
            var result = await PowershellUtilities.RunScript("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallInterfaceTypeFilter ");
            return result.psObjs;
        }

        public async Task EnableRule(string p_DisplayName)
        {
            await PowershellUtilities.RunScript("Enable-NetFirewallRule -DisplayName '" + p_DisplayName + "'");
        }

        public async Task DisableRule(string p_DisplayName)
        {
            await PowershellUtilities.RunScript("Disable-NetFirewallRule -DisplayName '" + p_DisplayName + "'");
        }


        public async Task<ObservableCollection<Rule>> CreateRuleCollection(PSDataCollection<PSObject> objs)
        {
            return await CreateRuleCollection(objs, false);
        }

        public async Task<ObservableCollection<Rule>> CreateRuleCollection(PSDataCollection<PSObject> objs, bool p_GetadditonalInfo)
        {
            var retObjs = new ObservableCollection<Rule>();

            foreach (var obj in objs)
            {
                retObjs.Add(await ConvertToRule(obj, p_GetadditonalInfo));
            }

            return retObjs;
        }

        public ObservableCollection<Profile> CreateProfileCollection(PSDataCollection<PSObject> objs)
        {

            var retObjs = new ObservableCollection<Profile>();

            foreach (var obj in objs)
            {
                retObjs.Add(ConvertToProfile(obj));
            }

            return retObjs;
        }



        public async Task<Rule> ConvertToRule(PSObject obj)
        {
            return await ConvertToRule(obj, false);
        }

        public string BuildRuleScript(Rule pRule)
        {
            var pShellBld = new StringBuilder();
            if (pRule.IsNew)
            {
                pShellBld.Append("New-NetFirewallRule -DisplayName '" + pRule.DisplayName + "'");
            }
            else
            {
                pShellBld.Append("Set-NetFirewallRule -DisplayName '" + pRule.DisplayName + "'");
            }
            pShellBld.Append(" ").Append("-Enabled " + (pRule.Enabled == Enumerations.Enabled.Enabled).ToString());
            pShellBld.Append(" ").Append(string.Format("-Action {0}", pRule.Action.ToString()));
            if (pRule.Description.Length > 0)
                pShellBld.Append(" ").Append(string.Format("-Description '{0}'", pRule.Description));
            if (pRule.DisplayGroup.Length > 0)
                pShellBld.Append(" ").Append(string.Format("-DisplayGroup {0}", pRule.DisplayGroup));

            pShellBld.Append(" ").Append(string.Format("-Direction {0}", pRule.Direction));
            if (pRule.LocalPort.Length > 0)
                pShellBld.Append(" ").Append(string.Format("-LocalPort {0}", pRule.LocalPort.ToString()));
            if (pRule.RemotePort.Length > 0)
                pShellBld.Append(" ").Append(string.Format("-RemotePort {0}", pRule.RemotePort.ToString()));
            if (pRule.LocalAddress.Length > 0)
                pShellBld.Append(" ").Append(string.Format("-LocalAddress {0}", pRule.LocalAddress.ToString()));
            if (pRule.RemoteAddress.Length > 0)
                pShellBld.Append(" ").Append(string.Format("-RemoteAddress {0}", pRule.RemoteAddress.ToString()));
            if (pRule.Protocol != null)
                pShellBld.Append(" ").Append(string.Format("-Protocol {0}", pRule.Protocol.ToString()));
            if (pRule.Program.Length > 0)
                pShellBld.Append(" ").Append(string.Format("-Program '{0}'", pRule.Program.ToString()));

            return pShellBld.ToString();
        }

        public string BuildProfileScript(ObservableCollection<Profile> ProfileCollection)
        {
            var pShellBld = new StringBuilder();
            foreach (var profile in ProfileCollection)
            {
                pShellBld.Append("Set-NetFirewallProfile -Profile '" + profile.Name + "'");
                pShellBld.Append(" ").Append("-Enabled " + (profile.Enabled == Enumerations.GpoBoolen.True).ToString());
                if (profile.LogFileName.Length > 0)
                    pShellBld.Append(" ").Append(string.Format("-LogFileName '{0}'", profile.LogFileName.ToString()));
                pShellBld.Append(" ").Append(string.Format("-DefaultInboundAction {0}", profile.DefaultInboundAction.ToString()));

                pShellBld.Append(" ").Append(string.Format("-DefaultOutboundAction {0}", profile.DefaultOutboundAction.ToString()));
                pShellBld.Append(" ").Append(string.Format("-NotifyOnListen {0}", profile.NotifyOnListen.ToString()));

                pShellBld.Append(" ").Append(string.Format("-AllowInboundRules {0}", profile.AllowInboundRules.ToString()));
                pShellBld.Append(" ").Append(string.Format("-AllowLocalFirewallRules {0}", profile.AllowLocalFirewallRules.ToString()));
                pShellBld.Append(" ").Append(string.Format("-AllowLocalIPsecRules {0}", profile.AllowLocalIPsecRules.ToString()));
                pShellBld.Append(" ").Append(string.Format("-AllowUserApps {0}", profile.AllowUserApps.ToString()));
                pShellBld.Append(" ").Append(string.Format("-AllowUserPorts {0}", profile.AllowUserPorts.ToString()));
                pShellBld.Append(" ").Append(string.Format("-EnableStealthModeForIPsec {0}", profile.EnableStealthModeForIPsec.ToString()));


                pShellBld.Append(" ").Append(string.Format("-LogAllowed {0}", profile.LogAllowed.ToString()));
                pShellBld.Append(" ").Append(string.Format("-LogBlocked {0}", profile.LogBlocked.ToString()));
                pShellBld.Append(" ").Append(string.Format("-LogIgnored {0}", profile.LogIgnored.ToString()));


                pShellBld.Append(";").Append(Environment.NewLine);
            }


            return pShellBld.ToString();
        }

        public async Task<Rule> ConvertToRule(PSObject obj, bool p_GetadditonalInfo)
        {
            var rule = new Rule();

            //rule.Name = obj.Properties["Name"].Value.ToString();
            rule.DisplayName = obj.Properties["DisplayName"].Value.ToString();
            rule.DisplayGroup = (obj.Properties["DisplayGroup"].Value != null)
                ? obj.Properties["DisplayGroup"].Value.ToString()
                : String.Empty;
            rule.InstanceID = (obj.Properties["InstanceID"].Value != null)
                ? obj.Properties["InstanceID"].Value.ToString()
                : String.Empty;
            rule.Description = (obj.Properties["Description"].Value != null)
                ? obj.Properties["Description"].Value.ToString()
                : String.Empty;
            rule.PolicyStoreSource = new String[] {
                obj.Properties["PolicyStoreSource"].Value.ToString()
            };

            rule.Direction = (Enumerations.Direction)Int32.Parse(obj.Properties["Direction"].Value.ToString());
            rule.Action = (Enumerations.Action)Int32.Parse(obj.Properties["Action"].Value.ToString());
            rule.EdgeTraversalPolicy = (Enumerations.EdgeTraversalPolicy)Int32.Parse(obj.Properties["EdgeTraversalPolicy"].Value.ToString());
            rule.Enabled = (Enumerations.Enabled)Int32.Parse(obj.Properties["Enabled"].Value.ToString());
            rule.PolicyStoreSourceType = (Enumerations.PolicyStoreSourceType)Int32.Parse(obj.Properties["PolicyStoreSourceType"].Value.ToString());
            rule.PrimaryStatus = (Enumerations.PrimaryStatus)Int32.Parse(obj.Properties["PrimaryStatus"].Value.ToString());

            if (p_GetadditonalInfo)
            {
                await rule.GetAdditionalInfo();
            }

            return rule;
        }

        public Profile ConvertToProfile(PSObject obj)
        {
            var profile = new Profile();


            profile.Name = obj.Properties["Name"].Value.ToString();
            if (obj.Properties["LogFileName"].Value != null)
                profile.LogFileName = obj.Properties["LogFileName"].Value.ToString();

            profile.DisabledInterfaceAliases = string.Join(",", (string[])obj.Properties["DisabledInterfaceAliases"].Value);

            profile.DefaultInboundAction = (Enumerations.Action)Int32.Parse(obj.Properties["DefaultInboundAction"].Value.ToString());
            profile.DefaultOutboundAction = (Enumerations.Action)Int32.Parse(obj.Properties["DefaultOutboundAction"].Value.ToString());

            profile.AllowLocalFirewallRules = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["AllowLocalFirewallRules"].Value.ToString());
            profile.AllowLocalIPsecRules = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["AllowLocalIPsecRules"].Value.ToString());
            profile.AllowUserApps = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["AllowUserApps"].Value.ToString());
            profile.AllowUserPorts = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["AllowUserPorts"].Value.ToString());
            profile.AllowInboundRules = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["DefaultOutboundAction"].Value.ToString());
            profile.EnableStealthModeForIPsec = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["EnableStealthModeForIPsec"].Value.ToString());
            profile.Enabled = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["Enabled"].Value.ToString());
            profile.LogAllowed = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["LogAllowed"].Value.ToString());
            profile.LogBlocked = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["LogBlocked"].Value.ToString());
            profile.LogIgnored = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["LogIgnored"].Value.ToString());
            profile.NotifyOnListen = (Enumerations.GpoBoolen)Int32.Parse(obj.Properties["NotifyOnListen"].Value.ToString());

            return profile;
        }


    }
}
