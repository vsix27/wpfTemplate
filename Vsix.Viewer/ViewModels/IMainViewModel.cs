using System.ComponentModel;

namespace Vsix.Viewer.ViewModels
{
    /// <summary> properties used in presenter </summary>
    public interface IMainViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;
        string FolderPath { get; set; }
        string OutputText { get; set; }
        string ErrorText { get; set; }
        string VsixPackage { get; set; }
        string VsixPath { get; set; }
        string VsixPathUnpacked { get; set; }
    }
}
