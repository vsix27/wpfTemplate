using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Vsix.Viewer.Infrastructure;
using Vsix.Viewer.Presenters;

namespace Vsix.Viewer.ViewModels
{
    public class VsixDetailsModel : ObservableObject, IVsixDetailsModel
    {
        private VsixDetailsPresenter _presenter;
        private string _txtOutputText;

        public VsixDetailsModel()
        {
            //LogHelper.LogEnter();
            _presenter = new VsixDetailsPresenter(this);
            //InitResources();
            //LogHelper.LogExit();
        }
        public string OutputText
        {
            get { return _txtOutputText; }
            set { _txtOutputText = value; OnPropertyChanged("OutputText"); }
        }

    }
}
