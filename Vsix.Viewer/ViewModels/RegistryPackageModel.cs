﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private BindingList<RegistryPackage> _registryPackagesData;
        private BindingList<ManifestPackage > _manifestPackagesData;

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

            RegistryPackagesData = new BindingList<RegistryPackage>();

            //InitSampleRegistryPackagesData(5);
            InitVsSCommonPackagesData();
            InitRegistryPackagesData();

            RegistryPackagesData = new BindingList<RegistryPackage>(
                RegistryPackagesDataAll.Where(o => o.VisualStudioVersion.Equals(VisualStudioVersion)).ToList());
        }

        private void InitVsSCommonPackagesData()
        {
            // get registry data
            var sKeys = RegistryHelper .GetVisualStudioExtensionPath();
            VisualStudioVersions = RegistryHelper.VisualStudioVersions;
            
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
            string preferredVS = ConfigHelper.GetSetting("preferredVS");
      
            if (!string.IsNullOrEmpty(preferredVS))
            {
                string preferredVSa = VisualStudioVersions.FirstOrDefault(o => o.StartsWith(preferredVS));
                if (!string.IsNullOrEmpty(preferredVSa)) VisualStudioVersion = preferredVSa;
            }
            else
            VisualStudioVersion = VisualStudioVersions.FirstOrDefault();
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
            string preferredVS = ConfigHelper.GetSetting("preferredVS");

            if (!string.IsNullOrEmpty(preferredVS))
            {
                string preferredVSa = VisualStudioVersions.FirstOrDefault(o => o.StartsWith(preferredVS));
                if (!string.IsNullOrEmpty(preferredVSa)) VisualStudioVersion = preferredVSa;
            }
            else
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


        //   change  List<RegistryPackage> to BindingList<RegistryPackage> to notify list change
        public BindingList<RegistryPackage> RegistryPackagesData
        {
            get { return _registryPackagesData; }
            set
            {
                if (_registryPackagesData == null)
                    _registryPackagesData = value;
                else
                {
                    _registryPackagesData.Clear();
                    foreach (var v in value)
                        _registryPackagesData.Add(v);
                }
                OnPropertyChanged("RegistryPackagesData");
            }
        }

        public BindingList<ManifestPackage> ManifestPackagesData
        {
            get { return _manifestPackagesData ; }
            set
            {
                if (_manifestPackagesData == null)
                    _manifestPackagesData = value;
                else
                {
                    _manifestPackagesData.Clear();
                    foreach (var v in value)
                        _manifestPackagesData.Add(v);
                }
                OnPropertyChanged("ManifestPackagesData");
            }
        }

        public List<RegistryPackage> RegistryPackagesDataAll { get; set; }
        public List<ManifestPackage> ManifestPackagesDataAll { get; set; }
        
        public string VisualStudioVersionLabel { get; set; }

        public string VisualStudioVersion
        {
            get { return _visualStudioVersion; }
            set
            {
                _visualStudioVersion = value;
                
                // refill datagrid
                RegistryPackagesData = new BindingList<RegistryPackage>(RegistryPackagesDataAll.Where(o => o.VisualStudioVersion.Equals(VisualStudioVersion)).ToList());

                ManifestPackagesData = new BindingList<RegistryPackage>(ManifestPackagesDataAll.Where(o => o.VisualStudioVersion.Equals(VisualStudioVersion)).ToList());
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