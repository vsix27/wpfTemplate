using System.Collections.Generic;
using System.ComponentModel;
using Vsix.Viewer.Helpers;

namespace Vsix.Viewer.ViewModels
{
    public interface IRegistryPackageModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        string VisualStudioVersion { get; set; }
        List<string> VisualStudioVersions { get; set; }
        List<RegistryPackage> RegistryPackagesData { get; set; }
    }
}
