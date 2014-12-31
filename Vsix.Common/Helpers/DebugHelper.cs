using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Vsix.Common.Helpers
{
   public static class DebugHelper
    {
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

       public static void OpenText(List<string> list, string ext, string attrStart, bool numbered = false)
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
       }
    }
}
