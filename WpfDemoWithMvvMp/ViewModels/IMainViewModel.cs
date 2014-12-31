using System.ComponentModel;

namespace WpfDemoWithMvvmp.ViewModels
{
    /// <summary>
    /// properties used in presenter
    /// </summary>
    public interface IMainViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;
        string FolderPath { get; set; }
        string OutputText { get; set; }
        string ErrorText { get; set; }       
    }
}
