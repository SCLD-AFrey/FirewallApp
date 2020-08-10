using System;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;
using System.Drawing;
using FirewallApp.Views;
using FirewallEngine;


namespace FirewallApp.ViewModels
{
    [MetadataType(typeof(MetaData))]
    public class RulesViewModel
    {
        public class MetaData : IMetadataProvider<RulesViewModel>
        {
            void IMetadataProvider<RulesViewModel>.BuildMetadata
                (MetadataBuilder<RulesViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.OnBuildPsScriptCommand()).CommandName("BuildPsScriptCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnRunPsScriptCommand()).CommandName("RunPsScriptCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnCreateNewRuleCommand()).CommandName("CreateNewRuleCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnViewPsNetFirewallRuleCommand()).CommandName("ViewPsNetFirewallRuleCommand");

                p_builder.CommandFromMethod(p_x => p_x.OnViewPsPortFilterCommand()).CommandName("ViewPsPortFilterCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnViewPsAddressFilterCommand()).CommandName("ViewPsAddressFilterCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnViewPsApplicationFilterCommand()).CommandName("ViewPsApplicationFilterCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnViewPsInterfaceTypeFilterCommand()).CommandName("ViewPsInterfaceTypeFilterCommand");
                p_builder.Property(p_x => p_x.SelectedRule).OnPropertyChangedCall(p_x => p_x.OnSelectedRuleChangedCommand());
            }
        }

        #region Constructors

        protected RulesViewModel()
        {
            PowerShellScript = string.Empty;
            fUtils = new FirewallUtilities();
            GetRules();

        }

        public static RulesViewModel Create()
        {
            return ViewModelSource.Create(() => new RulesViewModel());
        }

        #endregion

        #region Fields and Properties


        public virtual FirewallUtilities fUtils { get; set; }
        public virtual ObservableCollection<Rule> RuleCollection { get; set; }
        public virtual Rule SelectedRule { get; set; }
        public virtual string PowerShellScript { get; set; }
        public virtual bool IsEditEnabled { get; set; } = false;
        public virtual string PropertyHeader { get; set; } = string.Empty;

        #endregion

        #region Methods

        private async void GetRules()
        {
            IsEditEnabled = false;
            await fUtils.SetExecutionPolicy();
            var resultObjs = await fUtils.GetFirewallRule();
            RuleCollection = await fUtils.CreateRuleCollection(resultObjs);
            IsEditEnabled = true;
        }
        public void OnBuildPsScriptCommand()
        {
            IsEditEnabled = false;
            PowerShellScript = fUtils.BuildRuleScript(SelectedRule);
            IsEditEnabled = true;
        }

        public async void OnRunPsScriptCommand()
        {
            IsEditEnabled = false;
            await fUtils.SetExecutionPolicy();
            var result = await PowershellTools.RunScript(PowerShellScript);
            PowerShellScript = "Script Completed - " + result.IsSuccess.ToString();
            
            GetRules();
            IsEditEnabled = true;
        }

        public async void OnSelectedRuleChangedCommand()
        {
            IsEditEnabled = false;
            PowerShellScript = String.Empty;
            await SelectedRule.GetAdditionalInfo();
            PropertyHeader = SelectedRule.DisplayName;
            IsEditEnabled = true;
        }

        public void OnCreateNewRuleCommand()
        {
            IsEditEnabled = false;
            SelectedRule = new Rule(){IsNew = true};
            PropertyHeader = "New Rule";
            IsEditEnabled = true;
        }

        public async void OnViewPsNetFirewallRuleCommand()
        {
            var result = await fUtils.GetFirewallRule(SelectedRule.DisplayName);
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;



            newWin.Show();
        }


        public async void OnViewPsPortFilterCommand()
        {
            var result = await fUtils.GetNetFirewallPortFilter(SelectedRule.DisplayName);
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;
            newWin.Show();
        }

        public async void OnViewPsAddressFilterCommand()
        {
            var result = await fUtils.GetNetFirewallAddressFilter(SelectedRule.DisplayName);
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;
            newWin.Show();
        }

        public async void OnViewPsApplicationFilterCommand()
        {
            var result = await fUtils.GetNetFirewallApplicationFilter(SelectedRule.DisplayName);
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;
            newWin.Show();
        }
        public async void OnViewPsInterfaceTypeFilterCommand()
        {
            var result = await fUtils.GetNetFirewallInterfaceTypeFilter(SelectedRule.DisplayName);
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;
            newWin.Show();
        }
        #endregion
    }
}