using System;
using System.IO;    // File, StreamWriter

using LLE.Util;     // ILog, Log, LogLevel

namespace Cryoview_Log
{

    /// <summary>
    /// Subclass of Util.Log
    /// Adds a date time stamp and the log level _name in front of the message.
    /// </summary>
    public class CryoviewFileLog :  ILog, IDisposable
    {

        #region backing vars

        DateTime m_openLogDate; // date the log file was opened, used to test if it's time to close current log and create new one.
        StreamWriter m_streamWriter; // init'd in this class. Log.Instance(...) dynamically casts back to this class.
        string m_logFilename;   // full name of the log file (basename + date + '.txt')
        string m_logDir;

        #endregion backing vars

        #region ctors, dtor, dispose

        /// <summary>
        /// Initializes the log file
        /// </summary>
        /// <param _name="logDir">Directory of the file.  This name will be appended with the current date.</param>
        public CryoviewFileLog(string logDir)
        {
            m_logDir = logDir;
            OpenLogFile();    // check for existing log and take appropriate action
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param name="disposing">if true, called in code. If false, gc.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {       // managed resources here
                if (m_streamWriter != null)
                    m_streamWriter.Close();
            }
            // unmanaged resources here
            {
            }
        }

        #endregion ctors, dtor, dispose

        #region hardware code

        /// <summary>
        /// Called by Cryoview_Log instance
        /// </summary>
        /// <param name="level">importance of log msg</param>
        /// <param name="msg">what to record in log file</param>
        /// <remarks>compiler resolution</remarks>
        public virtual void WriteLine(LogLevel level, string msg)
        {
            WasChangeOfDate();  // if date rolled over, create a new file
            Log.Instance.WriteLine(level, msg); // Dynamically calls public void WriteLine(object aObject)
        }

        /// <summary>
        /// Class Log.cs receives an intf which is downcast back to the instance that has something to write.
        /// The downcast calls this method. It is here that the logfile entry is written.
        /// </summary>
        /// <param name="aObject"> msg to write to stream</param>
        /// <remarks> If called by dynamic intf resolution, will include log level of msg.</remarks>
        /// <remarks> If called by compiler resolution, will NOT include log level of msg.</remarks>
        public void WriteLine(object aObject)
        {
            WasChangeOfDate();
            m_streamWriter.WriteLine(aObject);
        }

        /// <summary>
        /// Called by Cryoview_Log instance (compiler resolution)
        /// </summary>
        /// <param name="level">importance of log msg</param>
        /// <param name="msg">what to record in log file</param>
        /// <remarks>compiler resolution</remarks>
        public virtual void Write(LogLevel level, string msg)
        {
            WasChangeOfDate();  // if date rolled over, create a new file
            Log.Instance.Write(level, System.DateTime.Now.ToString("yyyy MM dd HH:mm:ss") + " --> " + msg);
        }

        /// <summary>
        /// Class Log.cs receives an intf which is downcast back to the instance that has something to write.
        /// The downcast calls this method. It is here that the logfile entry is written.
        /// </summary>
        /// <param name="aObject"> msg to write to stream</param>
        /// <remarks> If called by dynamic intf resolution, will include log level of msg.</remarks>
        /// <remarks> If called by compiler resolution, will NOT include log level of msg.</remarks>
        public void Write(object aObject)
        {
            m_streamWriter.Write(System.DateTime.Now.ToString("yyyy MM dd HH:mm:ss") + " --> " + aObject);
        }

        #endregion hardware code

        #region utility methods

        /// <summary>
        /// Checks for an existing logfile (based on date) and either creates a new one or appends to an existing log file.    
        /// </summary>
        /// <param name="filename">Basename of the log file</param>
        private void OpenLogFile()
        {
            m_openLogDate = DateTime.Now.Date;
            m_logFilename = m_logDir + "_"  + m_openLogDate.ToString("yyyyMMdd") + ".txt";
            if (File.Exists(m_logFilename))
                m_streamWriter = File.AppendText(m_logFilename);
            else
                m_streamWriter = File.CreateText(m_logFilename);
            m_streamWriter.AutoFlush = true;
        }

        /// <summary>
        /// At 12:00:00 am, the log file in use is closed and a new log file is opened.
        /// </summary>
        private void WasChangeOfDate()
        {
            if (m_openLogDate != DateTime.Now.Date)
            {
                m_streamWriter.Close();
                OpenLogFile();
            }
        }

        #endregion utility methods

    }   // public class CryoviewFileLog :  ILog, IDisposable

}   // namespace Cryoview_Log
