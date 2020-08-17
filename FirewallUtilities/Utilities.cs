using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FirewallUtilities
{
    public class Utilities
    {
        /// <summary>
        /// Allows local scripts to be run
        /// </summary>
        /// <returns>boolean success</returns>
        public async Task<bool> SetExecutionPolicy()
        {
            var result = await ExecScriptTask("Set-ExecutionPolicy RemoteSigned");
            return result.IsSuccess;
        }

        public async Task<(PSDataCollection<PSObject> psObjs, bool IsSuccess)> ExecScriptTask(string scriptText, Boolean IsDebug = false)
        {
            if (IsDebug)
            {
                return (new PSDataCollection<PSObject>(), true);
            }
            else
            {
                return await PowershellUtilities.RunScript(scriptText);
            }
        }

        #region Rules Methods

        public async Task<PSDataCollection<PSObject>> GetFirewallRule()
        {
            var script = "Get-NetFirewallRule";
            var result = await ExecScriptTask(script);
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetFirewallRule(string p_DisplayName)
        {
            var script = "Get-NetFirewallRule";
            if (p_DisplayName != null)
            {
                script += " -DisplayName '" + p_DisplayName + "'";
            }
            var result = await ExecScriptTask(script);
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallPortFilter(string p_DisplayName)
        {
            var result = await ExecScriptTask("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallPortFilter");
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallApplicationFilter(string p_DisplayName)
        {
            var result = await ExecScriptTask("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallApplicationFilter");
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallAddressFilter(string p_DisplayName)
        {
            var result = await ExecScriptTask("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallAddressFilter ");
            return result.psObjs;
        }
        public async Task<PSDataCollection<PSObject>> GetNetFirewallInterfaceTypeFilter(string p_DisplayName)
        {
            var result = await ExecScriptTask("Get-NetFirewallRule -DisplayName '" + p_DisplayName + "' | Get-NetFirewallInterfaceTypeFilter ");
            return result.psObjs;
        }

        public async Task EnableRule(Rule p_Rule)
        {
            await EnableRule(p_Rule.DisplayName);
        }
        public async Task EnableRule(string p_DisplayName)
        {
            await ExecScriptTask("Enable-NetFirewallRule -DisplayName '" + p_DisplayName + "'");
        }

        public async Task DisableRule(Rule p_Rule)
        {
            await DisableRule(p_Rule.DisplayName);
        }
        public async Task DisableRule(string p_DisplayName)
        {
            await ExecScriptTask("Disable-NetFirewallRule -DisplayName '" + p_DisplayName + "'");
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

        public async Task<Rule> ConvertToRule(PSObject obj)
        {
            return await ConvertToRule(obj, false);
        }
        public async Task<Rule> ConvertToRule(PSObject obj, bool p_GetadditonalInfo)
        {
            if (obj.Properties["CimClass"].Value.ToString().Split(':')[1] != "MSFT_NetFirewallRule")
            {
                throw new Exception("Object is not of type 'MSFT_NetFirewallRule'");
            }

            var rule = new Rule
            {
                DisplayName = obj.Properties["DisplayName"].Value.ToString(),
                DisplayGroup = (obj.Properties["DisplayGroup"].Value != null)
                    ? obj.Properties["DisplayGroup"].Value.ToString()
                    : String.Empty,
                InstanceID = (obj.Properties["InstanceID"].Value != null)
                    ? obj.Properties["InstanceID"].Value.ToString()
                    : String.Empty,
                Description = (obj.Properties["Description"].Value != null)
                    ? obj.Properties["Description"].Value.ToString()
                    : String.Empty,
                PolicyStoreSource = new String[]
                {
                    obj.Properties["PolicyStoreSource"].Value.ToString()
                },
                Direction = (Enumerations.Direction) Int32.Parse(obj.Properties["Direction"].Value.ToString()),
                Action = (Enumerations.Action) Int32.Parse(obj.Properties["Action"].Value.ToString()),
                EdgeTraversalPolicy = (Enumerations.EdgeTraversalPolicy) Int32.Parse(obj.Properties["EdgeTraversalPolicy"].Value.ToString()),
                Enabled = (Enumerations.Enabled) Int32.Parse(obj.Properties["Enabled"].Value.ToString()),
                PolicyStoreSourceType = (Enumerations.PolicyStoreSourceType) Int32.Parse(obj.Properties["PolicyStoreSourceType"].Value.ToString()),
                PrimaryStatus = (Enumerations.PrimaryStatus) Int32.Parse(obj.Properties["PrimaryStatus"].Value.ToString()),
                Profile = (Enumerations.Profile) Int32.Parse(obj.Properties["Profiles"].Value.ToString())
            };

            if (p_GetadditonalInfo)
            {
                await rule.GetAdditionalInfo();
            }

            return rule;
        }
        #endregion

        #region Profiles Methods
        public async Task<PSDataCollection<PSObject>> GetFirewallProfile()
        {
            var script = "Get-NetFirewallProfile";
            var result = await ExecScriptTask(script);
            return result.psObjs;
        }

        public async Task<PSDataCollection<PSObject>> GetFirewallProfile(string p_Name)
        {
            var script = "Get-NetFirewallProfile";
            if (p_Name != null)
            {
                script += " -Name '" + p_Name + "'";
            }
            var result = await ExecScriptTask(script);
            return result.psObjs;
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
        public string BuildProfileScript(ObservableCollection<Profile> ProfileCollection)
        {
            var pShellBld = new StringBuilder();
            foreach (var profile in ProfileCollection)
            {
                pShellBld.Append("Set-NetFirewallProfile -Profile '" + profile.Name + "'");
                pShellBld.Append(" ").Append("-Enabled " + (profile.Enabled == Enumerations.GpoBoolean.True).ToString());
                if (profile.LogFileName.Length > 0)
                    pShellBld.Append(" ").Append($@"-LogFileName '{profile.LogFileName.ToString()}'");
                pShellBld.Append(" ").Append($@"-DefaultInboundAction {profile.DefaultInboundAction.ToString()}");

                pShellBld.Append(" ").Append($@"-DefaultOutboundAction {profile.DefaultOutboundAction.ToString()}");
                pShellBld.Append(" ").Append($@"-NotifyOnListen {profile.NotifyOnListen.ToString()}");

                pShellBld.Append(" ").Append($@"-AllowInboundRules {profile.AllowInboundRules.ToString()}");
                pShellBld.Append(" ").Append($@"-AllowLocalFirewallRules {profile.AllowLocalFirewallRules.ToString()}");
                pShellBld.Append(" ").Append($@"-AllowLocalIPsecRules {profile.AllowLocalIPsecRules.ToString()}");
                pShellBld.Append(" ").Append($@"-AllowUserApps {profile.AllowUserApps.ToString()}");
                pShellBld.Append(" ").Append($@"-AllowUserPorts {profile.AllowUserPorts.ToString()}");
                pShellBld.Append(" ").Append($@"-EnableStealthModeForIPsec {profile.EnableStealthModeForIPsec.ToString()}");


                pShellBld.Append(" ").Append($@"-LogAllowed {profile.LogAllowed.ToString()}");
                pShellBld.Append(" ").Append($@"-LogBlocked {profile.LogBlocked.ToString()}");
                pShellBld.Append(" ").Append(value: $@"-LogIgnored {profile.LogIgnored.ToString()}");

                pShellBld.Append(";").Append(Environment.NewLine);
            }


            return pShellBld.ToString();
        }
        public Profile ConvertToProfile(PSObject obj)
        {
            if (obj.Properties["CimClass"].Value.ToString().Split(':')[1] != "MSFT_NetFirewallProfile")
            {
                throw new Exception("Object is not of type 'MSFT_NetFirewallProfile'");
            }

            var profile = new Profile()
            {
                Name = obj.Properties["Name"].Value.ToString(),
                LogFileName = obj.Properties["LogFileName"].Value != null
                        ? obj.Properties["LogFileName"].Value.ToString() 
                        : string.Empty,

                DisabledInterfaceAliases = string.Join(",", (string[])obj.Properties["DisabledInterfaceAliases"].Value),

                DefaultInboundAction = (Enumerations.Action)Int32.Parse(obj.Properties["DefaultInboundAction"].Value.ToString()),
                DefaultOutboundAction = (Enumerations.Action)Int32.Parse(obj.Properties["DefaultOutboundAction"].Value.ToString()),

                AllowLocalFirewallRules = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["AllowLocalFirewallRules"].Value.ToString()),
                AllowLocalIPsecRules = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["AllowLocalIPsecRules"].Value.ToString()),
                AllowUserApps = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["AllowUserApps"].Value.ToString()),
                AllowUserPorts = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["AllowUserPorts"].Value.ToString()),
                AllowInboundRules = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["DefaultOutboundAction"].Value.ToString()),
                EnableStealthModeForIPsec = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["EnableStealthModeForIPsec"].Value.ToString()),
                Enabled = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["Enabled"].Value.ToString()),
                LogAllowed = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["LogAllowed"].Value.ToString()),
                LogBlocked = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["LogBlocked"].Value.ToString()),
                LogIgnored = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["LogIgnored"].Value.ToString()),
                NotifyOnListen = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["NotifyOnListen"].Value.ToString())
        };



            return profile;
        }
        #endregion

        #region Settings Methods
        public async Task<PSDataCollection<PSObject>> GetNetFirewallSetting()
        {
            var result = await ExecScriptTask("Get-NetFirewallSetting");
            return result.psObjs;
        }

        public Setting ConvertToSetting(PSObject obj)
        {
            if (obj.Properties["CimClass"].Value.ToString().Split(':')[1] != "MSFT_NetSecuritySettingData")
            {
                throw new Exception("Object is not of type 'MSFT_NetSecuritySettingData'");
            }
            var setting = new Setting();

            setting.Name = obj.Properties["ElementName"].Value.ToString();
            setting.MaxSAIdleTimeSeconds = UInt32.Parse(obj.Properties["MaxSAIdleTimeSeconds"].Value.ToString());


            setting.AllowIPsecThroughNAT = (Enumerations.IPsecThroughNAT)Int32.Parse(obj.Properties["AllowIPsecThroughNAT"].Value.ToString());
            setting.CertValidationLevel = (Enumerations.CRLCheck)Int32.Parse(obj.Properties["CertValidationLevel"].Value.ToString());
            setting.Exemptions = (Enumerations.TrafficExemption)Int32.Parse(obj.Properties["Exemptions"].Value.ToString());
            setting.EnableStatefulFtp = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["EnableStatefulFtp"].Value.ToString());
            setting.EnableStatefulPptp = (Enumerations.GpoBoolean)Int32.Parse(obj.Properties["EnableStatefulPptp"].Value.ToString());
            setting.KeyEncoding = (Enumerations.KeyEncoding)Int32.Parse(obj.Properties["KeyEncoding"].Value.ToString());
            setting.EnablePacketQueuing = (Enumerations.PacketQueuing)Int32.Parse(obj.Properties["EnablePacketQueuing"].Value.ToString());

            setting.RemoteMachineTransportAuthorizationList = obj.Properties["RemoteMachineTransportAuthorizationList"].Value.ToString();
            setting.RemoteMachineTunnelAuthorizationList = obj.Properties["RemoteMachineTunnelAuthorizationList"].Value.ToString();
            setting.RemoteUserTransportAuthorizationList = obj.Properties["RemoteUserTransportAuthorizationList"].Value.ToString();
            setting.RemoteUserTunnelAuthorizationList = obj.Properties["RemoteUserTunnelAuthorizationList"].Value.ToString();
            setting.RequireFullAuthSupport = obj.Properties["RequireFullAuthSupport"].Value.ToString();


            return setting;
        }
        #endregion


    }
}
