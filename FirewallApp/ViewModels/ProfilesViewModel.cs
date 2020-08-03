using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

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
            }
        }

        #region Constructors

        protected ProfilesViewModel()
        {
        }

        public static ProfilesViewModel Create()
        {
            return ViewModelSource.Create(() => new ProfilesViewModel());
        }

        #endregion

        #region Fields and Properties

        

        #endregion

        #region Methods

        #endregion
    }
}