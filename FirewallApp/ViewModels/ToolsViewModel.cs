using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo;
using FirewallApp.Data;
using FirewallUtilities;
using static FirewallUtilities.Utilities;

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

        public virtual bool IsOverwriteRules { get; set; } = true;

        #endregion

        #region Methods

        public async void OnInitializeRuleModelsCommand()
        {
            var RulesObjs = await GetFirewallRule();

            foreach (var ruleObj in RulesObjs)
            {
                if (ruleObj.Properties["DisplayName"].Value.ToString().ToLower().Contains("test") || 1==1)
                {
                    Debug.WriteLine("------------------------------" + ruleObj.Properties["DisplayName"].Value.ToString());


                    var rule = await ConvertToRule(ruleObj, false);

                    var existingRuleModel = RuleModelCollection.Where(x => x.InstanceID == rule.InstanceID);
                    RuleModel ruleModel;
                    bool IsNewRule = false;
                    if (!existingRuleModel.Any())
                    {
                        IsNewRule = true;
                        ruleModel = new RuleModel(uow)
                        {
                            DisplayName = rule.DisplayName,
                            Description = rule.Description,
                            DisplayGroup = rule.DisplayGroup,
                            InstanceID = rule.InstanceID
                        };
                    }
                    else 
                    {
                        ruleModel = (RuleModel)existingRuleModel.ToList()[0];
                    }

                    if (IsNewRule || IsOverwriteRules)
                    {
                        ruleModel.Action = rule.Action.ToString();
                        ruleModel.Direction = rule.Direction.ToString();
                        ruleModel.EdgeTraversalPolicy = rule.EdgeTraversalPolicy.ToString();
                        ruleModel.Enabled = rule.Enabled.ToString();
                        ruleModel.InterfaceType = rule.InterfaceType.ToString();
                        ruleModel.PrimaryStatus = rule.PrimaryStatus.ToString();
                        ruleModel.Profile = rule.Profile.ToString();

                        await rule.GetAdditionalInfo();
                        ruleModel.LocalAddress = rule.LocalAddress;
                        ruleModel.RemoteAddress = rule.RemoteAddress;
                        ruleModel.LocalPort = rule.LocalPort;
                        ruleModel.RemotePort = rule.RemotePort;
                        ruleModel.Protocol = rule.Protocol.ToString();
                        ruleModel.Program = rule.Program;

                        RuleModelCollection.Add(ruleModel);
                        await uow.CommitChangesAsync();
                    }



                }


            }
        }

        #endregion
    }
}