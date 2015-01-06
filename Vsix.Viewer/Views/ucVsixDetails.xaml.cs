using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Views
{
    /// <summary>
    /// Interaction logic for UcVsixDetails.xaml
    /// </summary>
    public partial class UcVsixDetails : UserControl
    {
        private readonly VsixDetailsModel _viewModel;

        public string ContentPath { get; set; }
        public VsixDetailsModel.ContentTypes ContentType { get; set; }

        public UcVsixDetails()
        {
            InitializeComponent();

            _viewModel = new VsixDetailsModel {ContentPath = ContentPath, ContentType = ContentType};
            TreeStyle = TreeFilesStyle;
            this.DataContext = _viewModel;

            //InitTreeViewSample(TvMain);
        }


        private Style _treeStyle;

        public Style TreeFilesStyle
        {
            get
            {
                if (_treeStyle == null)
                {
                    var res1 = Resources["TreeExpanded"] ?? FindResource("TreeExpanded");
                    if (res1 != null) _treeStyle = res1 as Style;
                }
                return _treeStyle;
            }
        }

        public void InitTreeViewSample(TreeView tv)
        {
            //var tv = new TreeView();
            //"/Vsix.Viewer;component/Resources/FileTypes/folder.ico"
            var treeItem = GetItem("top", "file types", FolderFile.UriPre + "folder.ico");

            treeItem.Items.Add(GetItem(null, "word_file.doc", FolderFile.UriPre + "doc.ico"));
            treeItem.Items.Add(GetItem(null, "xml_file.xml", FolderFile.UriPre + "icon_xml.gif"));
            treeItem.Items.Add(GetItem(null, "image_file.gif", FolderFile.UriPre + "picture.ico"));
            treeItem.Items.Add(GetItem(null, "text_file.log", FolderFile.UriPre + "text.jpg"));

            tv.Items.Add(treeItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="text"></param>
        /// <param name="imagePath">"pack://application:,,/Resources/FileTypes/folder.ico" that file should be type of content in project</param>
        /// <param name="isExpanded"></param>
        /// <returns></returns>
        public TreeViewItem GetItem(string uid, string text, string imagePath, bool isExpanded = true)
        {

            var stack = new StackPanel {Orientation = Orientation.Horizontal};
            if (!string.IsNullOrEmpty(imagePath))
            {
                if (imagePath.Split("\\/".ToCharArray()).Length == 1) imagePath = FolderFile.UriPre + imagePath;
                stack.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri(imagePath)),
                    Width = 16,
                    Height = 16
                });
            }
            stack.Children.Add(new Label {Content = text}); // Label
            var tri = new TreeViewItem {Uid = uid, IsExpanded = isExpanded, Header = stack, Style = TreeStyle};


            tri.MouseDoubleClick += TriOnMouseDoubleClick;
            //delegate { _viewModel.OutputText = ((tri.Header as StackPanel).Children [1] as Label ).Content.ToString()   ; };
            return tri;
        }

        public Style TreeStyle { get; set; }


        private string TreePathClicked { get; set; }


        private void TriOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var tri = sender as TreeViewItem;
            if (tri == null) return;

            var h = tri.Header;
            var stack = h as StackPanel;
            if (stack == null || stack.Children == null) return;
            foreach (var c in stack.Children)
            {
                Label lbl = c as Label;
                if (lbl != null)
                {
                    if (string.IsNullOrEmpty(TreePathClicked)) TreePathClicked = lbl.Content.ToString();
                    else TreePathClicked = lbl.Content.ToString() + "\\" + TreePathClicked;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(tri.Uid) && tri.Uid.Equals("top"))
            {
                _viewModel.OutputText = TreePathClicked;
                TreePathClicked = null;
            }
        }

        private void FolderFileDoubleClick(object sender, EventArgs eventArgs)
        {
            var tri = sender as TreeViewItem;
            if (tri == null) return;

            var h = tri.Header;
            var stack = h as StackPanel;
            if (stack == null || stack.Children == null) return;
            foreach (var c in stack.Children)
            {
                Label lbl = c as Label;
                if (lbl != null)
                {
                    if (string.IsNullOrEmpty(TreePathClicked)) TreePathClicked = lbl.Content.ToString();
                    else TreePathClicked = lbl.Content.ToString() + "\\" + TreePathClicked;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(tri.Uid) && tri.Uid.Equals("top"))
            {
                _viewModel.OutputText = TreePathClicked;
                TreePathClicked = null;
            }
        }

        private void TvMain_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tr = ((TreeView) sender).SelectedItem;
            var cf = tr as FolderFile;
            if (cf == null || !cf.IsFile) return;
            string cfl = cf.Name.ToLower();
            if (cfl.EndsWith(".dll") || cfl.EndsWith(".exe") || cfl.EndsWith(".vsix"))
                return;
            if (File.Exists(cf.FullPath))
            {
                _viewModel.OutputText = File.ReadAllText(cf.FullPath);
            }
            // Debug.WriteLine(cf.Name);
        }
    }
}
