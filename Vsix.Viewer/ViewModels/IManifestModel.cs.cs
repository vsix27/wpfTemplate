using System.ComponentModel;

namespace Vsix.Viewer.ViewModels
{
    /// <summary> properties used in presenter </summary>
    public interface IManifestModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        string ProductName { get; set; }
        string Author { get; set; }
        string ProductId { get; set; }
        string ProductIdGuid { get; set; }
        string Version { get; set; }
        string ProjectCsPath { get; set; }
        string Description { get; set; }
        string LicensePath { get; set; }
        string GettingStartedGuidePath { get; set; }
        string ReleaseNotesPath { get; set; }
        
        string IconPath { get; set; }
        string PreviewImagePath { get; set; }
        string Tags { get; set; }
    }
}
