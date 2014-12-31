using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Xml;
using Vsix.Common.Helpers;
using Vsix.Viewer.Presenters;
using Vsix.Viewer.Infrastructure;
using ResourcesHelper = Vsix.Viewer.Helpers.ResourcesHelper;

namespace Vsix.Viewer.ViewModels
{
    public class ManifestModel : ObservableObject, IManifestModel
    {
        private string _projectPath,
            _projectCsPath,
            _productName,
            _author,
            _version,
            _moreInfoUrl,
            _tags,
            _previewImagePath,
            _iconPath,
            _licensePath,
            _description,
            _productId,
            _productIdGuid,
            _gettingStartedGuidePath,
            _releaseNotesPath;

        private ManifestPresenter _presenter;

        #region labels
        public string TextProductNameLabel { get; set; }
        public string TextAuthorLabel { get; set; }
        public string TextProductIDLabel { get; set; }
        public string TextRefreshGuidLabel { get; set; }
        public string TextVersionLabel { get; set; }
        public string TextProjectCsPathLabel { get; set; }
        public string TextBrowseLabel { get; set; }
        public string TextMetadataLabel { get; set; }
        public string TextVsixDescriptionLabel { get; set; }
        public string TextVsixLicenseLabel { get; set; }
        public string TextVsixIconLabel { get; set; }
        public string TextVsixPreviewImageLabel { get; set; }
        public string TextVsixTagsLabel { get; set; }
        public string TextVsixReleaseNotesLabel { get; set; }
        public string TextVsixGettingStartedGuideLabel { get; set; }
        public string TextVsixMoreInfoURLLabel { get; set; }
        #endregion

        #region binded properties

        public string ProjectCsPath
        {
            get { return _projectCsPath; }
            set { _projectCsPath = value; OnPropertyChanged("ProjectCsPath"); }
        }

