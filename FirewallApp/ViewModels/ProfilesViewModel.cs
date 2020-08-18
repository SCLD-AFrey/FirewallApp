using System;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;
using FirewallApp.Views;
using FirewallUtilities;

namespace FirewallApp.ViewModels
{
    [MetadataType(typeof(MetaData))]
    public class ProfilesViewModel
    {
        public class MetaData : IMetadataProvider<ProfilesViewModel>
        {
            void IMetadataProvider<ProfilesViewModel>.BuildMetadata
                (MetadataBuilder<ProfilesViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.OnBuildPsScriptCommand()).CommandName("BuildPsScriptCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnViewProfileObjectsCommand()).CommandName("ViewProfileObjectsCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnRunPsScriptCommand()).CommandName("RunPsScriptCommand");

            }
        }

        #region Constructors

        protected ProfilesViewModel()
        {
            PowerShellScript = string.Empty;
            FirewallUtils = new FirewallUtilities.Utilities();
            GetProfiles();
        }

        public static ProfilesViewModel Create()
        {
            return ViewModelSource.Create(() => new ProfilesViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual FirewallUtilities.Utilities FirewallUtils { get; set; }
        public virtual ObservableCollection<Profile> ProfileCollection { get; set; }
        public virtual Profile SelectedProfile { get; set; }
        public virtual string PowerShellScript { get; set; }
        public virtual bool IsEditEnabled { get; set; } = false;

        #endregion

        #region Methods

        private async void GetProfiles()
        {
            await Utilities.SetExecutionPolicy();
            var resultObjs = await Utilities.GetFirewallProfile();
            ProfileCollection = Utilities.CreateProfileCollection(resultObjs);
        }
        public void OnBuildPsScriptCommand()
        {
            PowerShellScript  = String.Empty;
            foreach (var profile in ProfileCollection)
            {
                PowerShellScript += profile.BuildScript;
            }
        }

        public async void OnRunPsScriptCommand()
        {
            IsEditEnabled = false;
            PowerShellScript = String.Empty;
            foreach (var profile in ProfileCollection)
            {
                var result = await profile.Commit(true);
                PowerShellScript += $@"'{profile.Name}' Script Completed - " + result.Item1.ToString();
            }

            IsEditEnabled = true;
        }

        public async void OnViewProfileObjectsCommand()
        {
            var result = await Utilities.GetFirewallProfile();
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;
            newWin.Show();
        }

        #endregion
    }
}