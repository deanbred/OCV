using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition; // MEF, reqs ref to System.ComponentModel.Composition
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using ICryoview;
using HDF_ViewModel;
using Cryoview_ModuleMessages;
using Cryoview_Tools;   // logging
using LLE.Util; // logging

namespace HDF_View
{
    public partial class HDFView : UserControl, IDisposable, ICryoviewWindow
    {

        #region delegates, events
        #endregion delegates, events

        #region backing vars
        bool m_bUserControlLoaded = false;
        HDFViewModel m_HDFViewModel = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public HDFView()
        {
            InitializeComponent();
            m_HDFViewModel = new HDFViewModel();
            m_HDFViewModel.ExceptionRaised += HDF_ExceptionRaised;
        }
        /// <summary>
        /// Called by user-code. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Called by either user-code or the runtime. If runtime, disposing = false;
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {       // managed resources here
                m_HDFViewModel.ExceptionRaised -= HDF_ExceptionRaised;
            }
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        #endregion initialization

        #region windows events
        /// <summary>
        /// This event fires when the focus is moved back to the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HDFView_Loaded(object sender, RoutedEventArgs e)
        {
            //only load once
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;

                m_HDFViewModel.Initialize();
                this.DataContext = m_HDFViewModel;
            }
        }

        private void cmdCreate_Click(object sender, EventArgs e)
        {
            if ((Button)sender == OV1Createbtn) { m_HDFViewModel.CreateHDFFile(HDFViewModel.HDFFile.OpticalX); }
            else if ((Button)sender == OV2Createbtn) { m_HDFViewModel.CreateHDFFile(HDFViewModel.HDFFile.OpticalY); }
            else if ((Button)sender == XRVCreatebtn) { m_HDFViewModel.CreateHDFFile(HDFViewModel.HDFFile.XRay); }
        }

        private void cmdOpen_Click(object sender, EventArgs e)
        {
            if ((Button)sender == OV1Openbtn) { m_HDFViewModel.OpenHDFFile(HDFViewModel.HDFFile.OpticalX); }
            else if ((Button)sender == OV2Openbtn) { m_HDFViewModel.OpenHDFFile(HDFViewModel.HDFFile.OpticalY); }
            else if ((Button)sender == XRVOpenbtn) { m_HDFViewModel.OpenHDFFile(HDFViewModel.HDFFile.XRay); }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            if ((Button)sender == OV1Closebtn) { m_HDFViewModel.CloseHDFFile(HDFViewModel.HDFFile.OpticalX); }
            else if ((Button)sender == OV2Closebtn) { m_HDFViewModel.CloseHDFFile(HDFViewModel.HDFFile.OpticalY); }
            else if ((Button)sender == XRVClosebtn) { m_HDFViewModel.CloseHDFFile(HDFViewModel.HDFFile.XRay); }
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if ((Button)sender == OV1Savebtn) { m_HDFViewModel.SaveHDFFile(HDFViewModel.HDFFile.OpticalX); }
            else if ((Button)sender == OV2Savebtn) { m_HDFViewModel.SaveHDFFile(HDFViewModel.HDFFile.OpticalY); }
            else if ((Button)sender == XRVSavebtn) { m_HDFViewModel.SaveHDFFile(HDFViewModel.HDFFile.XRay); }
        }
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF

        /// <summary>
        /// allows MEF to make this WPF control/MVVM view available to other apps by discovery at runtime
        /// </summary>
        [Export(typeof(ICryoviewWindow))]
        public ICryoviewWindow Window
        {
            get { return this; }
        }

        /// <summary>
        /// close this control when called by user code or .net
        /// </summary>
        [Export]
        public void Close()
        {
            Dispose();
        }

        [Export]
        public String ServiceName { get { return " HDF "; } }
        #endregion MEF

        #region properties
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
        private void HDF_ExceptionRaised(string msg)
        {
            ErrorOrStatusMessageEvent.Instance.Publish(new ErrorOrStatusMessage
            {
                messageType = MessageType.Error,
                Message = msg
            });
        }
        #endregion event sinks
    }
}
