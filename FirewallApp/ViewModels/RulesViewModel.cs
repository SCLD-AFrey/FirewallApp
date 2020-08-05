using System;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using DevExpress.CodeParser;
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
                p_builder.Property(p_x => p_x.SelectedRule).OnPropertyChangedCall(p_x => p_x.OnSelectedRuleChangedCommand());
            }
        }

        #region Constructors

        protected RulesViewModel()
        {
            shellScript = string.Empty;
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
        public virtual string shellScript { get; set; }
        public virtual bool IsEditEnabled { get; set; } = false;

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
            shellScript = fUtils.BuildRuleScript(SelectedRule);
            IsEditEnabled = true;
        }

        public async void OnRunPsScriptCommand()
        {
            IsEditEnabled = false;
            await fUtils.SetExecutionPolicy();
            //var result = await PowershellTools.RunScript(OutputText);
            //OutputText = "Script Completed - " + result.IsSuccess.ToString();
            GetRules();
            IsEditEnabled = true;
        }

        public async void OnSelectedRuleChangedCommand()
        {
            IsEditEnabled = false;
            shellScript = String.Empty;
            await SelectedRule.GetAdditionalInfo();
            IsEditEnabled = true;
        }

        public void OnCreateNewRuleCommand()
        {
            IsEditEnabled = false;
            SelectedRule = new Rule(){IsNew = true};
            IsEditEnabled = true;
        }
        #endregion
    }
}