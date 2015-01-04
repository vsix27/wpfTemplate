using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Vsix.Common.Helpers;
using Vsix.Viewer.Helpers;
using Vsix.Viewer.ViewModels;
using Vsix.Viewer.Views;

namespace Vsix.Viewer.Presenters
{
    public class ManifestPresenter
    {
        private readonly IManifestModel _viewModel;
        //private readonly BackgroundWorker _bgWorker;
        public string _nl = Environment.NewLine;

        private string _allFiles, 
            _webFile,
            _textFile,
            _wordFile,
            _iconFile,
            _csprojects,
            _selectReleaseNotesTitle,
            _csprojectsTitle,
            _licenseTitle,
            _iconTitle,
            _previewImageTitle,
            _gettingStartedTitle;

        public ManifestPresenter(IManifestModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;

            #region init strings/labels


            if (string.IsNullOrEmpty(_allFiles)) _allFiles = ResourcesHelper.Instance.GetString("all_files");
            if (string.IsNullOrEmpty(_allFiles)) _allFiles = "All.files";

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

            if (string.IsNullOrEmpty(_selectReleaseNotesTitle)) _selectReleaseNotesTitle = ResourcesHelper.Instance.GetString("releaseNotes_title");
            if (string.IsNullOrEmpty(_selectReleaseNotesTitle)) _selectReleaseNotesTitle = "Select.Release.Notes.file";
     
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

            switch (e.PropertyName)
            {
                case "RefreshGuid":
                    _viewModel.ProductIdGuid = Guid.NewGuid().ToString();
                    break;

                case "BrowseIcon":
                    _viewModel.IconPath = GetIcon();
                    break;

                case "BrowsePreviewImage":
                    _viewModel.PreviewImagePath = GetPreviewImage();
                    break;

                case "BrowseReleaseNotes":
                    _viewModel.ReleaseNotesPath = GetReleaseNotes();
                    break;

                case "BrowseGettingStartedGuide":
                    _viewModel.GettingStartedGuidePath = GetGettingStartedGuide();
                    break;

                case "BrowseLicense":
                    _viewModel.LicensePath = GetLicense();
                    break;

                case "GetProjectCsPath":
                    _viewModel.ProjectCsPath = GetProjectCsPath();
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
                _allFiles + "|*.html;*.htm;*.txt;*.rtf",
                _webFile + "|*.html;*.htm",
                _textFile + "|*.txt",
                _wordFile + "|*.rtf",
            };
            return ExplorerHelper.SelectFileInExplorer(_selectReleaseNotesTitle, lst);
        }

        public string GetGettingStartedGuide()
        {
            var lst = new List<string>
            {
                _allFiles + "|*.html;*.htm;*.txt;*.rtf;*.doc;*.docx",
                _webFile + "|*.html;*.htm",
                _textFile + "|*.txt",
                _wordFile + "|*.rtf;*.doc;*.docx"
            };
           return ExplorerHelper.SelectFileInExplorer(_gettingStartedTitle, lst);
        }
        
        public string GetLicense()
        {
            var lst = new List<string>
            {
                _allFiles + "|*.txt;*.rtf",
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
            var frm = new ManifestWindow();
            frm.ShowDialog();
        }
        
        #region file sampler [Content_Types].xml, extension.vsixmanifest
        /* file 
         * [Content_Types].xml - static
          
            <?xml version="1.0" encoding="utf-8"?>
            <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
	            <Default Extension="zip" ContentType="application/zip" /><Default Extension="dll" ContentType="application/octet-stream" />
	            <Default Extension="vsixmanifest" ContentType="text/xml" />
            </Types>    
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
