using System.Windows.Input;
using Vsix.Common.Helpers;
using Vsix.Viewer.Presenters;
using Vsix.Viewer.Infrastructure;
using Vsix.Viewer.Views;
using ResourcesHelper = Vsix.Viewer.Helpers.ResourcesHelper;

namespace Vsix.Viewer.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private MainViewPresenter _presenter;

        public MainViewModel()
        {
            //LogHelper.LogEnter();
            _presenter = new MainViewPresenter(this);
            InitResources();
            //LogHelper.LogExit();
        }

        internal void InitResources()
        {
            TextAbout = ResourcesHelper.Instance.GetString("menuAbout");
            TextErrorReportVsix = ResourcesHelper.Instance.GetString("menuErrorReport");
            TextOptions = ResourcesHelper.Instance.GetString("menuOptions");
            TextSaveVsix = ResourcesHelper.Instance.GetString("menuSaveVsix");
            TextExit = ResourcesHelper.Instance.GetString("menuExit");
            TextOptionsInstalledVsix = ResourcesHelper.Instance.GetString("menuAboutInstalledVsix");
            TextOptionsRemoveVsix = ResourcesHelper.Instance.GetString("menuAboutRemoveVsix");
            TextAboutVsix = ResourcesHelper.Instance.GetString("menuAboutVsix");
            TextNewVsix = ResourcesHelper.Instance.GetString("menuNewVsix");
            TextOpenVsix = ResourcesHelper.Instance.GetString("menuOpenVsix");
            TextViewLog = ResourcesHelper.Instance.GetString("menuViewLog");
            TextViewBindings = ResourcesHelper.Instance.GetString("menuViewBindings");
            TextViewStyles = ResourcesHelper.Instance.GetString("menuViewStyles");
            TextOpenVsixAny = ResourcesHelper.Instance.GetString("menuTextOpenVsixAny");
            TextOpenVsixPrj = ResourcesHelper.Instance.GetString("menuTextOpenVsixPrj");
            VsixPackage = ResourcesHelper.Instance.GetString("vsix_package");
            
            TextProcessFiles = "process files";
            
#if DEBUG 
            VisibilityViewLog = true;
            VisibilityViewBindings = true;
            VisibilityViewStyles = true;
#endif

            //var outlookfiles = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Outlook Files");
            //outlookfiles = System.IO.Path.Combine(outlookfiles, "NotesMAC");
            //FolderPath = (System.IO.Directory.Exists(outlookfiles))
            //    ? outlookfiles
            //    : FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //OutputText = "............... 2 " + DateTime.Now;
        }

        #region properties  

        public bool VisibilityViewLog { get; set; }
        public bool VisibilityViewBindings { get; set; }
        public bool VisibilityViewStyles { get; set; }


        public string TextSetLanguage { get; set; }

        public string TextOptions
        {
            get { return _textOptions; }
            set { _textOptions = value; OnPropertyChanged("TextOptions"); }
        }

        public string TextSaveVsix
        {
            get { return _textSaveVsix; }
            set { _textSaveVsix = value; OnPropertyChanged("TextSaveVsix"); }
        }

        public string TextOptionsInstalledVsix
        {
            get { return _textOptionsInstalledVsix; }
            set { _textOptionsInstalledVsix = value; OnPropertyChanged("TextOptionsInstalledVsix"); }
        }

        public string TextOptionsRemoveVsix
        {
            get { return _textOptionsRemoveVsix; }
            set { _textOptionsRemoveVsix = value; OnPropertyChanged("TextOptionsRemoveVsix"); }
        }

        public string TextAboutVsix
        {
            get { return _textAboutVsix; }
            set { _textAboutVsix = value; OnPropertyChanged("TextAboutVsix"); }
        }

        public string TextNewVsix
        {
            get { return _textNewVsix; }
            set { _textNewVsix = value; OnPropertyChanged("TextNewVsix"); }
        }

        public string TextOpenVsix
        {
            get { return _textOpenVsix; }
            set { _textOpenVsix = value; OnPropertyChanged("TextOpenVsix"); }
        }

        public string TextViewLog
        {
            get { return _textViewLog; }
            set { _textViewLog = value; OnPropertyChanged("TextViewLog"); }
        }

        public string TextOpenVsixAny { get; set; }
        public string TextOpenVsixPrj { get; set; }

        public string TextProcessFiles { get; set; }
        public string VsixPackage { get; set; }
        
        public string TextAbout
        {
            get { return _textAbout; }
            set { _textAbout = value; OnPropertyChanged("TextAbout"); }
        }

        public string TextErrorReportVsix
        {
            get { return _textErrorReportVsix; }
            set { _textErrorReportVsix = value; OnPropertyChanged("TextErrorReportVsix"); }
        }
       
        public string TextExit
        {
            get { return _textExit; }
            set { _textExit = value; OnPropertyChanged("TextExit"); }
        }

        public string TextViewBindings
        {
            get { return _textViewBindings; }
            set { _textViewBindings = value; OnPropertyChanged("TextViewBindings"); }
        }
        
        #endregion

        #region binded properties 

        public string VsixPath { get; set; }
        public string VsixPathUnpacked { get; set; }
        public bool ShowErrorText { get; set; }
        
        private string _error;
        private string _txtOutputText, _textSaveVsix, _textOptionsInstalledVsix, _textOptionsRemoveVsix;
        private string _textNewVsix, _textAboutVsix, _textExit, _textAbout, _textOpenVsix, _textViewLog;
        private string _textViewStyles, _textViewBindings, _txtFolderPath, _textErrorReportVsix;

        public string TextViewStyles
        {
            get { return _textViewStyles; }
            set { _textViewStyles = value; OnPropertyChanged("TextViewStyles"); }
        }
        public string ErrorText
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged("ErrorText");

                ShowErrorText = !string.IsNullOrEmpty(_error);
                OnPropertyChanged("ShowErrorText");
            }
        }
        
        public string FolderPath
        {
            get { return _txtFolderPath; }
            set { _txtFolderPath = value; OnPropertyChanged("FolderPath"); }
        }

        public string OutputText
        {
            get { return _txtOutputText; }
            set { _txtOutputText = value; OnPropertyChanged("OutputText"); }
        }

        #endregion

        #region commands
          
        public ICommand CommandProcessFiles{get { return new RelayCommand(ProcessFiles); }}
        public void ProcessFiles(){OnPropertyChanged("ProcessFiles");}



        public ICommand CommandViewLog { get { return new RelayCommand(ViewLog); } }
        public void ViewLog(){ OnPropertyChanged("ViewLog"); }

        public ICommand CommandAbout{get { return new RelayCommand(About); }}
        public void About(){OnPropertyChanged("About");}

        public ICommand CommandViewBindings{get { return new RelayCommand(ViewBindings); }}
        public void ViewBindings(){OnPropertyChanged("ViewBindings");}

        public ICommand CommandViewStyles { get { return new RelayCommand(ViewStyles); } }
        public void ViewStyles() { OnPropertyChanged("ViewStyles"); }

        public ICommand CommandNewVsix { get { return new RelayCommand(NewVsix); } }

        private IManifestModel _manifestModel = new ManifestModel();
        private string _textOptions;

        public void NewVsix()
        {
            ////OnPropertyChanged("NewVsix");
            string csproj = new ManifestPresenter(_manifestModel).GetProjectCsPath();
            var manifestWindow = new ManifestWindow(csproj);
         
            bool? b = manifestWindow.ShowDialog();
            if (b.HasValue && (bool)b)
            {
                _manifestModel = manifestWindow.DataContext as ManifestModel;
            }
        }

        public ICommand CommandOpenVsix { get { return new RelayCommand(OpenVsix); } }
        public void OpenVsix() { OnPropertyChanged("OpenVsix"); }

        public ICommand CommandOpenVsixPrj { get { return new RelayCommand(OpenVsixPrj); } }
        public void OpenVsixPrj() { OnPropertyChanged("OpenVsixPrj"); }
        

        // short process without thread
        public ICommand CommandSaveVsix{ get { return new RelayCommand(SaveVsix); } }
        // call presenter 
        public void SaveVsix(){ OnPropertyChanged("SaveVsix"); }

        public ICommand CommandErrorReport{ get { return new RelayCommand(ErrorReport); } }
        public void ErrorReport(){ OnPropertyChanged("ErrorReport"); }

        
        #endregion
    }
}
