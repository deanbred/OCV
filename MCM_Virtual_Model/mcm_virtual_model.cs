using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCM_Interface;
using Cryoview_Tools;   // logging
using LLE.Util;         // logging

namespace MCM_Virtual_Model
{
    public class MCMVirtualModel : IDisposable, MCMInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        public event DelConnectionLost ConnectionLost;
        #endregion delegates, events

        #region backing vars
        private Single _LSTemp = 1.0f;
        private Single _CFETemp = 1.0f;
        #endregion backing vars

        #region ctors/dtors/dispose
        public MCMVirtualModel()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
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
            if (disposing)
            {       // managed resources here
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
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
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
        public Single LSSetPoint { get; private set; } = 1.0f;
        public Single CFESetPoint { get; private set; } = 1.0f;
        public Valve_Status ValveToTarget { get; private set; } = Valve_Status.MCMCONTROLLED;
        public Valve_Status ValveToDT { get; private set; } = Valve_Status.MCMCONTROLLED;
        public Valve_Status ValveToUBed { get; private set; } = Valve_Status.MCMCONTROLLED;
        public bool ControlOverValves { get; private set; } = false;
        public bool IsLeak { get; private set; } = false;
        public bool IsLeakAtTarget { get; private set; } = false;
        #endregion properties

        #region bindable properties
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        #endregion algorithm code

        #region hardware code
        public void Connect()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        public void SetLSTemp(Single temp)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");            
            _LSTemp = temp;
            LSSetPoint = temp;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        public void SetCFETemp(Single temp)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");            
            _CFETemp = temp;
            CFESetPoint = temp;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        public Single GetLSTemp()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return _LSTemp;
        }
        public Single GetCFETemp()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return _CFETemp;
        }
        public bool RequestValveControl(bool control)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            ControlOverValves = control;
            if( control == true)
            {
                ValveToTarget = Valve_Status.CLOSED;
                ValveToDT = Valve_Status.CLOSED;
                ValveToUBed = Valve_Status.CLOSED;
            }
            else
            {
                ValveToTarget = Valve_Status.MCMCONTROLLED;
                ValveToDT = Valve_Status.MCMCONTROLLED;
                ValveToUBed = Valve_Status.MCMCONTROLLED;
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return true;
        }
        public bool SetValveStatus(Valve_Status valve2Target, Valve_Status valve2DT, Valve_Status valve2UBed)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");           
            if (valve2Target == Valve_Status.MCMCONTROLLED ||
                valve2DT == Valve_Status.MCMCONTROLLED ||
                valve2UBed == Valve_Status.MCMCONTROLLED) { return false; }

            ValveToTarget = valve2Target;
            ValveToDT = valve2DT;
            ValveToUBed = valve2UBed;
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return true;
        }
        #endregion hardware code

        #region utility functions
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
