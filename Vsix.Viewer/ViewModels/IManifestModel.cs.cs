using System.ComponentModel;
using Vsix.Viewer.Helpers;

namespace Vsix.Viewer.ViewModels
{
    /// <summary> properties used in presenter </summary>
    public interface IManifestModel : IManifestPackage
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}
