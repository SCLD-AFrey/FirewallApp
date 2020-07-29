using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DevExpress.CodeParser;
using FirewallApp.Data;
using FirewallEngine;

namespace FirewallApp.ViewModels
{
    [MetadataType(typeof(MetaData))]
    public class MainViewModel
    {
        public class MetaData : IMetadataProvider<MainViewModel>
        {
            void IMetadataProvider<MainViewModel>.BuildMetadata
                (MetadataBuilder<MainViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.OnLoadRulesCommand()).CommandName("LoadRulesCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnCommitRuleCommand()).CommandName("CommitRuleCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnEnableRuleCommand()).CommandName("EnableRuleCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnDisableRuleCommand()).CommandName("DisableRuleCommand");
                p_builder.Property(p_x => p_x.SelectedRule).OnPropertyChangedCall(p_x => p_x.OnSelectedRuleChangedCommand());
            }
        }

        #region Constructors

        protected MainViewModel()
        {
            uow = new UnitOfWork();
            fUtilities = new FirewallUtilities();
            FirewallRuleCollection = new XPCollection<FirewallRule>(uow);

            OnLoadRulesCommand();
        }

        public static MainViewModel Create()
        {
            return ViewModelSource.Create(() => new MainViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual FirewallEngine.FirewallUtilities fUtilities { get; set; }
        public virtual UnitOfWork uow { get; set; }
        public virtual FirewallRule SelectedRule { get; set; }
        public virtual XPCollection<FirewallRule> FirewallRuleCollection { get; set; }

        #endregion

        #region Methods

        public async void OnLoadRulesCommand()
        {
            FirewallRuleCollection = new XPCollection<FirewallRule>(uow);
            await fUtilities.SetExecutionPolicy();
            var result = await fUtilities.GetFirewallRule();

            var RuleCollection = fUtilities.ConvertRuleCollection(result);
            
            foreach (var rule in RuleCollection)
            {
                FirewallRuleCollection.Add(new FirewallRule(uow)
                {
                    DisplayName = rule.DisplayName,
                    Direction = rule.Direction.ToString(),
                    Enabled = rule.Enabled.ToString()
                });
            }
        }

        public async void OnCommitRuleCommand()
        {
            //await fUtilities.SetEnabled(
            //    SelectedRule.DisplayName, 
            //    SelectedRule.Enabled
            //    );

        }

        public async void OnSelectedRuleChangedCommand()
        {

            var portInfo = await fUtilities.GetNetFirewallPortFilter(SelectedRule.DisplayName);
            var appInfo = await fUtilities.GetNetFirewallApplicationFilter(SelectedRule.DisplayName);
            var addressInfo = await fUtilities.GetNetFirewallAddressFilter(SelectedRule.DisplayName);

            if(appInfo[0].Properties["AppPath"].Value != null)
                SelectedRule.Program = appInfo[0].Properties["AppPath"].Value.ToString();

            if(portInfo[0].Properties["LocalPort"].Value != null)
                SelectedRule.LocalPort = string.Join(',', (string?[])portInfo[0].Properties["LocalPort"].Value);
            if (addressInfo[0].Properties["LocalAddress"].Value != null)
                SelectedRule.LocalAddress = string.Join(',', (string?[])addressInfo[0].Properties["LocalAddress"].Value);
            if (portInfo[0].Properties["RemotePort"].Value != null)
                SelectedRule.RemotePort = string.Join(',', (string?[])portInfo[0].Properties["RemotePort"].Value);
            if (addressInfo[0].Properties["RemoteAddress"].Value != null)
                SelectedRule.RemoteAddress = string.Join(',', (string?[])addressInfo[0].Properties["RemoteAddress"].Value);

            var x = await fUtilities.TryThis();
        }

        public async void OnEnableRuleCommand()
        {
            SelectedRule.Enabled = FirewallEngine.Enumerations.Enabled.Enabled.ToString();
            await fUtilities.EnableRule(this.SelectedRule.DisplayName);
            OnLoadRulesCommand();
        }

        public async void OnDisableRuleCommand()
        {
            SelectedRule.Enabled = FirewallEngine.Enumerations.Enabled.Disabled.ToString();
            await fUtilities.DisableRule(this.SelectedRule.DisplayName);
            OnLoadRulesCommand();
        }


        #endregion
    }
}