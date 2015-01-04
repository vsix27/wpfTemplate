using System.ComponentModel;
using Vsix.Common.Helpers;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Presenters
{
    public class VsixDetailsPresenter
    {
        private readonly IVsixDetailsModel _viewModel;

        public VsixDetailsPresenter(IVsixDetailsModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LogHelper.LogEnter(e.PropertyName);
            switch (e.PropertyName)
            {
                //case "RefreshGuid":
                //    _viewModel.ProductIdGuid = Guid.NewGuid().ToString();
                //    break;

                //case "BrowseIcon":
                //    _viewModel.IconPath = GetIcon();
                //    break;

                //case "BrowsePreviewImage":
                //    _viewModel.PreviewImagePath = GetPreviewImage();
                //    break;

                default:
                    break;
            }
        }

    }
}
