using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cryoview_Tools;   // logging
using LLE.Util;         // logging
using Objective_Interface;

namespace Objective_Virtual_Model
{
    public class ObjectiveVirtualModel : IDisposable, ObjectiveInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private int m_ZoomPos = 0;
        private int m_FocusPos = 0;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public ObjectiveVirtualModel()
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
            return true;
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
            get { return 100; }
        }
        
        public int FocusLimit
        {
            get { return 100; }
        }
        public int ZoomCurrentPos 
        { 
            get { return m_ZoomPos; } 
        }
        public int FocusCurrentPos 
        { 
            get { return m_FocusPos; } 
        }

        #endregion properties

        #region bindable properties
        public int ZoomTargetPos 
        { 
            set { m_ZoomPos = value; } 
        }
        public int FocusTargetPos 
        { 
            set { m_FocusPos = value; } 
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
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
