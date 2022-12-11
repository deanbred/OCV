using System;
using System.Collections.Generic;    // KeyValuePair
using System.Collections.Concurrent;    // ConcurrentDictionary<T, T>
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gardasoft.Controller.API.Exceptions;
using Gardasoft.Controller.API.Interfaces;
using Gardasoft.Controller.API.Managers;
using Gardasoft.Controller.API.Model;
using Gardasoft.Controller.API.EventsArgs;

//using Cryoview_ModuleMessages;
using Cryoview_Tools;   // logging
using LLE.Util;         // logging
using Illuminator_Interface;

namespace Illuminator_Gardasoft_Model
{
    public class IlluminatorGardasoftModel : IDisposable, IlluminatorInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars

        object m_objLock = new object();
        private string SerialNumber;
        private ControllerManager controllerManager;
        private IChannel Channel1;
        private IChannel Channel2;
        private IController activeController;

        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public IlluminatorGardasoftModel(string serialNumber)
        {
            SerialNumber = serialNumber;   
        }

        /// <summary>
        /// Got here by user code.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {       // managed resources here
                if(activeController != null)
                {
                    activeController.ConnectionStatusChanged -= Illuminator_Connection_Status_Changed;
                    activeController.Close();
                }

            }
            // unmanaged resources here
            {
            }
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization

        public bool Initialize()
        {
            CryoviewTools.LogMessage(LLE.Util.LogLevel.Debug7, "Initializing Illuminator");
            bool bRetVal = true;
            bool controllerfound = false;

            controllerManager = ControllerManager.Instance();
            controllerManager.DiscoverControllers();
            if (controllerManager.Controllers.Count > 0)
            {
                foreach (IController controller in controllerManager.Controllers)
                {
                    if(controller.SerialNumber.ToString() == SerialNumber)
                    {
                        controllerfound = true;
                        try
                        {
                            activeController = controller;
                            activeController.Open();
                            CryoviewTools.LogMessage(LLE.Util.LogLevel.Info, "Connected to illuminator.");
                            activeController.ConnectionStatusChanged += Illuminator_Connection_Status_Changed;
                            Channel1 = activeController.Channels[0];
                            Channel2 = activeController.Channels[1];
                        }
                        catch (Exception ex)
                        {
                            CryoviewTools.LogMessage(LLE.Util.LogLevel.Err, "Failed to connect to illuminator: " + ex.Message);
                            OnExceptionRaised("Failed to connect to illuminator: " + ex.Message);
                            if (activeController.IsOpen) { activeController.Close(); }
                            activeController = null;
                            bRetVal = false;
                        }
                        
                    }
                } 
            }
            if(!controllerfound)
            {
                //failed to connect to controller
                CryoviewTools.LogMessage(LLE.Util.LogLevel.Err, "Failed to connect to illuminator. Illuminator with serial number " + SerialNumber + " was not found.");
                OnExceptionRaised("Failed to connect to illuminator. Illuminator with serial number " + SerialNumber + " was not found.");
                bRetVal = false;
            }

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
        public Single CH1DiodeBrightness
        {
            get
            {
                if (activeController != null) { return (Single)Channel1.Registers["Brightness"].CurrentValue; }
                else { return 0.0f; }
            }
            set
            {
                if (activeController != null) { Channel1.Registers["Brightness"].CurrentValue = value; }
            }
        }
        public Single CH2DiodeBrightness
        {
            get
            {
                if (activeController != null) { return (Single)Channel2.Registers["Brightness"].CurrentValue; }
                else { return 0.0f; }
            }
            set
            {
                if (activeController != null) { Channel2.Registers["Brightness"].CurrentValue = value; }
            }
        }
        public Single CH1PulseDurration 
        {
            get 
            {
                if (activeController != null) { return (Single)Channel1.Registers["PulseWidth"].CurrentValue; }
                else { return 0.0f; }
            } 
            set 
            {
                if(activeController != null) { Channel1.Registers["PulseWidth"].CurrentValue = value; }
            }
        }
        public Single CH2PulseDurration
        {
            get
            {
                if (activeController != null) { return (Single)Channel2.Registers["PulseWidth"].CurrentValue; }
                else { return 0.0f; }
            }
            set
            {
                if (activeController != null) { Channel2.Registers["PulseWidth"].CurrentValue = value; }
            }
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
        private void OnExceptionRaised(string msg)
        {
            if (ExceptionRaised != null)    // check for subscribers
            {
                ExceptionRaised(msg);   // pass it on (to the view)
            }
        }
        #endregion utility functions

        #region event sinks
        /// <summary>
        /// Handles the ConnectionStatusChanged event of the controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Gardasoft.Controller.API.EventsArgs.ControllerConnectionStatusChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void Illuminator_Connection_Status_Changed(object sender, ControllerConnectionStatusChangedEventArgs e)
        {
            if(e.ConnectionStatus == ControllerConnectionStatus.Fault) 
            {
                CryoviewTools.LogMessage(LLE.Util.LogLevel.Err, "Lost connection to illuminator");
                OnExceptionRaised("Lost connection to the illuminator, attempting to reconnect.");
                activeController.ConnectionStatusChanged -= Illuminator_Connection_Status_Changed;
                activeController.Close();
                activeController = null;
                Initialize();
            }
        }
        #endregion event sinks
    }
}
