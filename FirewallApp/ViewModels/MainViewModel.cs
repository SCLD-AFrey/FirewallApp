using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using FirewallUtilities;
using DevExpress.Xpo;
using FirewallApp.Data;
using System.Reflection;
using DevExpress.XtraScheduler.iCalendar.Components;
using Namotion.Reflection;

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

                p_builder.CommandFromMethod(p_x => p_x.OnInitializeDatabaseRulesCommand()).CommandName("InitializeDatabaseRulesCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnCreateRuleFixScriptCommand()).CommandName("CreateRuleFixScriptCommand");
                p_builder.Property(p_x => p_x.SelectedRuleModel).OnPropertyChangedCall(p_x => p_x.SelectedRuleModelChanged());
                
            }
        }

        #region Constructors

        protected MainViewModel()
        {
            FirewallUtils = new FirewallUtilities.Utilities();
            uow = new UnitOfWork();
            GetRules();
        }

        public static MainViewModel Create()
        {
            return ViewModelSource.Create(() => new MainViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual UnitOfWork uow { get; set; }
        public virtual Utilities FirewallUtils { get; set; }
        public virtual ObservableCollection<Rule> RuleCollection { get; set; }
        public virtual XPCollection<RuleModel> RuleModelCollection { get; set; }
        public virtual Rule SelectedRule { get; set; }
        public virtual RuleModel SelectedRuleModel { get; set; }

        public virtual string Message { get; set; } = string.Empty;
        public virtual string PowerShellScript { get; set; } = string.Empty;
        public virtual bool AllowEdit { get; set; } = true;

        #endregion

        #region Methods


        private async Task GetRules()
        {
            await FirewallUtils.SetExecutionPolicy();
            //await OnInitializeDatabaseRulesCommand();
            var resultObjs = await FirewallUtils.GetFirewallRule();
            RuleCollection = await FirewallUtils.CreateRuleCollection(resultObjs);
            RuleModelCollection = new XPCollection<RuleModel>(uow);
        }

        public async void SelectedRuleModelChanged()
        {
            AllowEdit = false;
            SelectedRule = null;
            var _t = new Rule();
            if (SelectedRuleModel != null)
            {
                var _rule = RuleCollection.ToList().Where(x => x.InstanceID == SelectedRuleModel.InstanceID);

                if (_rule.Count() == 1)
                {
                    _t = _rule.First();
                    _t.IsNew = false;
                    await _t.GetAdditionalInfo();
                } else if (_rule.Count() == 0)
                {
                    _t = new Rule()
                    {
                        IsNew = true,
                        DisplayName = SelectedRuleModel.DisplayName,
                        Description = SelectedRuleModel.Description,
                        DisplayGroup = SelectedRuleModel.DisplayGroup,
                        Action = Enum.Parse<Enumerations.Action>(SelectedRuleModel.Action),
                        Enabled = Enum.Parse<Enumerations.Enabled>(SelectedRuleModel.Enabled),
                        Direction = Enum.Parse<Enumerations.Direction>(SelectedRuleModel.Direction),
                        EdgeTraversalPolicy = Enum.Parse<Enumerations.EdgeTraversalPolicy>(SelectedRuleModel.EdgeTraversalPolicy),
                        PrimaryStatus = Enum.Parse<Enumerations.PrimaryStatus>(SelectedRuleModel.PrimaryStatus),
                        InterfaceType = Enum.Parse<Enumerations.InterfaceType>(SelectedRuleModel.InterfaceType),
                        Program = SelectedRuleModel.Program,
                        LocalAddress = SelectedRuleModel.LocalAddress,
                        RemoteAddress = SelectedRuleModel.RemoteAddress,
                        LocalPort = SelectedRuleModel.LocalPort,
                        RemotePort = SelectedRuleModel.RemotePort,
                        Protocol = Enum.Parse<Enumerations.Protocol>(SelectedRuleModel.Protocol),
                        Profile = Enum.Parse<Enumerations.Profile>(SelectedRuleModel.Profile)
                    };
                }

                SelectedRule = _t;
            }

            AllowEdit = true;
        }

        public void OnCreateRuleFixScriptCommand()
        {

            //SelectedRule.DisplayName = SelectedRuleModel.DisplayName;
            SelectedRule.Description = SelectedRuleModel.Description;
            SelectedRule.DisplayGroup = SelectedRuleModel.DisplayGroup;
            SelectedRule.Action = Enum.Parse<Enumerations.Action>(SelectedRuleModel.Action);
            SelectedRule.Enabled = Enum.Parse<Enumerations.Enabled>(SelectedRuleModel.Enabled);
            //SelectedRule.Direction = Enum.Parse<Enumerations.Direction>(SelectedRuleModel.Direction);
            SelectedRule.EdgeTraversalPolicy = Enum.Parse<Enumerations.EdgeTraversalPolicy>(SelectedRuleModel.EdgeTraversalPolicy);
            SelectedRule.PrimaryStatus = Enum.Parse<Enumerations.PrimaryStatus>(SelectedRuleModel.PrimaryStatus);
            SelectedRule.InterfaceType = Enum.Parse<Enumerations.InterfaceType>(SelectedRuleModel.InterfaceType);
            SelectedRule.Program = SelectedRuleModel.Program;
            SelectedRule.LocalAddress = SelectedRuleModel.LocalAddress;
            SelectedRule.RemoteAddress = SelectedRuleModel.RemoteAddress;
            SelectedRule.LocalPort = SelectedRuleModel.LocalPort;
            SelectedRule.RemotePort = SelectedRuleModel.RemotePort;
            SelectedRule.Protocol = Enum.Parse<Enumerations.Protocol>(SelectedRuleModel.Protocol);

            PowerShellScript = SelectedRule.BuildScript;

        }

        public async Task OnInitializeDatabaseRulesCommand()
        {
            XPCollection<RuleModel> _rules = new XPCollection<RuleModel>(uow);
            var RulesMain = await FirewallUtils.GetFirewallRule();




            foreach (var psObj in RulesMain)
            {
                var r = new RuleModel(uow);
                var _rule = await FirewallUtils.ConvertToRule(psObj);
                XPCollection<RuleModel> check = new XPCollection<RuleModel>(uow, CriteriaOperator.Parse($@"DisplayName = '{_rule.DisplayName}'"));
                if (check.Count == 0 && _rule.DisplayGroup == "")
                {
                    try
                    {
                        await _rule.GetAdditionalInfo();

                        r.DisplayName = _rule.DisplayName;
                        r.DisplayGroup = _rule.DisplayGroup;
                        r.InstanceID = _rule.InstanceID;
                        r.Action = _rule.Action.ToString();
                        r.Enabled = _rule.Enabled.ToString();
                        r.Direction = _rule.Direction.ToString();
                        r.EdgeTraversalPolicy = _rule.EdgeTraversalPolicy.ToString();
                        r.PrimaryStatus = _rule.PrimaryStatus.ToString();
                        r.InterfaceType = _rule.InterfaceType.ToString();
                        r.Program = _rule.Program;
                        r.LocalAddress = _rule.LocalAddress;
                        r.RemoteAddress = _rule.RemoteAddress;
                        r.LocalPort = _rule.LocalPort;
                        r.RemotePort = _rule.RemotePort;
                        r.Protocol = _rule.Protocol.ToString();
                        r.Profile = _rule.Profile.ToString();
                        _rules.Add(r);
                        uow.CommitChanges();
                    }
                    catch
                    {
                        r.DisplayName = _rule.DisplayName;
                        r.Description = "FAILED";
                        _rules.Add(r);
                        Debug.WriteLine($@"FAILED INSERT {_rule.DisplayName}");
                    }

                }
            }
        }

        #endregion
    }
}