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
    public class ProfilesViewModel
    {
        public class MetaData : IMetadataProvider<ProfilesViewModel>
        {
            void IMetadataProvider<ProfilesViewModel>.BuildMetadata
                (MetadataBuilder<ProfilesViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.OnBuildPsScriptCommand()).CommandName("BuildPsScriptCommand");
                p_builder.CommandFromMethod(p_x => p_x.OnRunPsScriptCommand()).CommandName("RunPsScriptCommand");

            }
        }

        #region Constructors

        protected ProfilesViewModel()
        {
            shellScript = string.Empty;
            fUtils = new FirewallUtilities();
            GetProfiles();
        }

        public static ProfilesViewModel Create()
        {
            return ViewModelSource.Create(() => new ProfilesViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual FirewallUtilities fUtils { get; set; }
        public virtual ObservableCollection<Profile> ProfileCollection { get; set; }
        public virtual Profile SelectedProfile { get; set; }
        public virtual string shellScript { get; set; }
        public virtual bool IsEditEnabled { get; set; } = false;

        #endregion

        #region Methods

        private async void GetProfiles()
        {
            await fUtils.SetExecutionPolicy();
            var resultObjs = await fUtils.GetFirewallProfile();
            ProfileCollection = fUtils.CreateProfileCollection(resultObjs);
        }
        public void OnBuildPsScriptCommand()
        {
            shellScript = fUtils.BuildProfileScript(ProfileCollection);
        }

        public async void OnRunPsScriptCommand()
        {
            IsEditEnabled = false;
            await fUtils.SetExecutionPolicy();
            //var result = await PowershellTools.RunScript(OutputText);
            //OutputText = "Script Completed - " + result.IsSuccess.ToString();
            IsEditEnabled = true;
        }

        #endregion
    }
}