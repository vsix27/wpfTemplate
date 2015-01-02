using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Vsix.Common.Helpers;
using Vsix.Viewer.Helpers;
using Vsix.Viewer.Infrastructure;
using Vsix.Viewer.Presenters;

namespace Vsix.Viewer.ViewModels
{
    public class RegistryPackageModel : ObservableObject, IRegistryPackageModel
    {
        private RegistryPackagePresenter _presenter;
        private string _visualStudioVersion;
        private List<string> _visualStudioVersions;
        private List<RegistryPackage> _registryPackagesData;

        public RegistryPackageModel()
        {
            //LogHelper.LogEnter();
            _presenter = new RegistryPackagePresenter(this);
            InitResources();
            //LogHelper.LogExit();
        }

        internal void InitResources()
        {
            VisualStudioVersionLabel = ResourcesHelper.Instance.GetString("TestVisualStudioVersionLabel");

            RegistryPackagesData = new List<RegistryPackage>();

            //InitSampleRegistryPackagesData(5);
            InitRegistryPackagesData();
            
            RegistryPackagesData =
                RegistryPackagesDataAll.Where(o => o.VisualStudioVersion.Equals(VisualStudioVersion)).ToList();
        }

        private void InitRegistryPackagesData()
        {
            // get registry data
            VisualStudioVersions = new List<string>();
            var sKeys = RegistryPackage.GetRegistryPackagesPath();
            
            if (RegistryPackagesDataAll == null)
                RegistryPackagesDataAll = new List<RegistryPackage>();

            foreach (string sPackage in sKeys)
            {
                if (RegistryHelper.GetRegistryPath(sPackage) == null) continue;
                // guids of packages
                int k = 0;
                foreach (var subKey in RegistryHelper.GetRegistryPath(sPackage).GetSubKeyNames())
                {
                    var rk = RegistryHelper.GetRegistryPath(sPackage + "\\" + subKey);
                    var rkp = new RegistryPackage(rk);

                    if (string.IsNullOrEmpty(rkp.ProductName) || string.IsNullOrEmpty(rkp.ProductVersion) ||
                        string.IsNullOrEmpty(rkp.MinEdition)) continue;
                    rkp.N = k++;

                    if (!VisualStudioVersions.Contains(rkp.VisualStudioVersion)) VisualStudioVersions.Add(rkp.VisualStudioVersion);
                    RegistryPackagesDataAll.Add(rkp);
                }
            }
            VisualStudioVersion = VisualStudioVersions.FirstOrDefault();
        }

        private void InitSampleRegistryPackagesData(int rows = 7)
        {
            VisualStudioVersions = new List<string>();

            foreach (string s in new[] {"10", "11", "12"})
            {
                VisualStudioVersions.Add(s);

                if (rows < 1) rows = 7;
                if (rows > 100) rows = 27;
                if (RegistryPackagesDataAll == null)
                    RegistryPackagesDataAll = new List<RegistryPackage>();

                for (int k = 0; k < rows; k++)
                    RegistryPackagesDataAll.Add(new RegistryPackage(null)
                    {
                        CodeBase = "CodeBase " + s + k,
                        Default = "Default " + s + k,
                        MinEdition = "1." + s + k,
                        ProductName = "ProductName " + s + k,
                        ProductVersion = "ProductVersion " + s + k,
                        RegistryClass = "RegistryClass " + s + k,
                        RegistryGuid = "RegistryGuid " + s + k,
                        RegistryPath = "RegistryPath " + s + k,
                        VisualStudioVersion = s,
                        N = k,
                    });
            }
            VisualStudioVersion = VisualStudioVersions.FirstOrDefault();
        }

        public List<RegistryPackage> RegistryPackagesData
        {
            get { return _registryPackagesData; }
            set
            {
                _registryPackagesData = value;
                OnPropertyChanged("RegistryPackagesData");
            }
        }

        public List<RegistryPackage> RegistryPackagesDataAll { get; set; }

        public string VisualStudioVersionLabel { get; set; }

        public string VisualStudioVersion
        {
            get { return _visualStudioVersion; }
            set
            {
                _visualStudioVersion = value;
                //OnPropertyChanged("VisualStudioVersion");
                // refill datagrid
                RegistryPackagesData =
                    RegistryPackagesDataAll.Where(o => o.VisualStudioVersion.Equals(VisualStudioVersion)).ToList();
            }
        }

        public List<string> VisualStudioVersions
        {
            get { return _visualStudioVersions; }
            set
            {
                _visualStudioVersions = value;
                OnPropertyChanged("VisualStudioVersions");
            }
        }
        
    }
}