using System;
using System.Windows;       // Application; reqs ref to PresentationFramework
using System.Data;  // Sql CommandBehavior, e.g. close conn when finished
using System.Data.Odbc; // establish conn to db
using System.Collections.Generic;   //  key/value pair - SortedList
using System.Collections.Concurrent;   // threadsafe Dictionary<T, T>; threadsafe access to collection
using System.Text.RegularExpressions;   // Regex
using System.Threading.Tasks; // Task processing
using System.Runtime.CompilerServices;  

using Cryoview_Tools;
using LLE.Util; // Logging

namespace DB_Model
{
    public class DBModel : IDisposable
    {
        #region delegates, events

        public delegate void DelExceptionRaised(string msg);
        public event DelExceptionRaised ExceptionRaised;

        #endregion delegates, events

        #region backing vars

        // used in async ops to prevent race condx of multiple senders reporting new info
        private Object m_lockObj = new Object();
        private String m_dsn = "";    // odbc conn to db

        #endregion backing vars

        #region ctors, dtors, dispose

        public DBModel()
        {
            ConcurrentDictionary<string, string> properties = new ConcurrentDictionary<string, string>();
            properties = CryoviewTools.GetAppProperties();
            if (properties.ContainsKey("dev")) { m_dsn = "DSN=tfabt"; }
            else { m_dsn = "DSN=tfab"; }
            CryoviewTools.LogMessage(LogLevel.Debug7, "DBModel created with dsn = " + m_dsn);

        }  

