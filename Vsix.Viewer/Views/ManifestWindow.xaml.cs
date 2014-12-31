using System;
using System.Windows;
using Vsix.Common.Helpers;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Views
{
    /// <summary>
    /// Interaction logic for ManifestWindow.xaml
    /// </summary>
    public partial class ManifestWindow : Window
    {
        private readonly ManifestModel _viewModel;
        
        public ManifestWindow()
        {
            InitializeComponent();
            _viewModel = new ManifestModel();
            this.DataContext = _viewModel;
        }

        public ManifestWindow(string csproj)
        {
            InitializeComponent();
            try
            {
                _viewModel = new ManifestModel {ProjectCsPath = csproj};
               
                if (System.IO.File.Exists(csproj))
                {
                    var doc = XmlHelper.DocLoadNoNamespaces(csproj);
                    var nd = doc.SelectSingleNode("//RootNamespace");
                    _viewModel.ProductName = nd.InnerText.Replace( ".","");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            this.DataContext = _viewModel;
        }


        private void CommandCheckManifest(object sender, RoutedEventArgs e)
        {
            DebugHelper.DebugOpenHtml(_viewModel.ToStrings(), string.Empty, true);
            //Console.WriteLine(_viewModel.ToString());
        }

        private void ManifestSave(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void ManifestCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
