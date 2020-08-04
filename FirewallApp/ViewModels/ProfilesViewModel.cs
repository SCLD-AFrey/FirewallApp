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
    public class ProfilesViewModel
    {
        public class MetaData : IMetadataProvider<ProfilesViewModel>
        {
            void IMetadataProvider<ProfilesViewModel>.BuildMetadata
                (MetadataBuilder<ProfilesViewModel> p_builder)
            {
                p_builder.CommandFromMethod(p_x => p_x.OnCommitProfileCommand()).CommandName("CommitProfileCommand");

            }
        }

        #region Constructors

        protected ProfilesViewModel()
        {
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

        #endregion

        #region Methods

        private async void GetProfiles()
        {
            await fUtils.SetExecutionPolicy();
            var resultObjs = await fUtils.GetFirewallProfile();
            ProfileCollection = fUtils.CreateProfileCollection(resultObjs);
        }

        public void OnCommitProfileCommand()
        {

        }

        #endregion
    }
}