        public string ProjectPath
        {
            get { return _projectPath; }
            set { _projectPath = value; OnPropertyChanged("ProjectPath"); }
        }

        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                ProductId = _productName + ".." + _productIdGuid;
                OnPropertyChanged("ProductName");
            }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; OnPropertyChanged("Author"); }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; OnPropertyChanged("Version"); }
        }

        public string ProductId
        {
            get { return _productId; }
            set { _productId = value; OnPropertyChanged("ProductId"); }
        }

        public string ProductIdGuid
        {
            get { return _productIdGuid; }
            set
            {
                _productIdGuid = value;
                ProductId = _productName + ".." + _productIdGuid;
                OnPropertyChanged("ProductIdGuid");
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }

        public string LicensePath
        {
            get { return _licensePath; }
            set { _licensePath = value; OnPropertyChanged("LicensePath"); }
        }

        public string IconPath
        {
            get { return _iconPath; }
            set { _iconPath = value; OnPropertyChanged("IconPath"); }
        }

        public string PreviewImagePath
        {
            get { return _previewImagePath; }
            set { _previewImagePath = value; OnPropertyChanged("PreviewImagePath"); }
        }

        public string Tags
        {
            get { return _tags; }
            set { _tags = value; OnPropertyChanged("Tags"); }
        }

        public string ReleaseNotesPath
        {
            get { return _releaseNotesPath; }
            set { _releaseNotesPath = value; OnPropertyChanged("ReleaseNotesPath"); }
        }

        public string GettingStartedGuidePath
        {
            get { return _gettingStartedGuidePath; }
            set { _gettingStartedGuidePath = value; OnPropertyChanged("GettingStartedGuidePath"); }
        }

        public string MoreInfoUrl
        {
            get { return _moreInfoUrl; }
            set { _moreInfoUrl = value; OnPropertyChanged("MoreInfoUrl"); }
        }

        #endregion

        #region commands

        public ICommand CommandRefreshGuid { get { return new RelayCommand(RefreshGuid); } }
        public void RefreshGuid() { OnPropertyChanged("RefreshGuid"); }


        public ICommand CommandProjectCsPath { get { return new RelayCommand(GetProjectCsPath); } }
        public void GetProjectCsPath() { OnPropertyChanged("GetProjectCsPath"); }


        public ICommand CommandCheckManifest { get { return new RelayCommand(CheckManifest); } }
        public void CheckManifest() { OnPropertyChanged("CheckManifest"); }


        public ICommand CommandBrowseLicense { get { return new RelayCommand(BrowseLicense); } }
        public void BrowseLicense() { OnPropertyChanged("BrowseLicense"); }


        public ICommand CommandBrowseIcon { get { return new RelayCommand(BrowseIcon); } }
        public void BrowseIcon() { OnPropertyChanged("BrowseIcon"); }


        public ICommand CommandBrowsePreviewImage { get { return new RelayCommand(BrowsePreviewImage); } }
        public void BrowsePreviewImage() { OnPropertyChanged("BrowsePreviewImage"); }

        public ICommand CommandBrowseReleaseNotes { get { return new RelayCommand(BrowseReleaseNotes); } }
        public void BrowseReleaseNotes() { OnPropertyChanged("BrowseReleaseNotes"); }

        
        public ICommand CommandBrowseGettingStartedGuide { get { return new RelayCommand(BrowseGettingStartedGuide); } }
        public void BrowseGettingStartedGuide() { OnPropertyChanged("BrowseGettingStartedGuide"); }

        public ICommand CommandManifestSave { get { return new RelayCommand(ManifestSave); } }
        public void ManifestSave() { OnPropertyChanged("ManifestSave"); }
        

        public ICommand CommandManifestCancel { get { return new RelayCommand(ManifestCancel); } }
        public void ManifestCancel() { OnPropertyChanged("ManifestCancel"); }

        #endregion

        public ManifestModel()
        {
            _presenter = new ManifestPresenter(this);
            InitResources();
            InitResourcesValues();
        }

        internal void InitResourcesValues()
        {
            Version = "1.0";
            ProductIdGuid = Guid.NewGuid().ToString();
            Author = Environment.UserName;
        }

        internal void InitResources()
        {
            TextBrowseLabel = ResourcesHelper.Instance.GetString("TextBrowseLabel");

            TextProductNameLabel = ResourcesHelper.Instance.GetString("TextProductNameLabel");
            TextProductIDLabel = ResourcesHelper.Instance.GetString("TextProductIDLabel");
            TextAuthorLabel = ResourcesHelper.Instance.GetString("TextAuthorLabel");
            TextVersionLabel = ResourcesHelper.Instance.GetString("TextVersionLabel");

            TextProjectCsPathLabel = ResourcesHelper.Instance.GetString("TextProjectCsPathLabel");
            TextMetadataLabel = ResourcesHelper.Instance.GetString("TextMetadataLabel");
            TextRefreshGuidLabel = ResourcesHelper.Instance.GetString("TextRefreshGuidLabel");
            TextVsixDescriptionLabel = ResourcesHelper.Instance.GetString("TextVsixDescriptionLabel");

            TextVsixLicenseLabel = ResourcesHelper.Instance.GetString("TextVsixLicenseLabel");
            TextVsixIconLabel = ResourcesHelper.Instance.GetString("TextVsixIconLabel");
            TextVsixPreviewImageLabel = ResourcesHelper.Instance.GetString("TextVsixPreviewImageLabel");
            TextVsixTagsLabel = ResourcesHelper.Instance.GetString("TextVsixTagsLabel");
            TextVsixReleaseNotesLabel = ResourcesHelper.Instance.GetString("TextVsixReleaseNotesLabel");
            TextVsixGettingStartedGuideLabel = ResourcesHelper.Instance.GetString("TextVsixGettingStartedGuideLabel");
            TextVsixMoreInfoURLLabel = ResourcesHelper.Instance.GetString("TextVsixMoreInfoURLLabel");
        }
        
        public string ModelToSourceExtensionManifestV2()
        {
            string s = null;
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(GetInitSample());
                s = doc.OuterXml;
                var ndIdentity = doc.SelectSingleNode("//Identity");
                SetNodeValue(ndIdentity, "@Id", ProductId);
                SetNodeValue(ndIdentity, "@Version", Version);
                SetNodeValue(ndIdentity, "@Publisher", Author);
                SetNodeValue(doc, "//DisplayName", ProductName);
                SetNodeValue(doc, "//Description", Description);
                SetNodeValue(doc, "//MoreInfo", MoreInfoUrl);
                SetNodeValue(doc, "//License", LicensePath);
                SetNodeValue(doc, "//GettingStartedGuide", GettingStartedGuidePath);
                SetNodeValue(doc, "//ReleaseNotes", ReleaseNotesPath);
                SetNodeValue(doc, "//Icon", IconPath);
                SetNodeValue(doc, "//PreviewImage", PreviewImagePath);
                SetNodeValue(doc, "//Tags", Tags);

                s = doc.OuterXml;

                doc = null;
            }
            catch (Exception ex){LogHelper.LogError(ex);}
            return s;
        }

        private void SetNodeValue(XmlNode nd, string xPath, string ndValue)
        {
            if (nd == null) return;
            try
            {
                var ndSel = nd.SelectSingleNode(xPath);
                if (ndSel == null) return;
                ndSel.Value = ndValue;
            }
            catch (Exception ex) { LogHelper.LogError(ex); }
        }

        public string GetInitSample()
        {
            string s = @"<?xml version='1.0' encoding='utf-8'?>
<PackageManifest Version='2.0.0' xmlns='http://schemas.microsoft.com/developer/vsx-schema/2011' xmlns:d='http://schemas.microsoft.com/developer/vsx-schema-design/2011'>
  <Metadata>
    <Identity Id='VSIXProject1..7e1deb4c-03be-4f6c-bf09-e1e909406191' Version='1.0' Language='en-US' Publisher='xx' />
    <DisplayName>VSIXProject1</DisplayName>
    <Description>Empty VSIX Project.</Description>
    <MoreInfo>http://www.rrrr.dgfhsdfg</MoreInfo>
    <License>New Text Document.txt</License>
    <GettingStartedGuide>New Text Document.txt</GettingStartedGuide>
    <ReleaseNotes>New Text Document.txt</ReleaseNotes>
    <Icon>csharp2.png</Icon>
    <PreviewImage>csharp2.png</PreviewImage>
    <Tags>vsix, template</Tags>
  </Metadata>
  <Installation>
    <InstallationTarget Id='Microsoft.VisualStudio.Pro' Version='[12.0]' />
  </Installation>
  <Dependencies>
    <Dependency Id='Microsoft.Framework.NDP' DisplayName='Microsoft .NET Framework' d:Source='Manual' Version='[4.5,)' />
  </Dependencies>
</PackageManifest>";
            return s;
        }

        /// <summary> values of all properties with values </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToStrings().Aggregate(string.Empty, (current, c) => current + (c + Environment.NewLine));
        }

        /// <summary> list of all properties with values </summary>
        /// <returns></returns>
        public List<string> ToStrings()
        {
            return new List<string>
            {
                "ProjectCsPath " + ProjectCsPath,
                "ProjectPath " + ProjectPath,
                "ProductName " + ProductName,
                "Author " + Author,
                "ProductId " + ProductId,
                "Version " + Version,
                "Description " + Description,
                "LicensePath " + LicensePath,
                "IconPath " + IconPath,
                "PreviewImagePath " + PreviewImagePath,
                "Tags " + Tags,
                "ReleaseNotesPath " + ReleaseNotesPath,
                "GettingStartedGuidePath " + GettingStartedGuidePath,
                "MoreInfoUrl " + MoreInfoUrl
            };
        }
    }
}
