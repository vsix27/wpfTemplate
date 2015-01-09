using System;
using Vsix.Common.Helpers;

namespace Vsix.Viewer.Helpers
{
    public class ManifestPackage : IManifestPackage
    {
        /*
        load extension.vsixmanifest
         * version 1
            <Vsix Version="1.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2010">
         * version 2
         * <PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011">
            <Metadata>

         * 
         */

        /// <summary>
        /// load from version 1 or 2, xml is different
        /// </summary>
        /// <param name="extensionMvsixmanifest"></param>
        public ManifestPackage(string extensionMvsixmanifest)
        {
            // var doc = XmlHelper.DocLoadNoNamespaces(extensionMvsixmanifest);
            var doc = XmlHelper.LoadWithoutNamespacesReg(extensionMvsixmanifest);
            if (doc.DocumentElement.Name.Equals("Vsix", StringComparison.OrdinalIgnoreCase))
            {
                // Version 1 - see example 
                //string z1 = ManifestHelper.SampleVersion1Xml("string guid", "string projectName", "string projectVersion", "string projectDescription", "string author");
                
                Author = XmlHelper.GetNodeValue(doc, "//Author");
                Description = XmlHelper.GetNodeValue(doc, "//Description");

                GettingStartedGuidePath = XmlHelper.GetNodeValue(doc, "//GettingStartedGuide");
                IconPath = XmlHelper.GetNodeValue(doc, "//Icon");
                LicensePath = XmlHelper.GetNodeValue(doc, "//License");
                ManifestVersion = XmlHelper.GetNodeValue(doc, "//ManifestVersion");
                MoreInfoUrl = XmlHelper.GetNodeValue(doc, "//MoreInfoUrl");
                PreviewImagePath = XmlHelper.GetNodeValue(doc, "//PreviewImage");
                ProductId = XmlHelper.GetNodeValue(doc, "//Identifier/@Id");
                ProductIdGuid = XmlHelper.GetNodeValue(doc, "//ProductIdGuid");
                ProductName = XmlHelper.GetNodeValue(doc, "//Name");
                //ProjectCsPath = XmlHelper.GetNodeValue(doc, "//ProjectCsPath");
                ReleaseNotesPath = XmlHelper.GetNodeValue(doc, "//ReleaseNotes");
                Tags = XmlHelper.GetNodeValue(doc, "//Tags");
                Version = XmlHelper.GetNodeValue(doc, "//Version");
            }
            else if (doc.DocumentElement.Name.Equals("PackageManifest", StringComparison.OrdinalIgnoreCase))
            {
                // Version 2 
                // string z2 = ManifestHelper.SampleVersion2Xml("string guid", "string projectName", "string projectVersion", "string author");
                Author = XmlHelper.GetNodeValue(doc, "//Author");
                Description = XmlHelper.GetNodeValue(doc, "//Description");

                GettingStartedGuidePath = XmlHelper.GetNodeValue(doc, "//GettingStartedGuide");
                IconPath = XmlHelper.GetNodeValue(doc, "//Icon");
                LicensePath = XmlHelper.GetNodeValue(doc, "//License");
                ManifestVersion = XmlHelper.GetNodeValue(doc, "//ManifestVersion");
                MoreInfoUrl = XmlHelper.GetNodeValue(doc, "//MoreInfo");
                PreviewImagePath = XmlHelper.GetNodeValue(doc, "//PreviewImage");
                ProductId = XmlHelper.GetNodeValue(doc, "//Identifier/@Id");
                ProductIdGuid = XmlHelper.GetNodeValue(doc, "//ProductIdGuid");
                ProductName = XmlHelper.GetNodeValue(doc, "//Name");
                //ProjectCsPath = XmlHelper.GetNodeValue(doc, "//ProjectCsPath");
                ReleaseNotesPath = XmlHelper.GetNodeValue(doc, "//ReleaseNotes");
                Tags = XmlHelper.GetNodeValue(doc, "//Tags");
                Version = XmlHelper.GetNodeValue(doc, "//Version");
            }

        }

        public ManifestPackage()
        {
        }

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

        public string MoreInfoUrl { get; set; }
    }
}
