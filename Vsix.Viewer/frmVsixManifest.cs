using System;
using System.Windows.Forms;
using Vsix.Viewer.ViewModels;

namespace Vsix.Viewer
{
    public partial class frmVsixManifest : Form
    {
        private BindingSource Binding { get; set; } 
        public frmVsixManifest()
        {
            InitializeComponent();
        }

        private void frmVsixManifest_Load(object sender, EventArgs e)
        {
            Binding = new BindingSource {DataSource = typeof (ManifestModel)};
            //Binding.Add(new ManifestModel("Boeing 747", 800));
        }
    }
}
