using System;
using System.Collections.Generic;       // KeyValuePair
using System.Collections.Concurrent;    // ConcurrentDictionary<T,T>; threadsafe access to collection
using System.Collections.ObjectModel;   // ObservableCollection<T>
using System.Windows.Input; // for ICommand; requires ref to PresentationCore
using System.Windows;
using System.IO;                // Hdf file existence, dir creation
using System.Threading;
using Prism.Commands;
using Prism.Mvvm;
using System.Runtime.CompilerServices;  // [CallerFileName], etc

using Cryoview_Tools;   // logging
using LLE.Util;         //  logging
using Cryoview_ModuleMessages;
using DB_Model;
using MCM_Interface;
using MCM_Physical_Model;
using MCM_Virtual_Model;

namespace Cryoview_Main_ViewModel
{
    public class CryoviewMainViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        // Use this for catastrophic info to user. 
        // Use dependency property MsgFmViewModel for status and warnings.
        public delegate void DelExceptionRaised(string msg);
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private DBModel m_db = null;
        private string m_targetID = "";
        private string m_statusMsg = "";
        private string m_taskMsg = "";
        private Single m_layeringSphereTemp = 0.0f;
        private Single m_layeringSphereSetPoint = 0.0f;
        private Single m_coldFingerExtTemp = 0.0f;
        private Single m_coldFingerExtSetPoint = 0.0f;
        private bool m_NetworkConnected = false;
        private bool isDisposing = false;
        private bool m_MCMTryReconnect = true;
        private Thread NetworkStatus = null;
        private Thread GetMCMTemps = null;
        private MCMInterface _MCMModel = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        /// <summary>
        /// </summary>
        public CryoviewMainViewModel()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");           
            m_db = new DBModel();

            TargetID = "---------";
            StatusMessage = "---------";
            TaskMessage = "---------";
            LayeringSphereTemp = -1.0f;
            ColdFingerExtTemp = -1.0f;

