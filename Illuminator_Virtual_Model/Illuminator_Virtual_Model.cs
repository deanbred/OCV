using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryoview_Tools;   // logging
using LLE.Util;         // logging
using Illuminator_Interface;

namespace Illuminator_Virtual_Model
{
    public class IlluminatorVirtualModel : IDisposable, IlluminatorInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public IlluminatorVirtualModel()
        {
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
            CH1DiodeBrightness = 25.0f;
            CH2DiodeBrightness = 25.0f;
            CH1PulseDurration = 5.0f;
            CH2PulseDurration = 5.0f;

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
        public Single CH1DiodeBrightness { get; set; }
        public Single CH2DiodeBrightness { get; set; }
        public Single CH1PulseDurration { get; set; }
        public Single CH2PulseDurration { get; set; }
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
        #endregion utility functions

        #region event sinks
        #endregion event sinks

    }
}
