using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Vsix.Common.Helpers;
using Vsix.Viewer;
using Vsix.Viewer.ViewModels;
using ResourcesHelper = Vsix.Viewer.Helpers.ResourcesHelper;

namespace Vsix.Viewer.Presenters
{
    public class ManifestPresenter
    {
        private readonly IManifestModel _viewModel;
        //private readonly BackgroundWorker _bgWorker;
        public string _nl = Environment.NewLine;
        private string _webFile, _textFile, _wordFile, _iconFile, _csprojects,
            _csprojectsTitle, _licenseTitle, _iconTitle, _previewImageTitle, _gettingStartedTitle;

        public ManifestPresenter(IManifestModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;

            #region init strings/labels
            if (string.IsNullOrEmpty(_webFile)) _webFile = ResourcesHelper.Instance.GetString("web_file");
            if (string.IsNullOrEmpty(_webFile)) _webFile = "web.file";

            if (string.IsNullOrEmpty(_textFile)) _textFile = ResourcesHelper.Instance.GetString("text_file");
            if (string.IsNullOrEmpty(_textFile)) _textFile = "text.file";

            if (string.IsNullOrEmpty(_wordFile)) _wordFile = ResourcesHelper.Instance.GetString("word_file");
            if (string.IsNullOrEmpty(_wordFile)) _wordFile = "word.document";

            if (string.IsNullOrEmpty(_iconFile)) _iconFile = ResourcesHelper.Instance.GetString("icon_file");
            if (string.IsNullOrEmpty(_iconFile)) _iconFile = "image.file";

            if (string.IsNullOrEmpty(_csprojects)) _csprojects = ResourcesHelper.Instance.GetString("cs_project");
            if (string.IsNullOrEmpty(_csprojects)) _csprojects = "CSharp.projects";

     
            if (string.IsNullOrEmpty(_licenseTitle)) _licenseTitle = ResourcesHelper.Instance.GetString("license_title");
            if (string.IsNullOrEmpty(_licenseTitle)) _licenseTitle = "Select.License.file";

            if (string.IsNullOrEmpty(_csprojectsTitle)) _csprojectsTitle = ResourcesHelper.Instance.GetString("cs_project_title");
            if (string.IsNullOrEmpty(_csprojectsTitle)) _csprojectsTitle = "Select.cs.Project.file";

            if (string.IsNullOrEmpty(_iconTitle)) _iconTitle = ResourcesHelper.Instance.GetString("icon_title");
            if (string.IsNullOrEmpty(_iconTitle)) _iconTitle = "Select.Icon.file";

            if (string.IsNullOrEmpty(_previewImageTitle)) _previewImageTitle = ResourcesHelper.Instance.GetString("preivewImage_title");
            if (string.IsNullOrEmpty(_previewImageTitle)) _previewImageTitle = "Select.Preivew.Image.file";

            if (string.IsNullOrEmpty(_gettingStartedTitle)) _gettingStartedTitle = ResourcesHelper.Instance.GetString("gettingStarted_title");
            if (string.IsNullOrEmpty(_gettingStartedTitle)) _gettingStartedTitle = "Select.Getting.Started.file";
            #endregion
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
            string s = string.Empty;
            List<string> lst;
            switch (e.PropertyName)
            {
                case "RefreshGuid"	:
                   _viewModel.ProductIdGuid = Guid.NewGuid().ToString();
                   break;
                    
                case "BrowseIcon":
                    s = GetIcon();
                    if (!string.IsNullOrEmpty(s)) _viewModel.IconPath = s;
                    break;
                    
                case "BrowsePreviewImage":
                    s = GetPreviewImage();
                    if (!string.IsNullOrEmpty(s)) _viewModel.PreviewImagePath = s;
                    break;
                    
                case "BrowseReleaseNotes":
                    s = GetReleaseNotes();
                    if (!string.IsNullOrEmpty(s)) _viewModel.ReleaseNotesPath = s;
                    break;
                    
                case "BrowseGettingStartedGuide":
                    s = GetGettingStartedGuide();
                    if (!string.IsNullOrEmpty(s)) _viewModel.GettingStartedGuidePath = s;
                    break;
                    
                case "BrowseLicense":
                    s = GetLicense();
                    if (!string.IsNullOrEmpty(s)) _viewModel.LicensePath = s;
                    break;

                case "GetProjectCsPath":
                    s = GetProjectCsPath();
                    if (!string.IsNullOrEmpty(s)) _viewModel.ProjectCsPath = s;
                    break;
                    
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

        public string GetIcon()
        {
            var lst = new List<string> { _iconFile + "|*.ico;*.jpg;*.png;*.gif;*.tif;*.tiff;*.bmp" };
            return ExplorerHelper.SelectFileInExplorer(_iconTitle, lst);
        }

        public string GetPreviewImage()
        {
            var lst = new List<string> { _iconFile + "|*.ico;*.jpg;*.png;*.gif;*.tif;*.tiff;*.bmp" };
            return ExplorerHelper.SelectFileInExplorer(_previewImageTitle, lst);
        }

        public string GetReleaseNotes()
        {
            var lst = new List<string>
                    {
                        _webFile + "|*.html",
                        _webFile + "|*.htm",
                        _textFile + "|*.txt",
                        _wordFile + "|*.rtf",
                    };
            return ExplorerHelper.SelectFileInExplorer("Select Release Notes file", lst);
        }

        public string GetGettingStartedGuide()
        {
           var lst = new List<string>
                    {
                        _webFile + "|*.html",
                        _webFile + "|*.htm",
                        _textFile + "|*.txt",
                        _wordFile + "|*.rtf",
                        _wordFile + "|*.doc",
                        _wordFile + "|*.docx"
                    };
           return ExplorerHelper.SelectFileInExplorer(_gettingStartedTitle, lst);
        }
        
        public string GetLicense()
        {
           var lst = new List<string>
                    {
                        _wordFile + "|*.rtf",
                        _textFile + "|*.txt"
                    };
            return ExplorerHelper.SelectFileInExplorer(_licenseTitle, lst);
        }

        public string GetProjectCsPath()
        {
            return ExplorerHelper.SelectFileInExplorer(_csprojectsTitle, _csprojects + "|*.csproj");
        }

        private void NewVsix()
        {
            var frm = new frmVsixManifest();
            frm.ShowDialog();
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
             
    }
}
