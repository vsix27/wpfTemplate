using System.Windows;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Views
{
    /// <summary>
    /// Interaction logic for UcViewerWindow.xaml
    /// </summary>
    public partial class UcViewerWindow : Window
    {
        public UcViewerWindow()
        {
            InitializeComponent();
            this.DataContext = new VsixDetailsModel() { ContentPath = wuc.ContentPath };
        }
    }
}
