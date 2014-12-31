using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using WpfDemoWithMvvmp.ViewModels;
using System.Collections.Generic;

namespace WpfDemoWithMvvmp.Presenters
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

                case "ProcessFiles":
                    _bgWorker.RunWorkerAsync();
                    break;

                case "GetTime":
                    _viewModel.OutputText = DateTime.Now +_nl;
                    break;                               

                case "SelectInputFolder":
                    SelectInputFolder();
                    break;               

                default:
                    break;
            }
        }

        public string _nl = Environment.NewLine;

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
