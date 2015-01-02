using System;
using System.Data;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Vsix.Common.Helpers;
using Vsix.Viewer.Helpers;
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
            //DataGridRegistryPackages.Columns.Clear();
            _viewModel = new RegistryPackageModel();
            this.DataContext = _viewModel;

            // insert space before capital letter, handles acronims
            // Regex.Replace(sKey, "(\\B[A-Z])", " $1"); // insert space before capital letter
            Func<string, string> normalizeHeader = o => Regex.Replace(o, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1"); 

            var dgc1 = new DataGridTemplateColumn();
            foreach (string sKey in new RegistryPackage(null).ToStringPairs(true).Keys)
            {
                var dcText = new DataGridTextColumn
                {
                    Binding = new System.Windows.Data.Binding() {XPath = sKey},
                    Header = normalizeHeader(sKey)
                };
                //DataGridRegistryPackages.Columns.Add(dcText);

                //dgc1.SortMemberPath = sKey;
                ////string sHeader = Regex.Replace(sKey, "(\\B[A-Z])", " $1"); // insert space before capital letter
                //dgc1.Header = Regex.Replace(sKey, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1"); // insert space before capital letter
                //dgc1.CanUserSort = true;
                //dgc1.Width = new DataGridLength(100);
                //dgc1.CanUserResize = true;
                //dgc1.CellTemplate = new DataTemplate() ;
              
                //DataGridRegistryPackages.Columns.Add(dgc1);
            }

            /*
             <DataGridTemplateColumn SortMemberPath="PackageName" Header="Package Name" Width="100" CanUserSort="true">
                        <DataGridTemplateColumn.CellTemplate>
                            <DataTemplate>
                                <StackPanel><TextBlock Text="{Binding Path=PackageName}" /></StackPanel>
                            </DataTemplate>
                        </DataGridTemplateColumn.CellTemplate>
                    </DataGridTemplateColumn>
             */
            
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
    }
}
