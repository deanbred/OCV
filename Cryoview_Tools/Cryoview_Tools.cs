using System;
using System.Windows;   // Application
using System.Collections; // DictionaryEntry
using System.Collections.Generic;   // KeyValuePair
using System.Collections.Concurrent; // ConcurrentDictionary<T, T>
using System.Threading;
using System.Configuration;
using System.Net.NetworkInformation;    // use ping to test on network
using System.Runtime.CompilerServices;

using LLE.Util;

namespace Cryoview_Tools
{
    public static class CryoviewTools
    {
        #region backing vars

        private static object objLogLock = new object();

        #endregion backing vars

        #region utility functions

        /// <summary>
        /// get anything in the app resource file
        /// </summary>
        public static void AddAppResourcesToAppProperties(ConcurrentDictionary<string, string> resources)
        {
            foreach (var settings in resources)
            {
                Application.Current.Properties.Add(settings.Key, settings.Value);
            }
        }

        /// <summary>
        /// Make the cmd line args available thruout the app.
        /// </summary>
        /// <param name="dictArgs"></param>
        public static void AddCmdLineArgsToAppProperties(ConcurrentDictionary<string, string> dictCmdLineArgs)
        {
            foreach (KeyValuePair<string, string> kvp in dictCmdLineArgs)
            {
                Application.Current.Properties.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>anything that was added to the app properties</returns>
        public static ConcurrentDictionary<string, string> GetAppProperties()
        {
            ConcurrentDictionary<string, string> properties = new ConcurrentDictionary<string, string>();
            foreach (DictionaryEntry de in Application.Current.Properties)
            {       // cmd line params and values read in from app config file
                properties.AddOrUpdate(de.Key.ToString(), de.Value.ToString(), (k, v) => de.Value.ToString());
            }
            return properties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool OnNetwork()
        {
            bool bNetworkFound = false;
            Ping ping = new Ping();
            PingOptions pingOptions = new PingOptions();
            byte[] buffer = new byte[32];
            PingReply pingReply = null;
            //try { pingReply = ping.Send("redwood", 1000, buffer, pingOptions); }
            //catch (PingException) { }
            //if (pingReply.Status == IPStatus.Success) { bNetworkFound = true; }
            //else { }
            return bNetworkFound;
        }

        /// <summary>
        /// Sends the log level and the msg to the log file.
        /// </summary>
        /// <param name="level">importance of the msg</param>
        /// <param name="msg">what is being reported</param>
        /// <param name="sourceFile">source code file</param>
        /// <param name="lineNumber">source code line</param>
        /// <param name="memberName">method</param>
        public static void LogMessage(LogLevel level, string msg,
            [CallerFilePath] string sourceFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            lock (objLogLock)
            {
                string trace = sourceFile.Substring(sourceFile.LastIndexOf('\\') + 1) + "(" + lineNumber + "): " + memberName + " --> ";
                Log.Instance.WriteLine(level, trace + msg);
            }
        }
        #endregion utility functions
    }
}
