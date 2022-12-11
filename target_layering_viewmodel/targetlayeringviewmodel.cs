using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Concurrent;    // ConcurrentDictionary<T, T>
using System.Collections.ObjectModel;
//using System.Data.Odbc;     // read palettes in from file; reqs ref to System.Data
using System.IO;
using System.Reflection;
using System.Windows;   // Visibility
using System.Windows.Input; // for ICommand; requires ref to PresentationCore
using System.Threading;
using Prism.Commands;
using Prism.Mvvm;
using System.Runtime.CompilerServices;

using Cryoview_ModuleMessages;
using Cryoview_Tools;   // logging
using LLE.Util; // logging

namespace Target_Layering_ViewModel
{
    public class TargetLayeringViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private Single m_SetColdFingerTemp = 0.0f;
        private Single m_SetLayeringSphereTemp = 0.0f;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public TargetLayeringViewModel()
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
        public void Initialize()
        {
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        public Single SetColdFingerTemp { 
            get
            {
                return m_SetColdFingerTemp;
            }
            set
            {
                SetProperty<Single>(ref m_SetColdFingerTemp, value);
                RaisePropertyChanged(nameof(SetColdFingerTemp));
            }
        }
        public Single SetLayeringSphereTemp
        {
            get
            {
                return m_SetLayeringSphereTemp;
            }
            set
            {
                SetProperty<Single>(ref m_SetLayeringSphereTemp, value);
                RaisePropertyChanged(nameof(SetLayeringSphereTemp));
            }
        }
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
        #endregion hardware code

        #region utility functions
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
