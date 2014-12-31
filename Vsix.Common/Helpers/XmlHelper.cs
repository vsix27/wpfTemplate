using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Vsix.Common.Helpers
{
    public class XmlHelper
    {
        /// <summary>
        /// load xml ignoring all namespaces via doc.Load(new XmlTextReader(filePath) { Namespaces = false });
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static XmlDocument DocLoadNoNamespaces(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var doc = new XmlDocument();
                    doc.Load(new XmlTextReader(filePath) {Namespaces = false});
                    return doc;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return null;
        }

        /// <summary>
        /// /// load xml document from file with namespace
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nmspValue">http://schemas.microsoft.com/developer/msbuild/2003</param>
        /// <param name="xPath">//RootNamespace  //x0:RootNamespace</param>
        /// <returns></returns>
        public static XmlNode GetNodeFromDocWithNamespace(string filePath, string nmspValue, string xPath)
        {
            return GetNodeFromDocWithNamespace(filePath, new[] {nmspValue}, xPath);
        }

        /// <summary>
        /// load xml document from file with namespaces
        /// </summary>
        /// <param name="filePath">xml file path</param>
        /// <param name="nmspValues"> list of namespaces similar to http://schemas.microsoft.com/developer/msbuild/2003 </param>
        /// <param name="xPath">//RootNamespace  //x0:RootNamespace //x1:RootNamespace</param>
        /// <returns></returns>
        public static XmlNode GetNodeFromDocWithNamespace(string filePath, string[] nmspValues, string xPath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(filePath);
                    var ns = new XmlNamespaceManager(doc.NameTable);
                    // add expected namespaces:
                    int k = 0;
                    foreach (var nmspValue in nmspValues)
                        ns.AddNamespace("x" + (k++), nmspValue);

                    var nd = doc.SelectSingleNode(xPath, ns);
                    return nd;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return null;
        }

        /// <summary>
        /// load xml ignoring all namespaces via Regex.Replace
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static XmlDocument LoadWithoutNamespacesReg(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var doc = new XmlDocument();

                    string xml = System.IO.File.ReadAllText(filePath);
                    // 2. remove any XML Namespace (xmlns) declaration before loading the XML
                    string filter = @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""";
                    string xmlNoNamspace = Regex.Replace(xml, filter, "");
                    doc.LoadXml(xmlNoNamspace);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return null;
        }

    }
}
