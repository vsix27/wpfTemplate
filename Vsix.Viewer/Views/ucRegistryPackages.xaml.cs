using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vsix.Common.Helpers;
using Vsix.Viewer.Helpers;
using Vsix.Viewer.Infrastructure;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Views
{
    /// <summary>
    /// Interaction logic for ucRegistryPackages.xaml
    /// </summary>
    public partial class UcRegistryPackages : UserControl
    {
        private readonly RegistryPackageModel _viewModel;

        public UcRegistryPackages()
        {
            InitializeComponent();

            _viewModel = new RegistryPackageModel();
            this.DataContext = _viewModel;
            
            DataGridRegistryPackages.ItemsSource = _viewModel.RegistryPackagesData;

            _viewModel.RegistryPackagesData.ListChanged +=
                delegate { DataGridRegistryPackages.ItemsSource = _viewModel.RegistryPackagesData; };

            CtxMenu.Items.Clear();

            CtxMenu.Items.Add(new MenuItem
            {
                Header = ResourcesHelper.Instance.GetString("menuCtxOpenCodeBaseLocation"),
                Command = CommandOpenCodeBaseLocation
            });
            CtxMenu.Items.Add(new MenuItem
            {
                Header = ResourcesHelper.Instance.GetString("menuCtxOpenRegistryLocation"),
                Command = CommandOpenRegistry
            });
        }
        
        private RegistryPackage LastRegistryPackage { get; set; }

        private ICommand CommandOpenCodeBaseLocation
        {
            get { return new RelayCommand(OpenCodeBaseLocation); }
        }

        private void OpenCodeBaseLocation()
        {
            if (string.IsNullOrEmpty(LastRegistryPackage.CodeBase)) return;
            if (File.Exists(LastRegistryPackage.CodeBase))
            {
                Process.Start(Directory.GetParent(LastRegistryPackage.CodeBase).FullName);
            }
        }

        private ICommand CommandOpenRegistry
        {
            get { return new RelayCommand(OpenRegistry); }
        }

        private void OpenRegistry()
        {
            try
            {
                RegistryHelper.OpenToKey(LastRegistryPackage.RegistryPath);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        private void DataGridRegistryPackages_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var c in DataGridRegistryPackages.Columns)
            {
                switch ("" + c.Header)
                {
                    case "RegistryPath":
                    case "Default":
                        c.Width = 70;
                        break;
                    case "RegistryGuid":
                    case "ProductVersion":
                    case "VisualStudioVersion":
                        c.Width = 40;
                        break;
                    case "CodeBase":
                        c.Width = 200;
                        break;
                    case "ProductName":
                        c.Width = 200;
                        break;
                    default:
                        break;
                }
                c.Header = ResourcesHelper.CamelToSentence(c.Header.ToString());
                Debug.WriteLine(c.Header);
            }
        }
        public UcRegistryPackages(string csproj)
        {
            InitializeComponent();
            try
            {
                //_viewModel = new ManifestModel {ProjectCsPath = csproj};
               
                //if (System.IO.File.Exists(csproj))
                //{
                //    var doc = XmlHelper.DocLoadNoNamespaces(csproj);
                //    var nd = doc.SelectSingleNode("//RootNamespace");
                //    _viewModel.ProductName = nd.InnerText.Replace( ".","");
                //}
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            this.DataContext = _viewModel;
        }

        private void DataGridRegistryPackages_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastRegistryPackage = null;
            var dg = sender as DataGrid;
            if (dg == null) return;
            LastRegistryPackage = dg.CurrentItem as RegistryPackage;
        
            //Debug.WriteLine(dg.CurrentItem);
            //Debug.WriteLine(dg.CurrentCell);
            //Debug.WriteLine(dg.CurrentColumn);
            //Debug.WriteLine(e);
        }
    }
}
