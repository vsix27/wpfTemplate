using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer.Views
{
    /// <summary>
    /// Interaction logic for UcVsixDetails.xaml
    /// </summary>
    public partial class UcVsixDetails : UserControl
    {
        private readonly VsixDetailsModel _viewModel;

        public UcVsixDetails()
        {
            InitializeComponent();

            _viewModel = new VsixDetailsModel();
            this.DataContext = _viewModel;

            InitTreeViewSample(TvMain);
        }

            //uriPre = "/Vsix.Viewer;component/Resources/FileTypes/";
            const string UriPre = "pack://application:,,/Resources/FileTypes/";

        public void InitTreeViewSample(TreeView tv)
        {
            //var tv = new TreeView();
            //"/Vsix.Viewer;component/Resources/FileTypes/folder.ico"
            var treeItem = GetItem("top", "file types", UriPre + "folder.ico");

            treeItem.Items.Add(GetItem(null, "word", UriPre + "doc.ico"));
            treeItem.Items.Add(GetItem(null, "xxxxxxxxxxx", UriPre + "icon_xml.gif"));
            treeItem.Items.Add(GetItem(null, "image", UriPre + "picture.ico"));
            treeItem.Items.Add(GetItem(null, "text", UriPre + "text.jpg"));

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
        private TreeViewItem GetItem(string uid, string text, string imagePath, bool isExpanded = true)
        {

            var stack = new StackPanel {Orientation = Orientation.Horizontal};
            if (!string.IsNullOrEmpty(imagePath))
            {
                if (imagePath.Split("\\/".ToCharArray()).Length == 1) imagePath = UriPre + imagePath;
                stack.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri(imagePath)),
                    Width = 16,
                    Height = 16
                });
            }
            stack.Children.Add(new Label {Content = text}); // Label
            var tri = new TreeViewItem {Uid = uid, IsExpanded = isExpanded, Header = stack};
            tri.MouseDoubleClick += TriOnMouseDoubleClick;
            //delegate { _viewModel.OutputText = ((tri.Header as StackPanel).Children [1] as Label ).Content.ToString()   ; };
            return tri;
        }

        private bool isRootTree { get; set; }
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
    }
}
