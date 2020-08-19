using System;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using FirewallApp.Views;
using FirewallUtilities;
using DevExpress.Mvvm;

namespace FirewallApp.ViewModels
{
    [MetadataType(typeof(MetaData))]
    public class RulesViewModel : ViewModelBase
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
            FirewallUtils = new FirewallUtilities.Utilities();
            GetRules();
        }

        public static RulesViewModel Create()
        {
            return ViewModelSource.Create(() => new RulesViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual FirewallUtilities.Utilities FirewallUtils { get; set; }
        public virtual ObservableCollection<Rule> RuleCollection { get; set; }
        public virtual Rule SelectedRule { get; set; }
        public virtual string PowerShellScript { get; set; }
        public virtual bool IsEditEnabled { get; set; } = false;
        public virtual bool IsExistingRule { get; set; } = true;
        public virtual string PropertyHeader { get; set; } = string.Empty;
        
        #endregion

        #region Methods

        private async void GetRules()
        {
            IsEditEnabled = false;
            await Utilities.SetExecutionPolicy();
            var resultObjs = await Utilities.GetFirewallRule();
            RuleCollection = await Utilities.CreateRuleCollection(resultObjs);
            IsEditEnabled = true;
        }
        public void OnBuildPsScriptCommand()
        {
            IsEditEnabled = false;
            PowerShellScript = SelectedRule.BuildScript;
            IsEditEnabled = true;
        }

        public async void OnRunPsScriptCommand()
        {
            IsEditEnabled = false;
            await Utilities.SetExecutionPolicy();


            var check = RuleCollection.Where(x => x.DisplayName == SelectedRule.DisplayName);
            var intCheck = check.Count();

            if (intCheck == 0)
            {
                var result = await SelectedRule.Commit(true);
                PowerShellScript = "Script Completed - " + result.Item1.ToString();
                GetRules();
            }
            else
            {
                PowerShellScript = "Script Halted - Rule '" + SelectedRule.DisplayName + "' already exists";
            }

            IsEditEnabled = true;
        }

        public async void OnSelectedRuleChangedCommand()
        {
            IsEditEnabled = false;
            PowerShellScript = String.Empty;
            await SelectedRule.GetAdditionalInfo();

            RaisePropertyChanged(nameof(SelectedRule));
            
            PropertyHeader = SelectedRule.DisplayName;
            IsExistingRule = !SelectedRule.IsNew;
            
            IsEditEnabled = true;
        }

        public void OnCreateNewRuleCommand()
        {
            IsEditEnabled = false;
            SelectedRule = new Rule(){IsNew = true};
            IsExistingRule = false;
            IsEditEnabled = true;
        }

        public async void OnViewPsNetFirewallRuleCommand()
        {
            await ShowObjectWindow("GetFirewallRule");
        }

        public async void OnViewPsPortFilterCommand()
        {
            await ShowObjectWindow("GetNetFirewallPortFilter");
        }

        public async void OnViewPsAddressFilterCommand()
        {
            await ShowObjectWindow("GetNetFirewallAddressFilter");
        }

        public async void OnViewPsApplicationFilterCommand()
        {
            await ShowObjectWindow("GetNetFirewallApplicationFilter");
        }
        public async void OnViewPsInterfaceTypeFilterCommand()
        {
            await ShowObjectWindow("GetNetFirewallInterfaceTypeFilter");
        }

        private async Task ShowObjectWindow(string psObjType)
        {
            PSDataCollection<PSObject> resultObjs = new PSDataCollection<PSObject>();
            switch (psObjType)
            {
                case "GetFirewallRule":
                    resultObjs = await Utilities.GetFirewallRule(SelectedRule.DisplayName);
                    break;
                case "GetNetFirewallPortFilter":
                    resultObjs = await Utilities.GetNetFirewallPortFilter(SelectedRule.DisplayName);
                    break;
                case "GetNetFirewallAddressFilter":
                    resultObjs = await Utilities.GetNetFirewallAddressFilter(SelectedRule.DisplayName);
                    break;
                case "GetNetFirewallApplicationFilter":
                    resultObjs = await Utilities.GetNetFirewallApplicationFilter(SelectedRule.DisplayName);
                    break;
                case "GetNetFirewallInterfaceTypeFilter":
                    resultObjs = await Utilities.GetNetFirewallInterfaceTypeFilter(SelectedRule.DisplayName);
                    break;
            }

            if (resultObjs.Count != 1)
            {
                throw new Exception("ShowObjectWindow has not returned a single object");
            }

            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = resultObjs[0];
            newWin.DataContext = obj;
            newWin.Title = $@"Object Properties: {SelectedRule.DisplayName} {resultObjs[0].Properties["CimClass"].Value.ToString().Split(':')[1]}";
            newWin.Show();
        }
        #endregion
    }
}