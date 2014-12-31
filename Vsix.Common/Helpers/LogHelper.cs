using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;

namespace Vsix.Common.Helpers
{
    public static class LogHelper
    {
        private static ILog _logger;

        public static ILog Logger
        {
            get {
               
                if (_logger == null)
                { 
                    log4net.Config.XmlConfigurator.Configure();
                    _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                }
                return _logger;
            }
        }

        #region extra log configuration
        // list of additional log appenders
        private const string AutoUpdateLog = "AutoUpdateLogFile";
        private static ILog _loggerAutoUpdate;
        public static ILog LoggerAutoUpdate
        {
            get
            {
                if (_loggerAutoUpdate == null)
                    SetAppenders();
                if (_loggerAutoUpdate == null || !_loggerAutoUpdate.Logger.Repository.Configured)
                {
                    _loggerAutoUpdate = _logger;
                    _logger.Error("logger is not configured - check configuration file - " + AutoUpdateLog + " entries");
                }
                return _loggerAutoUpdate;
            }
            set { _loggerAutoUpdate = value; }
        }
        private static void SetAppenders()
        {
            XmlConfigurator.Configure();

            // get named logger from config
            LoggerAutoUpdate = LogManager.GetLogger(AutoUpdateLog);
        }
        #endregion

        private static string _logFilePath;
        /// <summary> location of log file 
        /// similar to  %appdata%\vsixedit\Logs\vsixLog.txt
        /// </summary>
        public static string LogFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    //var fa = _logger.Logger.Repository.GetAppenders().OfType<RollingFileAppender>().FirstOrDefault();
                    //var fav = log4net.LogManager.GetRepository().GetAppenders()[0];
                    //var appe = (((Hierarchy) _logger.Logger.Repository).Root).Appenders;
                    //var appenders = ((Hierarchy) LogManager.GetRepository()).Root.Appenders;

                    var rootAppender = ((Hierarchy) LogManager.GetRepository())
                        .Root.Appenders.OfType<FileAppender>()
                        .FirstOrDefault();

