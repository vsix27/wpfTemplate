using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            _releaseNotesPath,
            _selectedManifestVersion;

        private List<string> _manifestVersions = new List<string>();

        private ManifestPresenter _presenter;

        #region labels
        public string TextProductNameLabel { get; set; }
        public string TextAuthorLabel { get; set; }
        public string TextProductIDLabel { get; set; }
        public string TextRefreshGuidLabel { get; set; }
        public string TextVersionLabel { get; set; }
        public string TextManifestVersionLabel { get; set; }
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


        public List<string> ManifestVersions { get; set; }
        //{
        //    get { return _manifestVersions; }
        //    set { _manifestVersions = value; OnPropertyChanged("ManifestVersions"); }
        //}
        
        public string ManifestVersion
        {
            get { return _selectedManifestVersion; }
            set { _selectedManifestVersion = value; OnPropertyChanged("ManifestVersion"); }
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
            TextManifestVersionLabel = ResourcesHelper.Instance.GetString("TextManifestVersionLabel");

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

            ManifestVersions = new List<string> {"1.0", "2.0"};
            ManifestVersion = ManifestVersions.Last();
        }

        public string ModelToSourceExtensionManifestV1()
        {
            string s = null;
            try
            {
                var doc = new XmlDocument( );
                doc.LoadXml(GetManifestTemplateV1(false));
                var ndIdentity = doc.SelectSingleNode("//Identifier");
                XmlHelper.SetNodeValue(ndIdentity, "@Id", ProductId);

                XmlHelper.SetNodeValue(doc, "//Name", ProductName);
                XmlHelper.SetNodeValue(doc, "//Version", Version);
                XmlHelper.SetNodeValue(doc, "//Author", Author);
                XmlHelper.SetNodeValue(doc, "//Description", Description);

                XmlHelper.SetNodeValue(doc, "//MoreInfoUrl", MoreInfoUrl, true);
                XmlHelper.SetNodeValue(doc, "//License", LicensePath, true);
                XmlHelper.SetNodeValue(doc, "//GettingStartedGuide", GettingStartedGuidePath, true);
                XmlHelper.SetNodeValue(doc, "//ReleaseNotes", ReleaseNotesPath, true);
                XmlHelper.SetNodeValue(doc, "//Icon", IconPath, true);
                XmlHelper.SetNodeValue(doc, "//PreviewImage", PreviewImagePath, true);
                XmlHelper.SetNodeValue(doc, "//Tags", Tags, true);

                s = XmlHelper.IndentXml(doc);
                s = SetManifestTemplateV12Namespaces(s);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return s;
        }

        public string ModelToSourceExtensionManifestV2()
        {
            string s = null;
            try
            {
                //var doc = XmlHelper.XmlLoadNoNamespaces(GetManifestTemplateV2());
                var doc = new XmlDocument();
                doc.LoadXml(GetManifestTemplateV2(false));

                var ndIdentity = doc.SelectSingleNode("//Identity");
                XmlHelper.SetNodeValue(ndIdentity, "@Id", ProductId);
                XmlHelper.SetNodeValue(ndIdentity, "@Version", Version);
                XmlHelper.SetNodeValue(ndIdentity, "@Publisher", Author);
                XmlHelper.SetNodeValue(doc, "//DisplayName", ProductName);
                XmlHelper.SetNodeValue(doc, "//Description", Description);
                XmlHelper.SetNodeValue(doc, "//MoreInfo", MoreInfoUrl, true);
                XmlHelper.SetNodeValue(doc, "//License", LicensePath, true);
                XmlHelper.SetNodeValue(doc, "//GettingStartedGuide", GettingStartedGuidePath, true);
                XmlHelper.SetNodeValue(doc, "//ReleaseNotes", ReleaseNotesPath, true);
                XmlHelper.SetNodeValue(doc, "//Icon", IconPath, true);
                XmlHelper.SetNodeValue(doc, "//PreviewImage", PreviewImagePath, true);
                XmlHelper.SetNodeValue(doc, "//Tags", Tags, true);

                s = XmlHelper.IndentXml(doc);
                s = SetManifestTemplateV12Namespaces(s);
                s = s.Replace("d_Source=", "d:Source=");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return s;
        }


        public const string MnfstV1Nms = "xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='http://schemas.microsoft.com/developer/vsx-schema/2010'";
        public const string MnfstV2Nms = "xmlns='http://schemas.microsoft.com/developer/vsx-schema/2011'  xmlns:d='http://schemas.microsoft.com/developer/vsx-schema-design/2011'";

        /// <summary>
        /// add namespaces to xml string, based on Manifest version 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string SetManifestTemplateV12Namespaces(string s)
        {
            if (s.Contains("<PackageManifest>"))
                return s.Replace("<PackageManifest>", "<PackageManifest Version='2.0.0' " + MnfstV2Nms + ">");
            if (s.Contains("<PackageManifest "))
                return s.Replace("<PackageManifest", "<PackageManifest " + MnfstV2Nms);
            if (s.Contains("<Vsix>"))
                return s.Replace("<Vsix>", "<Vsix Version='1.0.0' " + MnfstV1Nms + ">");
            if (s.Contains("<Vsix "))
                return s.Replace("<Vsix", "<Vsix " + MnfstV1Nms);
            return s;
        }

        /// <summary>
        /// empty manifest xml string for Manifest version 1 - Vsix root node
        /// </summary>
        /// <param name="useNameSpace"></param>
        /// <returns></returns>
        public string GetManifestTemplateV1(bool useNameSpace)
        {
            string s = @"<?xml version='1.0' encoding='utf-8'?>";

            if (useNameSpace)
                s += @"<Vsix Version='1.0.0' " + MnfstV1Nms + ">";
            else
                s += @"<Vsix Version='1.0.0'>";

            s += @"
<Identifier Id=''>
    <Name/>
    <Author/>
    <Version/>
    <Description xml:space='preserve'/>
    <Locale>1033</Locale>
    <MoreInfoUrl/>
    <License/>
    <Icon/>
    <PreviewImage/>
    <InstalledByMsi>false</InstalledByMsi>
    <SupportedProducts>
      <VisualStudio Version='10.0'><Edition>Ultimate</Edition><Edition>Premium</Edition><Edition>Pro</Edition><Edition>Express_All</Edition></VisualStudio>
      <VisualStudio Version='11.0'><Edition>Ultimate</Edition><Edition>Premium</Edition><Edition>Pro</Edition><Edition>Express_All</Edition></VisualStudio>
      <VisualStudio Version='12.0'><Edition>Ultimate</Edition><Edition>Premium</Edition><Edition>Pro</Edition><Edition>Express_All</Edition></VisualStudio>
      <VisualStudio Version='13.0'><Edition>Ultimate</Edition><Edition>Premium</Edition><Edition>Pro</Edition><Edition>Express_All</Edition></VisualStudio>
      <VisualStudio Version='14.0'><Edition>Ultimate</Edition><Edition>Premium</Edition><Edition>Pro</Edition><Edition>Express_All</Edition></VisualStudio>
    </SupportedProducts>
    <SupportedFrameworkRuntimeEdition  MinVersion='2.0' MaxVersion='4.6'  />
  </Identifier>
  <References>
    <Reference Id='Microsoft.VisualStudio.MPF' MinVersion='10.0'>
      <Name>Visual Studio MPF</Name>
    </Reference>
  </References>
  <Content>
    <VsPackage>|%CurrentProject%;PkgdefProjectOutputGroup|</VsPackage>
    <MefComponent>|%CurrentProject%;PkgdefProjectOutputGroup|</MefComponent>
  </Content>
</Vsix>";
            return s;
        }
       
        /// <summary>
        /// empty manifest xml string for Manifest version 2 - PackageManifest root node
        /// </summary>
        /// <param name="useNameSpace"></param>
        /// <returns></returns>
        public string GetManifestTemplateV2(bool useNameSpace = false)
        {
            string s = @"<?xml version='1.0' encoding='utf-8'?>";
            if (useNameSpace)
                s += @"<PackageManifest Version='2.0.0' " + MnfstV2Nms + ">";
            else
                s += @"<PackageManifest Version='2.0.0'>";

            s += @"
  <Metadata>
    <Identity Id='' Version='' Language='en-US' Publisher='' />
    <DisplayName/>
    <Description/>
    <MoreInfo/>
    <License/>
    <GettingStartedGuide/>
    <ReleaseNotes/>
    <Icon/>
    <PreviewImage/>
    <Tags/>
  </Metadata>
  <Installation>
    <InstallationTarget Id='Microsoft.VisualStudio.Pro' Version='[12.0,)' />
  </Installation>
  <Dependencies>
    <Dependency Id='Microsoft.Framework.NDP' DisplayName='Microsoft .NET Framework' d:Source='Manual' Version='[3.5,)' />
  </Dependencies>
</PackageManifest>";
            if (!useNameSpace) 
                s = s.Replace("d:Source='Manual'", "d_Source='Manual'");
            return s;
        }

        /// <summary> values of all properties with values </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // return ToStrings().Aggregate(string.Empty, (current, c) => current + (c + Environment.NewLine));
            return ToStringPairs().Aggregate(string.Empty, (current, c) => current + (c.Key + " " + c.Value + Environment.NewLine));
        }

        /// <summary> list of all properties with values </summary>
        /// <returns></returns>
        public List<string> ToStrings()
        {
            return ToStringPairs().Select(kvp => kvp.Key + " " + kvp.Value).ToList();
        }

        public Dictionary<string, string> ToStringPairs()
        {
            return new Dictionary<string, string>
            {
                {"ProjectCsPath", ProjectCsPath},
                {"ProjectPath", ProjectPath},
                {"ProductName", ProductName},
                {"Author", Author},
                {"ProductId", ProductId},
                {"Version", Version},
                {"Description", Description},
                {"LicensePath", LicensePath},
                {"IconPath", IconPath},
                {"PreviewImagePath", PreviewImagePath},
                {"Tags", Tags},
                {"ReleaseNotesPath", ReleaseNotesPath},
                {"GettingStartedGuidePath", GettingStartedGuidePath},
                {"MoreInfoUrl", MoreInfoUrl},
                {"ManifestVersion", ManifestVersion}
            };
        }
    }
}
