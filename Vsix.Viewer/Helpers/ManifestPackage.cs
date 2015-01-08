namespace Vsix.Viewer.Helpers
{
    public class ManifestPackage : IManifestPackage
    {

        public string VisualStudioVersion { get; set; }
        public string ProductName { get; set; }

        public string Author { get; set; }

        public string ProductId { get; set; }

        public string ProductIdGuid { get; set; }

        public string Version { get; set; }

        public string ProjectCsPath { get; set; }

        public string Description { get; set; }

        public string LicensePath { get; set; }

        public string GettingStartedGuidePath { get; set; }

        public string ReleaseNotesPath { get; set; }

        public string IconPath { get; set; }

        public string PreviewImagePath { get; set; }

        public string Tags { get; set; }

        public string ManifestVersion { get; set; }
    }
}
