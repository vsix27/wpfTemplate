using System.ComponentModel;

namespace Vsix.Viewer.ViewModels
{
    public interface IVsixDetailsModel
    {
        event PropertyChangedEventHandler PropertyChanged;
        string OutputText { get; set; }
    }
}
