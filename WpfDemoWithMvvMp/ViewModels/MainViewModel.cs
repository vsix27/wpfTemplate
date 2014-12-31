using System;
using System.Windows.Input;
using System.ComponentModel;
using WpfDemoWithMvvmp.Infrastructure;
using WpfDemoWithMvvmp.Presenters;

namespace WpfDemoWithMvvmp.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private MainViewPresenter _presenter;
    
        public MainViewModel()
        {
            _presenter = new MainViewPresenter(this);
            InitResources();
        }

        private void InitResources()
        {
            TextSelectInputFolder = "select folder";
            TextProcessFiles = "process files";

            var outlookfiles = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Outlook Files");
            outlookfiles = System.IO.Path.Combine(outlookfiles, "NotesMAC");
            FolderPath = (System.IO.Directory.Exists(outlookfiles))
                ? outlookfiles
                : FolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            OutputText = "............... 2 " + DateTime.Now;
            TextGetTime = "Clear Output";
        }

        #region properties  
           
        public string TextSelectInputFolder { get; set; }
        public string TextProcessFiles { get; set; }
        public string TextGetTime { get; set; }

        #endregion

        #region binded properties 

        private string _error;
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

        public bool ShowErrorText { get; set; }

        private string _txtFolderPath;
        public string FolderPath
        {
            get { return _txtFolderPath; }
            set
            {
                _txtFolderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        private string _txtOutputText;
        public string OutputText
        {
            get { return _txtOutputText; }
            set
            {
                _txtOutputText = value;
                OnPropertyChanged("OutputText");
            }
        }

        #endregion

        #region commands
          
        // long process in the thread
        public ICommand CommandProcessFiles
        {
            get { return new RelayCommand(ProcessFiles); }
        }

        // call presenter 
        public void ProcessFiles()
        {
            OnPropertyChanged("ProcessFiles");
        }

        // short process without thread
        public ICommand CommandGetTime
        {
            get { return new RelayCommand(GetTime); }
        }

        // call presenter 
        public void GetTime()
        {
            OnPropertyChanged("GetTime");
        }

        // short process without thread
        public ICommand CommandSelectInputFolder
        {
            get { return new RelayCommand(SelectInputFolder); }
        }

        // call presenter 
        public void SelectInputFolder()
        {
            OnPropertyChanged("SelectInputFolder");
        }
        #endregion
    }
}
