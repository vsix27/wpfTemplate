using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vsix.Common.Helpers
{
    public class ExplorerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="winTitle">"Select Vsix package "</param>
        /// <param name="filter">@"Vsix packages (.vsix)|*.vsix"</param>
        /// <returns></returns>
        public static string SelectFileInExplorer(string winTitle, string filter)
        {
            return SelectFileInExplorer(winTitle, new List<string>() { filter });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="winTitle">"Select Vsix package "</param>
        /// <param name="filters">"Vsix packages (.vsix)|*.vsix|All Files (*.*)|*.*
        /// filter += ";" + filters[k]; // creates single entry for all extensions</param>
        /// <returns></returns>
        public static string SelectFileInExplorer(string winTitle, IList<string> filters)
        {
            string s = string.Empty;
            string filter = filters[0];

            if (filters.Count > 1)
                for (int k = 1; k < filters.Count; k++)
                    filter += "|" + filters[k]; // creates list of extension

            var dialog = new OpenFileDialog
            {
                Filter = filter,
                Multiselect = false,
                FilterIndex = 1,
                Title = winTitle,
                RestoreDirectory = true,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                s = dialog.FileName;

            return s;
        }

    }
}
