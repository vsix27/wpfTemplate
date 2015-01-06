using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Utils.Zip;
using Vsix.Viewer.Infrastructure;
using Vsix.Viewer.Presenters;

namespace Vsix.Viewer.ViewModels
{
   
    public class FolderFile
    {
        //uriPre = "/Vsix.Viewer;component/Resources/FileTypes/";

        /// <summary> "pack://application:,,/Resources/FileTypes/"  </summary>
        public const string UriPre = "pack://application:,,/Resources/FileTypes/";

        public string Name
        {
            get { return ("" + FullPath).Split("\\/".ToCharArray()).Last(); }
        }

        public string FullPath { get; set; }
        public bool IsFile { get; set; }
        public string IconImage
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return UriPre + "error.png";
                if (!IsFile) return UriPre + "folder.ico";
                switch (Name.Split('.').Last().ToLower())
                {
                    case "doc":
                    case "docx":
                    case "rtf":
                        return UriPre + "doc.ico";
                    case "txt":
                        return UriPre + "text.jpg";
                    case "xml":
                        return UriPre + "xml.gif";
                    case "png":
                    case "tif":
                    case "tiff":
                    case "bmp":
                    case "ico":
                        return UriPre + "picture.ico";
                    case "exe":
                        return UriPre + "exe.png";
                    case "dll":
                        return UriPre + "dll.png";
                    case "zip":
                    case "7z":
                    case "arc":
                    case "tar":
                        return UriPre + "compressed.ico";
                    default:
                        return UriPre + "document.ico";
                }

            }
         }
        public List<FolderFile> Children { get; set; }
    }

    public class VsixDetailsModel : ObservableObject, IVsixDetailsModel
    {
        private VsixDetailsPresenter _presenter;
        private string _txtOutputText;
        //private List<FolderFile> _folderFiles;

        /// <summary> folder or archive path </summary>
        public string ContentPath { get; set; }
        internal ContentTypes ContentType { get; set; }

      
        public enum ContentTypes
        {
            None,
            Archive,
            Directory,
        }

        public VsixDetailsModel()
        {
            //LogHelper.LogEnter();
            _presenter = new VsixDetailsPresenter(this);
            InitResources();
            //LogHelper.LogExit();
        }

        public void InitResources()
        {
            ContentType = ContentTypes.None;

            if (string.IsNullOrEmpty(ContentPath))
                OutputText = "Empty ContentPath " + ContentPath;
            else if (Directory.Exists(ContentPath)){
                InitTreeFromFolder(ContentPath); 
                ContentType = ContentTypes.Directory;
            }
            else if (File.Exists(ContentPath))
            {
                InitTreeFromArchive(ContentPath);
                ContentType = ContentTypes.Archive;
            }
            else
                OutputText = "Content does not exist " + ContentPath;
        }

        public void InitTreeFromArchive(string archivepath)
        {
            if (!File.Exists(archivepath)) return;
            string s = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            s = Path.Combine(s, Path.GetFileName(archivepath));
            Directory.CreateDirectory(s);
            using (var decompress = ZipFile.Read(archivepath))
            {
                foreach (var e in decompress)
                {
                    e.Extract(s, true);
                }
                /* expected: files: 
                            [Content_Types].xml
                            extension.vsixmanifest
                            TemplateBuilder.dll
                            Output\ProjectTemplates\CSharp\Windows%20Desktop\WpfDemoWithMvvMp.project.zip
                     * */
            }
            InitTreeFromFolder(s);
        }

        public void InitTreeFromFolder(string folderPath)
        {
            TextFolderPath = folderPath.Split("\\/".ToCharArray()).Last();
            FolderFiles = MakeDirFiles(folderPath).Children;
        }

        private FolderFile MakeDirFiles(string folderPath)
        {
            var folder = new FolderFile {FullPath = folderPath, IsFile = false, Children = new List<FolderFile>(),};

            foreach (var d in Directory.GetDirectories(folderPath))
                folder.Children.Add(MakeDirFiles(d));

            foreach (var f in Directory.GetFiles(folderPath))
                folder.Children.Add(new FolderFile() {FullPath = f, IsFile = true,});
            return folder;
        }


        public string TextFolderPath{ get; set; }

        public List<FolderFile> FolderFiles { get; set; }
     
        public string OutputText
        {
            get { return _txtOutputText; }
            set { _txtOutputText = value; OnPropertyChanged("OutputText"); }
        }

    }
}
