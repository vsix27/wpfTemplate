using System;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace EncryptUtility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text;
            string pass = txtPassword.Text;
            string userEncr = CryptoHelper.EncryptStringAes(user);
            string passEncr = CryptoHelper.EncryptStringAes(pass);
            string xml = @"<add key='SPUser' value='{0}' />" + _nl + @"<add key='SPPass' value='{1}' />";
            txtOutput.Text = string.Format(xml, userEncr, passEncr);
        }

        private static string _nl = Environment.NewLine;

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = string.Empty;
            var xml = "<x>" + txtOutput.Text + "</x>";
            try
            {
                Func<XmlElement, string, string> attrVal = (el, attr) => el.SelectSingleNode(attr).InnerText;
                Func<XmlElement, string, string> attrValCr =
                    (el, attr) => CryptoHelper.DecryptStringAes(attrVal(el, attr));
                var doc = new XmlDocument();
                doc.LoadXml("<x>" + txtOutput.Text + "</x>");
                foreach (XmlElement el in doc.SelectNodes("//add"))
                    s += string.Format("<add key='{0}' value='{1}' />", attrVal(el, "@key"), attrValCr(el, "@value")) +
                         _nl;

                txtDecrypted.Text = s;
            }
            catch (Exception ex)
            {
                txtDecrypted.Text = ex.Message;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void clearOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtOutput.Text = string.Empty;
        }

        private void clearDecryptedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDecrypted.Text = string.Empty;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string s1 = ConfigurationSettings.AppSettings.Get("SharedString");
            //string s2 = ConfigurationSettings.AppSettings.Get("Salt");

            string info = "usage:" + _nl;
            var steps = new[]
            {
                "type UserName ,",
                "type Password,",
                "click menu 'Encrypt',",
                "click meny 'Copy Encrypted',",
                "paste into config file (overwrite existing keys)",
                "save config file",
                "-------------------------------------------",
                "to check encryption: ",
                "paste encrypted xml section into 'Encrypted config section'",
                "click menu Decrypt"
            };
            int k = 1;
            info = steps.Aggregate(info, (current, s) => current + (string.Format("   {0}. ", k++) + s + _nl));
            info += _nl;
            info += @".net library used System.dll, System.Core.dll located in" + _nl +
                    @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\" + _nl +
                    @"class used: System.Security.Cryptography.RijndaelManaged, " + _nl +
                    @"with methods Rfc2898DeriveBytes, CreateEncryptor" + _nl;
            txtDecrypted.Text = info;
        }
    }
}