                    _logFilePath = (rootAppender != null) ? rootAppender.File : string.Empty;
                }
                return _logFilePath;
            }
        }

        /// <summary> log4net.Info(message)  </summary>
        /// <param name="message"></param>
        public static void LogInfo(object message)
        {
            try
            {
                Logger.Info(message);
            }
            catch
            {
            }
        }

        /// <summary>
        /// log4net.InfoFormat(formatString, messages)
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="messages"></param>
        /// <param name="lg"></param>
        public static void LogInfo(string formatString, string[] messages)
        {
            try
            {
                Logger.InfoFormat(formatString, messages);
            }
            catch
            {
            }
        }

        /// <summary> log4net.Warn(message)  </summary>
        /// <param name="message"></param>
        public static void LogWarn(object message)
        {
            try
            {
                Logger.Warn(message);
            }
            catch
            {
            }
        }

        /// <summary>
        /// write into output - could be used in debug mode or with DebugView
        /// </summary>
        /// <param name="message"></param>
        public static void LogDiag(object message)
        {
            int frameDeep = 2;
            try
            {
                Debug.WriteLine(ClassMethod(frameDeep));
                Debug.WriteLine(message.ToString());
            }
            catch
            {
            }
        }

        /// <summary>
        /// write into output - could be used in debug mode or with DebugView, extension for nested methods
        /// </summary>
        /// <param name="frameDeep">if 2 - the same as LogDiag; for wrapper use 3</param>
        /// <param name="message"></param>
        public static void LogDiagFrame(int frameDeep, object message)
        {
            try
            {
                string cl = ClassMethod(frameDeep);
                Debug.WriteLine(cl);
                Debug.WriteLine(message.ToString());
            }
            catch
            {
            }
        }

        /// <summary>
        /// write into logfile, extension for nested methods
        /// </summary>
        /// <param name="frameDeep">if 2 - the same as LogDiag; for wrapper use 3</param>
        /// <param name="message"></param>
        public static void LogDebugFrame(int frameDeep, object message)
        {
            try
            {
                string cl = ClassMethod(frameDeep);
                Logger.Debug(cl);
                Logger.Debug(message);
            }
            catch
            {
            }
        }

        public static void DiagXmlNodeValues(string s)
        {
            try
            {
                s = s.Replace(">\r\n", ">").Trim();
                var rdr = XmlReader.Create(new System.IO.StringReader(s));
                int deep = 0;
                string sNd = string.Empty;
                while (rdr.Read())
                {
                    if (rdr.NodeType == XmlNodeType.Element)
                    {
                        sNd = "rdr.LocalName Element=" + rdr.LocalName;
                        deep = rdr.Depth;
                        if (deep > 0) sNd = "".PadLeft(deep, ' ');
                        LogDiag(sNd);
                    }
                    else if (rdr.NodeType == XmlNodeType.Attribute)
                    {
                        deep = rdr.Depth;
                        sNd = "rdr.LocalName Attribute=@" + rdr.LocalName;
                        if (deep > 0) sNd = "".PadLeft(deep, ' ');
                        LogDiag(sNd);
                    }
                    else if (rdr.NodeType == XmlNodeType.Text || rdr.NodeType == XmlNodeType.CDATA)
                    {
                        // if Text right after element, then it is its value
                        sNd += "rdr.Value Text/CDATA=" + rdr.Value;
                        if (sNd.Length > 200) sNd = sNd.Substring(0, 200) + "...";
                        if (deep > 0) sNd = "".PadLeft(deep, ' ');
                        LogDiag(sNd);
                    }
                }
            }
            catch (Exception ex)
            {
                LogDiag(ex);
            }
        }

        /// <summary> log4net.Debug(message)  </summary>
        /// <param name="message"></param>
        /// <param name="lg"></param>
        public static void LogDebug(object message)
        {
            try
            {
                Logger.Debug(message);
            }
            catch
            {
            }
        }

        /// <summary> log4net.Error(ex.Message, ex)  </summary>
        /// <param name="ex"></param>
        public static void LogError(Exception ex)
        {
            try
            {
                Logger.Error(ex.Message, ex);
            }
            catch
            {
            }
        }

        /// <summary> log4net.Error(message)  </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
            try
            {
                Logger.Error(message);
            }
            catch
            {
            }
        }

        /// <summary>
        /// log4net.Error(message,exception)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void LogError(object message, Exception exception)
        {
            try
            {
                Logger.Error(message, exception);
            }
            catch
            {
            }
        }

        private static string nl = System.Environment.NewLine;

        /// <summary>
        /// log method entering info under DEBUG category
        /// </summary>
        /// <param name="args"></param>
        public static void LogEnter(params object[] args)
        {
            LogEnterExit("<<< Entering ", true, args);
        }

        /// <summary>
        /// log method info under DEBUG category, log entry will be prefixed with "direction"
        /// </summary>
        /// <param name="direction">cuastom message</param>
        /// <param name="useParamNames"></param>
        /// <param name="args"></param>
        public static void LogEnterExit(string direction, bool useParamNames, params object[] args)
        {
            int frameDeep = 3;
            if (!Logger.IsDebugEnabled)
                return;
            try
            {
                string s = direction;
                int k = 0;

                // if useParamNames then ClassMethod(3, true) will have ClassMethod|param1name|param2name...
                if (!useParamNames)
                {
                    s += ClassMethod(frameDeep);
                    if (args != null)
                    {
                        foreach (var o in args)
                            s += nl + string.Format("\t {0}:{1}", k++, o ?? "NULL");
                    }
                }
                else
                {
                    string[] ss = ClassMethod(frameDeep, true).Split('|');
                    s += ss[0];
                    if (args != null && args.Length > 0)
                    {
                        foreach (var o in args)
                            s += nl + string.Format("\t {0}: {1}: {2}", k++, (k < ss.Length) ? ss[k] : "", o ?? "NULL");
                    }
                }
                //GetLogger(Logs.VIA).Debug(s);
                Logger.Debug(s);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// log method exit info under DEBUG category
        /// </summary>
        /// <param name="args"></param>
        public static void LogExit(params object[] args)
        {
            LogEnterExit(">>> Exiting  ", false, args);
        }

        /// <summary>
        /// log method exit info under DEBUG category, list Dictionary values
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        public static void LogExit<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (!Logger.IsDebugEnabled)
                return;

            var lst = new List<string>();
            if (dict != null && dict.Count > 0)
            {
                int k = 0;
                foreach (var kvp in dict)
                    lst.Add(string.Format("{0,-2}. {1}:: {2}", k++, kvp.Key, kvp.Value));
            }
            LogEnterExit(">>> Exiting  ", false, lst);
        }

        /// <summary>
        /// log method exit info under DEBUG category, list SortedList values
        /// </summary>
        /// <param name="dict"></param>
        public static void LogExit(IDictionary dict)
        {
            if (!Logger.IsDebugEnabled)
                return;

            var lst = new List<string>();
            if (dict != null && dict.Count > 0)
            {
                int k = 0;
                foreach (KeyValuePair<string, string> kvp in dict)
                    lst.Add(string.Format("{0,-2}. {1}:: {2}", k++, kvp.Key, kvp.Value));
            }
            LogEnterExit(">>> Exiting  ", false, lst);
        }

        /// <summary>
        /// log method exit info under DEBUG category
        /// </summary>
        /// <param name="dict"></param>
        public static void LogExit(Dictionary<string, string> dict)
        {
            if (!Logger.IsDebugEnabled)
                return;

            int k = 0;
            string args = dict.Aggregate(string.Empty,
                                         (current, kvp) => current + (nl + (k++) + ". " + kvp.Key + ":: " + kvp.Value));
            LogEnterExit(">>> Exiting  ", false, args);
        }

        /// <summary>
        /// log method exit info under DEBUG category
        /// </summary>
        /// <param name="lst"></param>
        public static void LogExit(List<string> lst)
        {
            if (!Logger.IsDebugEnabled)
                return;

            int k = 0;
            string args = lst.Aggregate(string.Empty, (current, kvp) => current + (nl + (k++) + ". " + kvp));
            LogEnterExit(">>> Exiting  ", false, args);
        }

        /// <summary>
        /// log method exit info under DEBUG category, will log xml for the node
        /// </summary>
        /// <param name="nd"></param>
        public static void LogExit(System.Xml.XmlNode nd)
        {
            if (!Logger.IsDebugEnabled)
                return;

            string args = (nd != null) ? IndentXml(nd) : "NULL";
            LogEnterExit(">>> Exiting  ", false, args);
        }

        /// <summary>
        /// log method exit info under DEBUG category, will log xml for the document
        /// </summary>
        /// <param name="doc"></param>
        public static void LogExit(System.Xml.XmlDocument doc)
        {
            if (!Logger.IsDebugEnabled)
                return;

            string args = (doc != null) ? IndentXml(doc) : "NULL";
            LogEnterExit(">>> Exiting  ", false, args);
        }

        #region event logging methods

        /// <summary>  event log with EventLogEntryType  </summary>
        /// <param name="sSource"></param>
        /// <param name="sEvent"></param>
        /// <param name="evtId"></param>
        /// <param name="evtType"></param>
        public static void LogEvent(string sSource, string sEvent, int evtId, EventLogEntryType evtType)
        {
            try
            {
                if (string.IsNullOrEmpty(sSource)) sSource = "...";
                string sLog = "Application";

                if (!EventLog.SourceExists(sSource)) EventLog.CreateEventSource(sSource, sLog);
                if (evtId > 0)
                    EventLog.WriteEntry(sSource, sEvent, evtType, evtId);
                else
                    EventLog.WriteEntry(sSource, sEvent, evtType);
            }
            catch (Exception ex) { Debug.WriteLine(ex);}
        }

        /// <summary> event log with EventLogEntryType.Information </summary>
        /// <param name="sSource"></param>
        /// <param name="sEvent"></param>
        /// <param name="evtId"></param>
        public static void LogEventI(string sSource, string sEvent, int evtId = 0)
        {
            LogEvent(sSource, sEvent, evtId, EventLogEntryType.Information);
        }

        /// <summary> event log with EventLogEntryType.Warning </summary>
        /// <param name="sSource"></param>
        /// <param name="sEvent"></param>
        /// <param name="evtId"></param>
        public static void LogEventW(string sSource, string sEvent, int evtId = 0)
        {
            LogEvent(sSource, sEvent, evtId, EventLogEntryType.Warning);
        }

        /// <summary> event log with EventLogEntryType.Error </summary>
        /// <param name="sSource"></param>
        /// <param name="sEvent"></param>
        /// <param name="evtId"></param>
        public static void LogEventE(string sSource, string sEvent, int evtId = 0)
        {
            LogEvent(sSource, sEvent, evtId, EventLogEntryType.Error);
        }

        #endregion

        public static string IndentXml(System.Xml.XmlDocument doc, string indentChars = "  ")
        {
            var sb = new StringBuilder();
            try
            {
                var settings = new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = indentChars,
                    NewLineChars = System.Environment.NewLine,
                    NewLineHandling = System.Xml.NewLineHandling.Replace
                };
                using (var writer = System.Xml.XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                    writer.Close();
                }
            }
            catch (Exception)
            {
                ;
            }
            return sb.ToString();
        }

        public static string IndentXml(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return IndentXml(doc);
            }
            catch (Exception)
            {
                ;
            }
            return xml;
        }

        public static string IndentXml(object o)
        {
            string s = string.Empty;
            try
            {
                var serializer = new System.Runtime.Serialization.DataContractSerializer(o.GetType());
                using (var sw = new System.IO.StringWriter())
                {
                    using (var writer = new XmlTextWriter(sw))
                    {
                        writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                        serializer.WriteObject(writer, o);
                        writer.Flush();
                        s = sw.ToString();
                    }
                }
            }
            catch (Exception)
            {
                ;
            }
            return s;
        }

        /// <summary>
        /// return namespace.class.method name  of the scope where called
        /// usage: 
        ///     1. directly in calling method : 0.ClassMethod(1)
        ///     2. create property:   private static string ClassMethod{get { return "".ClassMethod(); }}
        ///              and call within method this property: ClassMethod
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="frame"></param>
        /// <param name="useParamNames"></param>
        /// <returns>string with the full method path</returns>
        public static string ClassMethod(int frame = 2, bool useParamNames = false)
        {
            if (frame < 0) frame = 2;
            var method = (new StackTrace()).GetFrame(frame).GetMethod();

            string s = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);

            // get parameter names and types. you cannot get values here.
            if (useParamNames)
            {
                s = method.GetParameters().Aggregate(s, (current, pi) => current + string.Format("|{0}", pi.Name));
            }
            return s;
        }
    }

}
