using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Vsix.Common.Helpers
{
   public static class DebugHelper
   {
       private static string _nl = Environment.NewLine;

       /// <summary>
       /// pring in output/debug window all values from list
       /// </summary>
       /// <param name="ss"></param>
       public static void DebugConsole(this string[] ss)
       {
           if (ss == null)
           {
               Debug.WriteLine("=== array is null ===");
               return;
           }
           if (ss.Length==0 )
           {
               Debug.WriteLine("=== array is not null, but empty ===");
               return;
           }
           int k = 0;
           foreach (string s in ss) Debug.WriteLine("{0,3} {1}", k++, s);
       }

       /// <summary>
       /// pring in output/debug window all values from list
       /// </summary>
       /// <param name="ss"></param>
       public static void DebugConsole(this List<string> ss)
       {
           if (ss == null)
           {
               Debug.WriteLine("=== List is null ===");
               return;
           }
           if (ss.Count == 0)
           {
               Debug.WriteLine("=== List is not null, but empty ===");
               return;
           }
           int k = 0;
           foreach (string s in ss) Debug.WriteLine("{0,3} {1}", k++, s);
       }

       public static void DebugOpenText(this List<string> list, string attrStart = "", bool numbered = false)
       {
           OpenText(list, ".txt", attrStart, numbered);
       }

       public static void DebugOpenText(this List<string> list, string[] attrStarts = null, bool numbered = false)
       {
           string attrStart = string.Empty;
           if (attrStarts != null && attrStarts.Length >0)
               attrStart = attrStarts.Aggregate(attrStart, (current, s) => current + (s + ";"));
           OpenText(list, ".txt", attrStart, numbered);
       }
       
       public static void DebugOpenHtml(this List<string> list, string attrStart = "", bool numbered = false)
       {
           OpenText(list, ".html", attrStart, numbered);
       }

       public static void DebugOpenHtml(this List<string> list, string[] attrStarts = null, bool numbered = false)
       {
           string attrStart = string.Empty;
           if (attrStarts != null && attrStarts.Length > 0)
               attrStart = attrStarts.Aggregate(attrStart, (current, s) => current + (s + ";"));
           OpenText(list, ".html", attrStart, numbered);
       }

       public static void OpenTextAsFile(string s, string ext)
       {
           var tmp = Path.GetTempFileName();
           File.Delete(tmp);
           tmp += ext;
           File.WriteAllText(tmp, s);
           Process.Start(tmp);
       }

       public static string OpenText(List<string> list, string ext, string attrStart, bool numbered = false)
       {
           // list.Sort();
           var tmp = Path.GetTempFileName();
           File.Delete(tmp);
           tmp += ext;
           if (ext.StartsWith(".htm", StringComparison.OrdinalIgnoreCase))
           {
               if (numbered)
               {
                   for (int k = 0; k < list.Count; k++)
                   {
                       if (!string.IsNullOrEmpty(attrStart) && attrStart != ";" && attrStart.Trim().Length > 0)
                       {
                           var ss = attrStart.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                           //  if (list[k].Contains(attrStart))
                           if (ss.Any(list[k].Contains))
                               list[k] = "<li>" + list[k] + "</li>";
                       }
                       else
                           list[k] = "<li>" + list[k] + "</li>";
                   }
                   list.Insert(0, "<ol>");
                   list.Add("</ol>");
               }
               else
               {
                   list.Insert(0, "<pre>");
                   list.Add("</pre>");
               }
           }
           else if (ext.StartsWith(".txt", StringComparison.OrdinalIgnoreCase) && numbered)
           {
               int c = 1;
               for (int k = 0; k < list.Count; k++)
               {
                   if (!string.IsNullOrEmpty(attrStart) && attrStart != ";" && attrStart.Trim().Length > 0)
                   {
                       var ss = attrStart.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                       //  if (list[k].Contains(attrStart))
                       if (ss.Any(list[k].Contains))
                           list[k] = string.Format("{0,3}. ", c++) + list[k];
                       else
                           c = 1;
                   }
                   else
                       list[k] = string.Format("{0,3}. ", k) + list[k];
               }
           }

           File.WriteAllLines(tmp, list);
           Process.Start(tmp);
           return tmp;
       }

       public static string ToHtmlTable(this Dictionary<string, string> dictionary)
       {
           // return ToStrings().Aggregate(string.Empty, (current, c) => current + (c + Environment.NewLine));
           int k = 0;
           string s = "<table border='1' cellpadding='2' cellspacing='0'>";
           s += dictionary.Aggregate(string.Empty, (current, c) =>
               current + string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", k++, c.Key, c.Value) + _nl);
           s += "</table>";
           return s;
       }

    }
}
