using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.Concurrent;
//using System.Configuration;
//using System.Data;
using System.Linq;
using System.Windows.Threading;
using System.Diagnostics;

using Cryoview_Tools;
using LLE.Util;           // LogLevel
using Cryoview_Log; // Log files
using System.Threading;

namespace Cryoview_Main_View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// <remarks>
    /// Kick off the process.  
    /// </remarks>
    public partial class App : Application, IDisposable
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars
        LogLevel m_defaultLevel = LogLevel.Debug;
        CryoviewFileLog m_cryoviewFileLog = null;
        EventLog m_eventLog = null;
        MainWindow mainWin = null;
        string m_appName = "Cryoview2";
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        /// <summary>
        /// Starts the app.
        /// </summary>
        App()
        {
            if (!VerifyExistenceOfEventSource()) Process.GetCurrentProcess().Kill();
            if (!VerifyOnlyOneAppInstance()) Process.GetCurrentProcess().Kill();
        }
        /// <summary> 
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Info, " Disposing program  ");
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary> 
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        public void Dispose(bool disposing)
        {
            if (disposing)
            {   // managed resources here
                Log.Instance.RemoveLog(m_cryoviewFileLog);
            }
            // unmanaged resources here
            {
            }
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        private void Initialize(string[] args)
        {
            ConcurrentDictionary<string, string> dictCmdLineArgs = new ConcurrentDictionary<string, string>();
            bool bArgsVerfied = GetCommandLineArgs(args, dictCmdLineArgs);
            if (bArgsVerfied) { CryoviewTools.AddCmdLineArgsToAppProperties(dictCmdLineArgs); }
        }
        #endregion initialization

        #region windows events
        /// <summary>
        /// Instantiate the UI by calling ctor and .Show() directly.
        /// </summary>
        private void Cryoview_Startup(object sender, StartupEventArgs e)
        {
            Initialize(e.Args);

            bool bNetworkFound = CryoviewTools.OnNetwork();
            if (!bNetworkFound)
            {
                MessageBox.Show("NO NETWORK CONNECTION!!", m_appName, MessageBoxButton.OK);
                //Process.GetCurrentProcess().Kill();
            }

            string strLogDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";
            try
            {
                m_cryoviewFileLog = new CryoviewFileLog(strLogDirPath + m_appName + "_Log_Files" + @"\" + m_appName);
            }
            catch (Exception ex)
            {
                m_eventLog.WriteEntry(ex.ToString());
                MessageBox.Show("LOGGING FAILURE!", m_appName, MessageBoxButton.OK);
                Process.GetCurrentProcess().Kill();
            }
            Log.Instance.AddLog(m_cryoviewFileLog);
            if (Properties.Contains("log")) 
            { 
                string logLevel = Properties["log"].ToString(); 
                SetLogLevel(logLevel, ref m_defaultLevel);
            }
            Log.Instance.SetLogLevel(m_defaultLevel); // Log file will only get msgs at this level or higher.
            CryoviewTools.LogMessage(LogLevel.Debug0, " Log level set to " + m_defaultLevel.ToString());
            CryoviewTools.LogMessage(LogLevel.Info, " ... continuing startup procedure ");

            try
            {
                mainWin = new MainWindow(m_eventLog);
            }
            catch (Exception ex)
            {
                m_eventLog.WriteEntry(" Startup crash: " + ex.Message);
            }
            //Thread.Sleep(30000); //temorary sleep while other threads finish loading (to solve a bug), may remove later
            mainWin.Show();
        }

        /// <summary> 
        /// absolute last chance to do something in the program
        /// </summary>
        private void Cryoview_Exit(object sender, ExitEventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Info, " Ending program. ");
            Dispose();
        }
        /// <summary> 
        /// Raised when the app is made the foreground window.
        /// </summary>
        private void Cryoview_Activated(object sender, EventArgs e){}
        /// <summary> 
        /// Raised when the app loses the foreground focus.
        /// </summary>
        private void Cryoview_Deactivated(object sender, EventArgs e){}
        /// <summary> 
        /// Raised if windows is logging out.
        /// </summary>
        private void Cryoview_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            if (m_eventLog != null)
            {
                m_eventLog.WriteEntry(m_appName + " exiting.");
            }
        }
        /// <summary> 
        /// Something happened to lead to a program crash.
        /// </summary>
        void Cryoview_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) 
        {
            m_eventLog.WriteEntry("Cryoview2_DispatcherUnhandledException(): " + e.Exception.ToString());
            Application.Current.Shutdown();
        }


        #endregion windows events

        #region utility functions
        /// <summary>
        /// Store cmd line args for future use
        /// </summary>
        /// <param name="args">passed in by o/s</param>
        /// <param name="dict">holds any discovered cmd line args</param>
        private bool GetCommandLineArgs(string[] args, ConcurrentDictionary<string, string> dictArgs)
        {
            bool bRetVal = false;
            string key = "";
            string value = "";
            foreach (string s in args)
            {
                ParseCmdLineArg(s, ref key, ref value);   // check this cmd line param for its arg
                dictArgs.AddOrUpdate(key, value, (k, v) => v);
                bRetVal = true;
            }  
            return bRetVal;
        }

        /// <summary>
        /// Break the cmd line arg into its constituent parts, e.g. /dev:true
        /// </summary>
        /// <param name="arg">something from the cmd line</param>
        /// <param name="dictArgs">storage for specified cmd line arg and its value</param>
        private void ParseCmdLineArg(string arg, ref string key, ref string value)
        {
            int index = arg.IndexOf(':');
            if (index == -1)
            {
                key = arg.Substring(1, arg.Length - 1);
                value = "";
            }
            else
            {
                key = arg.Substring(1, index - 1);
                value = arg.Substring(index + 1);
            }
        }

        /// <summary>
        /// Turn the cmd line arg into an enumerated value
        /// </summary>
        /// <param name="s">cmd line arg for the desired log level</param>
        private void SetLogLevel(string s, ref LogLevel defaultLevel)
        {
            switch (s)
            {
                case "Emerg": defaultLevel = LogLevel.Emerg; break;
                case "Alert": defaultLevel = LogLevel.Alert; break;
                case "Crit": defaultLevel = LogLevel.Crit; break;
                case "Err": defaultLevel = LogLevel.Err; break;
                case "Warning": defaultLevel = LogLevel.Warning; break;
                case "Notice": defaultLevel = LogLevel.Notice; break;
                case "Info": defaultLevel = LogLevel.Info; break;
                case "Debug": defaultLevel = LogLevel.Debug; break;
                case "Debug0": defaultLevel = LogLevel.Debug0; break;
                case "Debug1": defaultLevel = LogLevel.Debug1; break;
                case "Debug2": defaultLevel = LogLevel.Debug2; break;
                case "Debug3": defaultLevel = LogLevel.Debug3; break;
                case "Debug4": defaultLevel = LogLevel.Debug4; break;
                case "Debug5": defaultLevel = LogLevel.Debug5; break;
                case "Debug6": defaultLevel = LogLevel.Debug6; break;
                case "Debug7": defaultLevel = LogLevel.Debug7; break;
                default:
                    MessageBox.Show("Unrecognized log level option: " + s, "Contact SDG!");
                    break;
            }   // switch (s)
        }

        /// <summary>
        /// The EventViewer event source must exist.
        /// </summary>
        /// <returns>true if event source exists else false</returns>
        /// <remarks>
        /// Without the event source, we don't have a place to put msgs about program crashes. Last resort for debugging.
        /// </remarks>
        private bool VerifyExistenceOfEventSource()
        {
            bool bRetVal = true;

            try
            {       // test for existence of event source in the event log. 
                if (!EventLog.SourceExists(m_appName))

                {
                    EventLog.CreateEventSource(m_appName, "Application"); // must exit to allow the source to be registered.
                    MessageBox.Show("Registered a new event source.", "Requires restart of the app.", MessageBoxButton.OK, MessageBoxImage.Stop);
                    bRetVal = false;
                }
                else
                {
                    m_eventLog = new EventLog();
                    m_eventLog.Source = m_appName;
                    m_eventLog.WriteEntry(m_appName + " starting.");
                }
            }
            catch (System.Security.SecurityException ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("Security Exception on Creating Event Source: App expects the Event Viewer to be pre-configured",
                    "Must run the app as administrator.", MessageBoxButton.OK, MessageBoxImage.Stop);
                bRetVal = false;
            }
            return bRetVal;
        }

        /// <summary>
        /// If the app has a running instance, don't let this one proceed.
        /// </summary>
        /// <returns>true if this is the only running instance</returns>
        private bool VerifyOnlyOneAppInstance()
        {
            // when getting the process names, the '.exe' extension is not included. This app will show as 'Cryoview_win'.
            // In debugging, it will show as 'Cryoview_win7.vshost'.
            bool bRetVal = true;
            Process[] processes = Process.GetProcessesByName(m_appName);
            if (processes.Count() > 1)
            {
                string msg1 = "An instance of " + m_appName + " is already running ...";
                string msg2 = "Forcing shutdown of this instance.";
                m_eventLog.WriteEntry(msg1 + msg2);
                MessageBox.Show(msg1, msg2);
                bRetVal = false;
            }
            return bRetVal;
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
