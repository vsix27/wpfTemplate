namespace Vsix.Viewer.Helpers
{
    public interface IManifestPackage
    {
        string ProductName { get; set; }
        string Author { get; set; }
        string ProductId { get; set; }
        string ProductIdGuid { get; set; }

        /// <summary> version of the vsix developer package </summary>
        string Version { get; set; }
        string ProjectCsPath { get; set; }
        string Description { get; set; }
        string LicensePath { get; set; }
        string GettingStartedGuidePath { get; set; }
        string ReleaseNotesPath { get; set; }

        string IconPath { get; set; }
        string PreviewImagePath { get; set; }
        string Tags { get; set; }

        /// <summary> version of the xml schema for manifest </summary>
        string ManifestVersion { get; set; }
 
    }
}