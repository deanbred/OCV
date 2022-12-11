using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryoview_Tools;   // logging
using LLE.Util;         // logging
using Objective_Interface;
using Navitar;

namespace Objective_Navitar_Model
{
    public class ObjectiveNavitarModel : IDisposable, ObjectiveInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private Controller NavController = null;
        private string SerialNumber = "";
        private bool idle = false;
        private bool hasTriedReconnect = false;

        private int m_ZoomTargetPos = 0;
        private int m_FocusTargetPos = 0;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public ObjectiveNavitarModel(string serialNumber)
        {
            SerialNumber = serialNumber;
            ControllerHub.DiscoverControllers();
            ICollection<Controller> controllers = ControllerHub.GetAll();

            foreach(Controller C in controllers)
            {
                if( C.Read(Controller.regProductSerialNum).ToString() == SerialNumber) 
                { 
                    NavController = C;
                    NavController.Connect();
                    if (NavController.Connected) { CryoviewTools.LogMessage(LogLevel.Info, "Sucessfully connected to Navitar with serial number " + serialNumber); }
                    else 
                    { 
                        CryoviewTools.LogMessage(LogLevel.Warning, "Failed to connect to Navitar with serial number " + serialNumber);
                        OnExceptionRaised("Failed to connect to Navitar with serial number " + SerialNumber);
                    }
                    return;
                }
            }

            CryoviewTools.LogMessage(LogLevel.Warning, "Failed to find to Navitar with serial number " + SerialNumber);
            OnExceptionRaised("Failed to find to Navitar with serial number " + SerialNumber);
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
                if (NavController != null)
                {
                    CryoviewTools.LogMessage(LogLevel.Info, "Disposing of Navitar with serial number " + SerialNumber);
                    if (NavController.Connected) { NavController.Disconnect(); }
                    NavController.Dispose();
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
            if(NavController != null)
            {
                return NavController.Connected;
            }
            return false;
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        public int ZoomLimit
        {
            get 
            { 
                if(NavController != null && NavController.Connected)
                {
                    int limit = NavController.Read(Controller.regSetupLimit_1);
                    return limit;
                }
                else if(NavController != null && !NavController.Connected)
                {
                    OnExceptionRaised("Lost connectiong to the Navitar with serial number " + SerialNumber);
                }
                return 0;
            }
        }

        public int FocusLimit
        {
            get
            {
                if (NavController != null && NavController.Connected)
                {
                    return NavController.Read(Controller.regSetupLimit_2);
                }
                else if (NavController != null && !NavController.Connected)
                {
                    OnExceptionRaised("Lost connectiong to the Navitar with serial number " + SerialNumber);
                }
                return 0;
            }
        }
        
        public int ZoomCurrentPos
        {
            get
            {
                if (NavController != null && NavController.Connected)
                {
                    return NavController.Read(Controller.regCurrent_1); ;
                }
                else if (NavController != null && !NavController.Connected)
                {
                    OnExceptionRaised("Lost connectiong to the Navitar with serial number " + SerialNumber);
                }
                return 0;
            }
        }

        public int FocusCurrentPos
        {
            get
            {
                if (NavController != null && NavController.Connected)
                {
                    return NavController.Read(Controller.regCurrent_2);
                }
                else if (NavController != null && !NavController.Connected)
                {
                    OnExceptionRaised("Lost connectiong to the Navitar with serial number " + SerialNumber);
                }
                return 0;
            }
        }
        #endregion properties

        #region bindable properties
        public int ZoomTargetPos
        {
            set
            {
                m_ZoomTargetPos = value;
                UpdateMotorPosition("zoom");
            }
        }

        public int FocusTargetPos
        {
            set 
            {
                m_FocusTargetPos = value;
                UpdateMotorPosition("focus");
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
        private void reconnect()
        {
            if (NavController != null && NavController.Connected) { hasTriedReconnect = false; return; /*already connected*/ }
            else if(NavController != null) { NavController.Dispose(); NavController = null; }

            ControllerHub.DiscoverControllers();
            ICollection<Controller> controllers = ControllerHub.GetAll();

            foreach (Controller C in controllers)
            {
                if (C.Read(Controller.regProductSerialNum).ToString() == SerialNumber)
                {
                    NavController = C;
                    NavController.Connect();
                    if (NavController.Connected) 
                    { 
                        CryoviewTools.LogMessage(LogLevel.Info, "Sucessfully connected to Navitar with serial number " + SerialNumber); 
                        hasTriedReconnect = false; 
                    }
                    else 
                    { 
                        CryoviewTools.LogMessage(LogLevel.Warning, "Failed to connect to Navitar with serial number " + SerialNumber); 
                        hasTriedReconnect = true;
                        OnExceptionRaised("Failed to connect to Navitar with serial number " + SerialNumber);
                    }
                    return;
                }
            }

            CryoviewTools.LogMessage(LogLevel.Warning, "Failed to find to Navitar with serial number " + SerialNumber);
            hasTriedReconnect = true;
            OnExceptionRaised("Failed to find to Navitar with serial number " + SerialNumber);
        }
        private void UpdateMotorPosition(string motor)
        {
            if (NavController == null || !NavController.Connected)
            {
                OnExceptionRaised("Lost connectiong to the Navitar with serial number " + SerialNumber);
                return;
                
            }
            else if (motor == "zoom")
            {
                int status = NavController.Read(Controller.regStatus_1);
                idle = (((uint)status & 0x000000ff) == 0) ? true : false;
                if (idle) 
                { 
                    NavController.Write(Controller.regTarget_1, m_ZoomTargetPos);
                }   
            }
            else
            {
                int status = NavController.Read(Controller.regStatus_2);
                idle = (((uint)status & 0x000000ff) == 0) ? true : false;
                if (idle) 
                { 
                    NavController.Write(Controller.regTarget_2, m_FocusTargetPos); 
                }   
            }

        }
        private void OnExceptionRaised(string msg)
        {
            if (!hasTriedReconnect) 
            {             
                if (ExceptionRaised != null)    // check for subscribers
                {
                    ExceptionRaised(msg);   // pass it on (to the view)
                }
                reconnect(); 
            }
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
