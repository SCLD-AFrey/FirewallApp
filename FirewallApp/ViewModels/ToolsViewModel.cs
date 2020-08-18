using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo;
using FirewallApp.Data;
using FirewallUtilities;

namespace FirewallApp.ViewModels
{
    [MetadataType(typeof(MetaData))]
    public class ToolsViewModel
    {
        public class MetaData : IMetadataProvider<ToolsViewModel>
        {
            void IMetadataProvider<ToolsViewModel>.BuildMetadata
                (MetadataBuilder<ToolsViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.OnInitializeRuleModelsCommand()).CommandName("InitializeRuleModelsCommand");
            }
        }

        #region Constructors

        protected ToolsViewModel()
        {
            FirewallUtils = new FirewallUtilities.Utilities();
            uow = new UnitOfWork();
            RuleModelCollection = new XPCollection<RuleModel>(uow);
        }

        public static ToolsViewModel Create()
        {
            return ViewModelSource.Create(() => new ToolsViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual UnitOfWork uow { get; set; }
        public virtual Utilities FirewallUtils { get; set; }
        public virtual XPCollection<RuleModel> RuleModelCollection { get; set; }

        #endregion

        #region Methods

        public async void OnInitializeRuleModelsCommand()
        {
            await Utilities.SetExecutionPolicy();
            var RulesObjs = await Utilities.GetFirewallRule();

            foreach (var ruleObj in RulesObjs)
            {
                var rule = Utilities.ConvertToRule(ruleObj, false);

            }
        }

        #endregion
    }
}