        /// <summary>
        /// clean up in a .net way
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Disposing DBModel");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// clean up in a .net way
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)  // managed resources here
            {
            }
            { } // unmanaged resources here
        }

        #endregion ctors, dtors, dispose

        #region hardware code
        /// <summary>
        /// Retrieve settings from the database for a particular operation.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="operation"></param>
        /// <param name="configParams"></param>
        public bool RetrieveSettings(string location, string operation, ConcurrentDictionary<string, string> configParams)
        {
            bool bRetVal = false;
            string statement = "";
            OdbcDataReader reader = null;
            using (OdbcConnection conn = new OdbcConnection(m_dsn))
            {
                try
                {
                    conn.Open();
                    statement = "Select cfc_setting_name, cfc_setting_value from OPS$DBRED.cryoview_filltube_config where cfc_location = '" + location +
                        "' and cfc_hardware = '" + operation + "'";
                    OdbcCommand cmd = new OdbcCommand(statement, conn);
                    CryoviewTools.LogMessage(LogLevel.Debug, statement);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string key = reader["CFC_SETTING_NAME"].ToString();
                        string value = reader["CFC_SETTING_VALUE"].ToString();
                        configParams.TryAdd(key, value);  // if same key encountered, multiple times, then value is concatenated.

                        CryoviewTools.LogMessage(LogLevel.Debug5, "Data pulled from database for " + location + operation + ": setting name is " + key + ", value is " + value);
                    }
                    reader.Close();
                    bRetVal = true;
                    conn.Close();
                }   
                catch (Exception ex)
                {
                    HandleExceptions(ex);
                }
            }  
            return bRetVal;
        }

        /// <summary>
        /// Creating a new hdf file so need the next available file Id with which to name the file
        /// </summary>
        /// <param name="fileId"></param>
        public bool RetrieveNextFileId(ref string fileId)
        {
            bool bRetVal = true;
            using (OdbcConnection conn = new OdbcConnection(m_dsn))
            {
                try // if there was a db problem, it should have happened on startup so minimal checks here.
                {
                    conn.Open();

                    // this sequence retrieves the next available file id. The sequence does the increment; we don't need to.    
                    // s_cryo_view-file_id is the lle generated sequence; dual is an oracle provided place (it's not a table).
                    // Currently no other program does this, so targetfab will not have a missing file number, but it could happen.
                    string statement = "Select OPS$DBRED.CRYO_FILLTUBE_FILE_ID.nextval from dual";
                    //string statement = "Select s_cryo_view_file_id.nextval from dual";
                    CryoviewTools.LogMessage(LogLevel.Debug, statement);
                    OdbcCommand cmd = new OdbcCommand(statement, conn);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        fileId = reader[0].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    HandleExceptions(ex);
                }
            } 
            CryoviewTools.LogMessage(LogLevel.Info, " Retrieved file id is " + fileId);
            return bRetVal;
        }
        #endregion hardware code

        #region utility methods

        /// <summary>
        /// Aspect-oriented programming
        /// Break down program logic into distinct parts, aka concerns, one of which is handling exceptions.
        /// </summary>
        /// <param name="ex">the unusual event</param>
        private void HandleExceptions(Exception ex,
                        [CallerFilePath] string sourceFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            string trace = sourceFile.Substring(sourceFile.LastIndexOf('\\') + 1) + "(" + lineNumber + "): " + memberName + " --> ";
            int stopAt = 0;
            int len = ex.ToString().Length;
            if (len < 1000) stopAt = len - 1; else stopAt = 999;
            if (ex.GetType() == typeof(OdbcException))
            {

                if (ex.ToString().Contains("ORA-00942"))
                {       //  this is a catastrophic error, e.g. table or view name is either missing or invalid, or sql stmnt is wrong
                    CryoviewTools.LogMessage(LogLevel.Err, " table/column name or sql stmnt problem --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" table/column name or sql stmnt problem --> " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
                }
                else if (ex.ToString().Contains("ORA-00904"))
                {       //  this is a catastrophic error, e.g. column name is either missing or invalid, or sql stmnt is wrong
                    CryoviewTools.LogMessage(LogLevel.Err, " column name or sql stmnt problem --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" column name or sql stmnt problem --> " + ex.ToString());
                }
                else if (ex.ToString().Contains("ORA-12154"))
                {       //  this is a catastrophic error, e.g. loss of db conn or wrongly configured odbc dsn
                    CryoviewTools.LogMessage(LogLevel.Err, " tns service issue --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" tns service issue --> " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
                }
                else if (ex.ToString().Contains("ORA-12545"))
                {
                    CryoviewTools.LogMessage(LogLevel.Err, " missing network conn? --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" missing network conn? --> " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
                }
                else if (ex.ToString().Contains("ORA-00936"))
                {
                    CryoviewTools.LogMessage(LogLevel.Err, " failed insert  --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" failed insert  --> " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
                }
                else if (ex.ToString().Contains("architecture mismatch"))
                {
                    CryoviewTools.LogMessage(LogLevel.Err, " failed db call --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" failed db call --> " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
                }
                else if (ex.ToString().Contains("IM002"))
                {
                    CryoviewTools.LogMessage(LogLevel.Err, " DSN problem --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" missing DSN --> " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
                }
                else
                {
                    CryoviewTools.LogMessage(LogLevel.Err, " odbc exception --> " + " ( " + m_dsn + " ) " + ex.ToString());
                    OnExceptionRaised(" odbc exception --> " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
                }
            }
            else if (ex.GetType() == typeof(InvalidCastException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(" invalid cast: " + trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
            }
            else if (ex.GetType() == typeof(IndexOutOfRangeException))
            {   // thrown if reader["..."] attempts to get a value not retrieved by the sql statement. Could be catastrophic.
                // Require that code or db be fixed before continuing.
                // catching something like reader["xxx"] or reader[27] does not exist. In a data-correct table, this should not happen. 
                CryoviewTools.LogMessage(LogLevel.Err, " ( " + m_dsn + " ) " + " --> " + ex.ToString());
                OnExceptionRaised(" index out of range: " + trace + " ( " + m_dsn + " ) " + ex.ToString());
            }
            else
            {
                CryoviewTools.LogMessage(LogLevel.Err, " ( " + m_dsn + " ) " + ex.ToString());
                OnExceptionRaised(trace + " ( " + m_dsn + " ) " + ex.ToString().Substring(0, stopAt));
            }
        }

        /// <summary>
        /// Let subscribers know that something completely unexpected happened.
        /// Wraps the test for any subscribers to the event before raising it.
        /// </summary>
        /// <param name="msg"></param>
        private void OnExceptionRaised(string msg)
        {
            if (ExceptionRaised != null)
            {
                ExceptionRaised(msg);
            }
        }   

        #endregion utility methods
    }
}
