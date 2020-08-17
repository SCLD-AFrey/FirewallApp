﻿using System;
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
            await FirewallUtils.SetExecutionPolicy();
            var resultObjs = await FirewallUtils.GetFirewallProfile();
            ProfileCollection = FirewallUtils.CreateProfileCollection(resultObjs);
        }
        public void OnBuildPsScriptCommand()
        {
            PowerShellScript = FirewallUtils.BuildProfileScript(ProfileCollection);
        }

        public async void OnRunPsScriptCommand()
        {
            IsEditEnabled = false;
            await FirewallUtils.SetExecutionPolicy();

            var result = await FirewallUtils.ExecScriptTask(PowerShellScript, true);
            PowerShellScript = "Script Completed - " + result.IsSuccess.ToString();
            IsEditEnabled = true;
        }

        public async void OnViewProfileObjectsCommand()
        {
            var result = await FirewallUtils.GetFirewallProfile();
            var newWin = new ObjectView();
            var obj = ObjectViewModel.Create();
            obj.SelectedPsObject = result[0];
            newWin.DataContext = obj;
            newWin.Show();
        }

        #endregion
    }
}