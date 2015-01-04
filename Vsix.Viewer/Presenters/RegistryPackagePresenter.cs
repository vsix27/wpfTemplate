using System.ComponentModel;
using Vsix.Common.Helpers;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Presenters
{
    public class RegistryPackagePresenter
    {
        private readonly IRegistryPackageModel _viewModel;
        //private readonly BackgroundWorker _bgWorker;

        public RegistryPackagePresenter(IRegistryPackageModel viewModel)
        {
            _viewModel = viewModel;
            //// initialize bgWorker
            //_bgWorker = new BackgroundWorker();
            //_bgWorker.DoWork += BgWorkerDoWork;
            //_bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;

        }

        //private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        //{
        //    ProcessFiles();
        //}

        //private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    _viewModel.Author += "processed all files " + _nl;
        //}
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

                //case "BrowseReleaseNotes":
                //    _viewModel.ReleaseNotesPath = GetReleaseNotes();
                //    break;

                //case "BrowseGettingStartedGuide":
                //    _viewModel.GettingStartedGuidePath = GetGettingStartedGuide();
                //    break;

                //case "BrowseLicense":
                //    _viewModel.LicensePath = GetLicense();
                //    break;

                //case "GetProjectCsPath":
                //    _viewModel.ProjectCsPath = GetProjectCsPath();
                //    break;

                //// put verfication logic if needed
                //_viewModel.ErrorText = (!Directory.Exists(_viewModel.FolderPath))
                //    ? ("invalid directory, please change: " + _viewModel.FolderPath)

                //case "ProcessFiles":
                //    _bgWorker.RunWorkerAsync();
                //    break;

                default:
                    break;
            }
        }

    }
}
