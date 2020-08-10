using System.ComponentModel.DataAnnotations;
using System.Management.Automation;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace FirewallApp.ViewModels
{
    [MetadataType(typeof(MetaData))]
    public class ObjectViewModel
    {
        public class MetaData : IMetadataProvider<ObjectViewModel>
        {
            void IMetadataProvider<ObjectViewModel>.BuildMetadata
                (MetadataBuilder<ObjectViewModel> p_builder)
            {
            }
        }

        #region Constructors

        protected ObjectViewModel()
        {
        }

        public static ObjectViewModel Create()
        {
            return ViewModelSource.Create(() => new ObjectViewModel());
        }

        #endregion

        #region Fields and Properties

        public virtual PSObject SelectedPsObject { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}