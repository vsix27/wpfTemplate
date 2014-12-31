using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Ionic.Utils.Zip;
using Vsix.Common.Helpers;
using Vsix.Viewer.ViewModels;
using Vsix.Viewer;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Presenters
{
    public class MainViewPresenter
    {
        private readonly IMainViewModel _viewModel;
        private readonly BackgroundWorker _bgWorker;

        public MainViewPresenter(IMainViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;

            // initialize bgWorker
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += BgWorkerDoWork;
            _bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
        }

        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ProcessFiles();
        }

        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.OutputText += "processed all files " + _nl;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LogHelper.LogEnter("e.PropertyName " + e.PropertyName);
            switch (e.PropertyName)
            {
                case "FolderPath":
                    // put verfication logic if needed
                    _viewModel.ErrorText = (!Directory.Exists(_viewModel.FolderPath))
                        ? ("invalid directory, please change: " + _viewModel.FolderPath)
                        : null;
                    break;

                case "OutputText":
                    break;

                case "ViewLog":
                    string logfile = LogHelper.LogFilePath;
                    if (!string.IsNullOrEmpty(logfile) && File.Exists(logfile))
                        Process.Start(logfile);
                    else
                        _viewModel.ErrorText = "log file not detected";
                    
                    break;

                //case "NewVsix":
                //    NewVsix();
                //    break;

                case "OpenVsix":
                    OpenVsix();
                    break;

                case "SaveVsix":
                    SaveVsix();
                    break;

                    #region done

                case "ViewStyles":
                    ViewXamlAttributesStartingWith(new[]
                    {
                        "{StaticResource",
                        "{DynamicResource"
                    });
                    break;

                case "ViewBindings":
                    ViewXamlAttributesStartingWith("{Binding");
                    break;

                case "ProcessFiles":
                    _bgWorker.RunWorkerAsync();
                    break;
                    //case "GetTime":
                    //    _viewModel.OutputText = DateTime.Now +_nl;
                    //    break;                               

                    #endregion

                case "SelectInputFolder":
                    SelectInputFolder();
                    break;

                default:
                    break;
            }
        }

        public string _nl = Environment.NewLine;

      

        private void NewVsix()
        {
            var frm = new frmVsixManifest();
            frm.ShowDialog();
        }
        private void OpenVsix()
        {
            var dialog = new OpenFileDialog
            {
                Filter = @"Vsix packages (.vsix)|*.vsix",
                Multiselect = false,
                FilterIndex = 1,
                Title=@"Select Vsix package ",
                RestoreDirectory=true,
                InitialDirectory=AppDomain.CurrentDomain.BaseDirectory ,
            };
            //dialog.Filter = @"Vsix packages (.vsix)|*.vsix|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _viewModel.VsixPath = dialog.FileName;
                _viewModel.VsixPathUnpacked = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(_viewModel.VsixPathUnpacked);
                using (var decompress = ZipFile.Read(_viewModel.VsixPath))
                {
                    foreach (var e in decompress)
                    {
                        e.Extract(_viewModel.VsixPathUnpacked, true);
                        /*
                         FileName: "TemplateBuilder.dll"
                         LocalFileName: "TemplateBuilder.dll"
                         IsDirectory: false
                         */
                        //Console.WriteLine(e.LocalFileName);
                    }
                    /* expected: files: 
                            [Content_Types].xml
                            extension.vsixmanifest
                            TemplateBuilder.dll
                            Output\ProjectTemplates\CSharp\Windows%20Desktop\WpfDemoWithMvvMp.project.zip
                     * */
                }
                Process.Start(_viewModel.VsixPathUnpacked);
            }
        }

        #region file samler [Content_Types].xml, extension.vsixmanifest
        /* file 
         * [Content_Types].xml - static
          
            <?xml version="1.0" encoding="utf-8"?>
            <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
	            <Default Extension="zip" ContentType="application/zip" /><Default Extension="dll" ContentType="application/octet-stream" />
	            <Default Extension="vsixmanifest" ContentType="text/xml" />
            </Types>
                           
         * extension.vsixmanifest (Version="2.0.0"     )
          
        <PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011">
          <Metadata>
            <Identity Id="VSIXProjectForWpfTemplate..b553d51a-30d4-433b-863d-7f67a93c460a" Version="1.0" Language="en-US" Publisher="Alex Bondarev" />
            <DisplayName>VSIXProjectForWpfTemplate</DisplayName>
            <Description>Empty VSIX Project.</Description>
          </Metadata>
          <Installation>
            <InstallationTarget Id="Microsoft.VisualStudio.Pro" Version="[14.0]" />
          </Installation>
          <Dependencies>
            <Dependency Id="Microsoft.Framework.NDP" DisplayName="Microsoft .NET Framework" Version="[4.5,)" />
          </Dependencies>
          <Assets>
            <Asset Type="Microsoft.VisualStudio.ItemTemplate" Path="Output\ItemTemplates" />
            <Asset Type="Microsoft.VisualStudio.ProjectTemplate" Path="Output\ProjectTemplates" />
          </Assets>
        </PackageManifest>
         
         */

        #endregion

        private void SaveVsix() { }

        #region xaml attribute debug

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attrStart">"{Binding"  </param>
        private void ViewXamlAttributesStartingWith(string attrStart)
        {
            var list = GetXamlAttributesStartingWith(attrStart);
            if (list.Count > 0)
                list.DebugOpenText(attrStart, true);
        }
        private void ViewXamlAttributesStartingWith(string[] attrStarts)
        {
            var list = GetXamlAttributesStartingWith(attrStarts);
            if (list.Count > 0)
            {
                list.DebugOpenText(attrStarts, true);
                //list.DebugOpenHtml(attrStarts, true);
            }
        }
        private List<string> GetXamlAttributesStartingWith(IEnumerable<string> attrStarts)
        {
            var listM = new List<string>();
            foreach (string attrStart in attrStarts)
            {
                var list = GetXamlAttributesStartingWith(attrStart);
                if (list != null && list.Count > 0) 
                    listM.AddRange(list);
            }
            return listM;
        }
        private List<string> GetXamlAttributesStartingWith(string attrStart)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(attrStart)) return list; 

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // should ends with bin\debug or bin\release, but could be different if custom profile...
            if (baseDir.ToLower().Contains("\\bin\\"))
            {
                foreach (string file in Directory.GetFiles(Path.GetFullPath(baseDir + "..\\.."), "*.xaml", SearchOption.AllDirectories))
                {
                    var doc = new XmlDocument();
                    doc.Load(file);
                    var xDoc = XDocument.Load(reader: new XmlNodeReader(doc));
                    if (xDoc.Root == null) continue;

                    var items = xDoc.Root.DescendantsAndSelf().Attributes().Where(a => a.Value.StartsWith(attrStart, StringComparison.OrdinalIgnoreCase));

                    if (items.Count() == 0) 
                        continue;

                    list.Add(" ===== file ===== " + Path.GetFileName(file));
                    list.AddRange(items.Select(nd => nd.Name + "=" + nd.Value));
                }
            }
            return list;
        }

        #endregion

        private void SelectInputFolder()
        {
            var dialog = new FolderBrowserDialog();

            var outlookfiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Outlook Files");

            if (Directory.Exists(outlookfiles))
            {
                _viewModel.ErrorText = null;
                dialog.SelectedPath = outlookfiles;
            }
            else
            {
                _viewModel.ErrorText = "outlook is not installed, will use 'my documents'";
                dialog.RootFolder = Environment.SpecialFolder.MyDocuments;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
                _viewModel.FolderPath = dialog.SelectedPath;
        }

        private void ProcessFiles()
        {
            if (!Directory.Exists(_viewModel.FolderPath))
            {
                _viewModel.ErrorText = "invalid directory - please change: " + _viewModel.FolderPath;
                return;
            }

            _viewModel.ErrorText = null;

            foreach (var f in Directory.GetFiles(_viewModel.FolderPath, "*.olk14Note", SearchOption.AllDirectories))
            {
                var ss = new List<string>();
                string fName = string.Empty;
                bool skip = true;
                foreach (var line in File.ReadAllLines(f))
                {
                    if (line.Equals("</html>", StringComparison.OrdinalIgnoreCase))
                    {
                        skip = false;
                        continue;
                    }

                    if (skip) continue;

                    if (string.IsNullOrEmpty(fName))
                        fName = line;
                    else
                        ss.Add(line);
                }
                try
                {
                    string ffName = Path.Combine(Directory.GetParent(f).FullName, fName + ".txt");
                    File.WriteAllLines(ffName, ss);
                    File.Delete(f);
                    _viewModel.OutputText += "processed " + f + " as " + ffName+ _nl;
                }
                catch (Exception ex) { _viewModel.ErrorText += ex.Message + _nl; }
            }
        }
              
    }
}
