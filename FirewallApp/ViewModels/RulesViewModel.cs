using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                p_builder.CommandFromMethod(p_x => p_x.OnLoadRulesCommand()).CommandName("LoadRulesCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnCommitRuleCommand()).CommandName("CommitRuleCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnEnableRuleCommand()).CommandName("EnableRuleCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnDisableRuleCommand()).CommandName("DisableRuleCommand");
                p_builder.Property(p_x => p_x.SelectedRule).OnPropertyChangedCall(p_x => p_x.OnSelectedRuleChangedCommand());
            }
        }

        #region Constructors

        protected RulesViewModel()
        {
            uow = new UnitOfWork();
            fUtilities = new FirewallUtilities();
            RuleCollection = new ObservableCollection<Rule>();

            OnLoadRulesCommand();
        }

        public static RulesViewModel Create()
        {
            return ViewModelSource.Create(() => new RulesViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual FirewallUtilities fUtilities { get; set; }
        public virtual UnitOfWork uow { get; set; }
        public virtual Rule SelectedRule { get; set; }
        public virtual ObservableCollection<Rule> RuleCollection { get; set; }

        #endregion

        #region Methods
        public async void OnLoadRulesCommand()
        {
            await fUtilities.SetExecutionPolicy();
            var result = await fUtilities.GetFirewallRule();

            var RuleCollection = await fUtilities.CreateRuleCollection(result);


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
            await SelectedRule.GetAdditionalInfo();


        }

        public async void OnEnableRuleCommand()
        {
            SelectedRule.Enabled = Enumerations.Enabled.Enabled;
            await fUtilities.EnableRule(this.SelectedRule.DisplayName);
            OnLoadRulesCommand();
        }

        public async void OnDisableRuleCommand()
        {
            SelectedRule.Enabled = Enumerations.Enabled.Disabled;
            await fUtilities.DisableRule(this.SelectedRule.DisplayName);
            OnLoadRulesCommand();
        }

        #endregion
    }
}