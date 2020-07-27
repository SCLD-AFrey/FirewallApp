using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DevExpress.CodeParser;
using FirewallUtils;
using FirewallApp.Data;

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
                p_builder.Property(p_x => p_x.SelectedRule).OnPropertyChangedCall(p_x => p_x.OnSelectedRuleChangedCommand());
            }
        }

        #region Constructors

        protected MainViewModel()
        {
            uow = new UnitOfWork();
            fUtilities = new FirewallUtils.FirewallUtils();
            FirewallRuleCollection = new XPCollection<FirewallRule>(uow);

            OnLoadRulesCommand();
        }

        public static MainViewModel Create()
        {
            return ViewModelSource.Create(() => new MainViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual FirewallUtils.FirewallUtils fUtilities { get; set; }
        public virtual UnitOfWork uow { get; set; }
        public virtual FirewallRule SelectedRule { get; set; }
        public virtual XPCollection<FirewallRule> FirewallRuleCollection { get; set; }

        #endregion

        #region Methods

        public async void OnLoadRulesCommand()
        {
            FirewallRuleCollection = new XPCollection<FirewallRule>(uow);
            await fUtilities.SetExecutionPolicy();
            var result = await fUtilities.GetAllRules();
            
            foreach (var rule in result)
            {
                FirewallRuleCollection.Add(new FirewallRule(uow)
                {
                    ElementName = rule.ElementName,
                    DisplayName = rule.DisplayName,
                    Direction = rule.Direction.ToString(),
                    Enabled = rule.Enabled
                });
            }
        }

        public async void OnCommitRuleCommand()
        {
            await fUtilities.SetEnabled(
                SelectedRule.DisplayName, 
                SelectedRule.Enabled
                );
        }

        public void OnSelectedRuleChangedCommand()
        {

        }


        #endregion
    }
}