            ErrorOrStatusMessageEvent.Instance.Subscribe(DisplayErrorOrStatus);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        /// <summary>
        /// Got here by user code.
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");            
            Dispose(true);
            GC.SuppressFinalize(this);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        } 

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (disposing)
            {       // managed resources here
                isDisposing = true;
                if(NetworkStatus != null) { NetworkStatus.Join(); }
                if(GetMCMTemps != null) { GetMCMTemps.Join(); }
                _MCMModel.ExceptionRaised -= MCM_Exception_Raised;
                _MCMModel.ConnectionLost -= MCM_Connection_Lost;
                ErrorOrStatusMessageEvent.Instance.Unsubscribe(DisplayErrorOrStatus);
                MCMTemperatureMessageEvent.Instance.Unsubscribe(SetMCMTemp);
                _MCMModel.Dispose();
            }
            // unmanaged resources here
            {
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        /// <summary>
        /// Handle all network and hardware communication required for initialization
        /// </summary>
        /// <param name="properties">application properties. we are going to add values from db configuration.</param>
        public bool Initialize()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            bool bRetVal = true;
            TaskMessage = "Starting Up...";
            new Thread(
                    delegate ()
                    {
                        ProgramStarted();
                    }
                ).Start();
            //NetworkStatus = new Thread(new ThreadStart(NewtorkStatusUpdate));
            //NetworkStatus.Start();
            //SetUpMCM();
            ////GetMCMTemps = new Thread(new ThreadStart(GetTempsFromMCM));
            ////GetMCMTemps.Start();
            //MCMTemperatureMessageEvent.Instance.Subscribe(SetMCMTemp);
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return bRetVal;
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        #endregion properties

        #region bindable properties
        public bool NetworkConnected
        {
            get { return m_NetworkConnected; }
            set { SetProperty<bool>(ref m_NetworkConnected, value); RaisePropertyChanged(nameof(NetworkConnected)); }
        }
        public string TargetID 
        {
            get { return m_targetID; }
            set { SetProperty<string>(ref m_targetID, value); RaisePropertyChanged(nameof(TargetID)); }
        }

        public string StatusMessage
        {
            get { return m_statusMsg; }
            set { SetProperty<string>(ref m_statusMsg, value); RaisePropertyChanged(nameof(StatusMessage)); }
        }

        public string TaskMessage
        {
            get { return m_taskMsg; }
            set { SetProperty<string>(ref m_taskMsg, value); RaisePropertyChanged(nameof(TaskMessage)); }
        }

        public Single LayeringSphereTemp
        {
            get { return m_layeringSphereTemp; }
            set { SetProperty<Single>(ref m_layeringSphereTemp, value); RaisePropertyChanged(nameof(LayeringSphereTemp)); }
        }

        public Single ColdFingerExtTemp
        {
            get { return m_coldFingerExtTemp; }
            set { SetProperty<Single>(ref m_coldFingerExtTemp, value); RaisePropertyChanged(nameof(ColdFingerExtTemp)); }
        }
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        #endregion algorithm code

        #region hardware code
        #endregion hardware code

        #region utility functions
        
        private void ProgramStarted()
        {
            Thread.Sleep(60000);
            TaskMessage = "Started";
        }
        private void NewtorkStatusUpdate()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");      
            while (!isDisposing)
            {
                NetworkConnected = CryoviewTools.OnNetwork();
                Thread.Sleep(20000); //check network connection once every 20 seconds
            }
            Thread.Sleep(1000);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void GetTempsFromMCM()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");            
            while (!isDisposing)
            {
                LayeringSphereTemp = _MCMModel.GetLSTemp();
                ColdFingerExtTemp = _MCMModel.GetCFETemp();
                Thread.Sleep(2000); 
            }
            Thread.Sleep(1000);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void SetUpMCM()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");            
            
            ConcurrentDictionary<string, string> properties = CryoviewTools.GetAppProperties();
            string Location = "";
            if (properties.ContainsKey("Location")) { Location = properties["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.

            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();
            m_db.RetrieveSettings(Location, "MCM", configParams);

            string model;
            configParams.TryGetValue("Model", out model);
            string IPAddr;
            configParams.TryGetValue("IPAddress", out IPAddr);
            string Port;
            configParams.TryGetValue("Port", out Port);

            if (model == "Virtual")
            {
                _MCMModel = new MCMVirtualModel();
            }
            else
            {
                ushort port = Convert.ToUInt16(Port);
                _MCMModel = new MCMPhysicalModel(IPAddr, port);
            }
            _MCMModel.ExceptionRaised += MCM_Exception_Raised;
            _MCMModel.ConnectionLost += MCM_Connection_Lost;
            _MCMModel.Initialize();
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void DisplayErrorOrStatus(object state)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");          
            ErrorOrStatusMessage msg = (ErrorOrStatusMessage)state;
            if(msg.messageType == MessageType.Error)
            {
                CryoviewTools.LogMessage(LogLevel.Alert, msg.Message);
                StatusMessage = msg.Message;
            }
            else
            {
                TaskMessage = msg.Message;
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void SetMCMTemp(object state)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            MCMTemperatureMessage msg = (MCMTemperatureMessage)state;
            if(msg.Target == MCMTarget.LS)
            {
                Single temp = _MCMModel.LSSetPoint;
                switch (msg.Direction)
                {
                    case MCMTempDirection.SET:
                        _MCMModel.SetLSTemp(msg.Temperature);
                        break;
                    case MCMTempDirection.PLUS:
                        temp += msg.Temperature;
                        _MCMModel.SetLSTemp(temp);
                        break;
                    case MCMTempDirection.MINUS:
                        temp -= msg.Temperature;
                        _MCMModel.SetLSTemp(temp);
                        break;
                }
                //Temporary code for testing only
                string msgbx = _MCMModel.LSSetPoint.ToString("0.000");
                MessageBox.Show(msgbx,"LS Setpoint", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Single temp = _MCMModel.CFESetPoint;
                switch (msg.Direction)
                {
                    case MCMTempDirection.SET:
                        _MCMModel.SetCFETemp(msg.Temperature);
                        break;
                    case MCMTempDirection.PLUS:
                        temp += msg.Temperature;
                        _MCMModel.SetCFETemp(temp);
                        break;
                    case MCMTempDirection.MINUS:
                        temp -= msg.Temperature;
                        _MCMModel.SetCFETemp(temp);
                        break;
                }
                //Temporary code for testing only
                string msgbx = _MCMModel.CFESetPoint.ToString("0.000");
                MessageBox.Show(msgbx, "CFE Setpoint", MessageBoxButton.OK, MessageBoxImage.Information);
                
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            }
        }

        public void SendMCMObject()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");           
            MCMObjectMessageEvent.Instance.Publish(new MCMObjectMessage
            {
                MCM = _MCMModel
            });
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="sourceFile"></param>
        /// <param name="lineNumber"></param>
        /// <param name="memberName"></param>
        private void HandleExceptions(Exception ex,
                [CallerFilePath] string sourceFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            CryoviewTools.LogMessage(LogLevel.Err, " Entering ... ");
            string trace = sourceFile.Substring(sourceFile.LastIndexOf('\\') + 1) + "(" + lineNumber + "): " + memberName + " --> ";
            int stopAt = 0;
            int len = ex.ToString().Length;
            if (len < 100) stopAt = len - 1; else stopAt = 99;
            if (ex.GetType() == typeof(DirectoryNotFoundException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(trace + Environment.NewLine + ex.ToString());
            }
            else if (ex.GetType() == typeof(IOException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(trace + Environment.NewLine + ex.ToString().Substring(0, stopAt));
            }
            else if (ex.GetType() == typeof(UnauthorizedAccessException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(trace + Environment.NewLine + ex.ToString().Substring(0, stopAt));
            }
            else
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                string msg = ex.ToString().Substring(0, ex.ToString().IndexOf('-'));
                OnExceptionRaised(msg + " " + trace + " " + ex.Message);
            }
            CryoviewTools.LogMessage(LogLevel.Err, " Exiting ");
        }   // private void HandleExceptions(Exception ex,

        /// <summary>
        ///  Try to get problem notifications back to the user via the Status Dependency Property, but something may go 
        ///  seriously wrong before the initialization is complete and the Status DP is bound to the view.
        /// </summary>
        /// <param name="msg">Description of the problem</param>
        private void OnExceptionRaised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Err, " Entering ... ");
            if (ExceptionRaised != null)    // check for subscribers
            {
                ExceptionRaised(msg);   // pass it on (to the view)
            }
            CryoviewTools.LogMessage(LogLevel.Err, " Exiting ");
        }
        #endregion utility functions

        #region event sinks
        /// <summary>
        /// Sinks any exceptions thrown by the db model
        /// </summary>
        /// <param name="msg">description of problem</param>
        /// <remarks>
        /// Generally try and use the Dependency Property Status, but something may go wrong during initialization.
        /// </remarks>
        private void m_db_ExceptionRaised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Err, " Entering ... ");
            HandleExceptions(new Exception(msg));
            CryoviewTools.LogMessage(LogLevel.Err, " Exiting ");
        }
        private void MCM_Exception_Raised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Err, "Entering ...");            
            CryoviewTools.LogMessage(LogLevel.Err, msg);
            StatusMessage = msg;
            CryoviewTools.LogMessage(LogLevel.Err, "Exiting ...");
        }
        private void MCM_Connection_Lost(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            if (m_MCMTryReconnect)
            {
                CryoviewTools.LogMessage(LogLevel.Err, msg);
                StatusMessage = msg;
                MessageBoxResult result = MessageBox.Show("Connection to the MCM lost. Reconnect?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if(result == MessageBoxResult.Yes) { _MCMModel.Connect(); }
                else { m_MCMTryReconnect = false; }
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion event sinks
    }
}
