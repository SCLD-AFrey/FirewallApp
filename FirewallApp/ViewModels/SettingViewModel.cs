using System.ComponentModel.DataAnnotations;
using System.Management.Automation;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using FirewallUtilities;
using FirewallApp.Views;

namespace FirewallApp.ViewModels
{
    [MetadataType(typeof(MetaData))]
    public class SettingViewModel
    {
        public class MetaData : IMetadataProvider<SettingViewModel>
        {
            void IMetadataProvider<SettingViewModel>.BuildMetadata
                (MetadataBuilder<SettingViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.OnBuildPsScriptCommand()).CommandName("BuildPsScriptCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnViewSettingObjectsCommand()).CommandName("ViewSettingObjectsCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnRunPsScriptCommand()).CommandName("RunPsScriptCommand");
            }
        }

        #region Constructors

        protected SettingViewModel()
        {
            FirewallUtils = new Utilities();
            GetSettings();
        }

        public static SettingViewModel Create()
        {
            return ViewModelSource.Create(() => new SettingViewModel());
        }

        #endregion

        #region Fields and Properties



        public virtual FirewallUtilities.Utilities FirewallUtils { get; set; }
        public virtual PSObject SettingObject { get; set; }
        public virtual string PowerShellScript { get; set; }
        public virtual bool IsEditEnabled { get; set; } = false;
        public virtual Setting SelectedSetting { get; set; }

        #endregion

        #region Methods
        public async void GetSettings()
        {

            var result = await FirewallUtils.GetNetFirewallSetting();
            SelectedSetting = FirewallUtils.ConvertToSetting(result[0]);
        }

        public void OnBuildPsScriptCommand()
        {
            
        }

        public async void OnRunPsScriptCommand()
        {
            IsEditEnabled = false;
            await FirewallUtils.SetExecutionPolicy();

            IsEditEnabled = true;
        }

        public async void OnViewSettingObjectsCommand()
        {
            var result = await FirewallUtils.GetNetFirewallSetting();
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;
            newWin.Show();
        }
        #endregion
    }
}

