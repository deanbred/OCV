using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition; // MEF, reqs ref to System.ComponentModel.Composition
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using ICryoview;
using Focus_ViewModel;
using Cryoview_ModuleMessages;
using Cryoview_Tools;   // logging
using LLE.Util; // logging

namespace Focus_View
{
    /// <summary>
    /// Interaction logic for FocusView.xaml
    /// </summary>
    public partial class FocusView : UserControl, IDisposable, ICryoviewWindow
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars
        bool m_bUserControlLoaded = false;
        FocusViewModel m_FocusViewModel = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public FocusView()
        {
            InitializeComponent();
            m_FocusViewModel = new FocusViewModel();
            m_FocusViewModel.ExceptionRaised += VM_ExceptionRaised;
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
                m_FocusViewModel.ExceptionRaised -= VM_ExceptionRaised;
                m_FocusViewModel.Dispose();
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
        private void FocusView_Loaded(object sender, RoutedEventArgs e)
        {
            //only load once
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;

                m_FocusViewModel.Initialize();
                this.DataContext = m_FocusViewModel;
            }
        }

        private void cmdSetROI_Click(object sender, EventArgs e)
        {
            int v = 0;
            if (sender == setROI1btn && !m_FocusViewModel.AutofocusRunningV1)
            {
                v = 1;
                ROISet1lbl.Background = System.Windows.Media.Brushes.Coral;
            }
            else if (!m_FocusViewModel.AutofocusRunningV2)
            {
                v = 2;
                ROISet2lbl.Background = System.Windows.Media.Brushes.Coral;
            }

            if(v > 0) { m_FocusViewModel.SetROI(v); }
            else { MessageBox.Show("Can not set or remove ROI while auto focusing. Please cancel auto focus first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void cmdRemoveROI_Click(object sender, EventArgs e)
        {
            int v = 0;
            if (sender == removeROI1btn && !m_FocusViewModel.AutofocusRunningV1)
            {
                v = 1;
                ROISet1lbl.Background = System.Windows.Media.Brushes.Transparent;
            }
            else if (!m_FocusViewModel.AutofocusRunningV2)
            {
                v = 2;
                ROISet2lbl.Background = System.Windows.Media.Brushes.Transparent;
            }

            if (v > 0) { m_FocusViewModel.RemoveROI(v); }
            else { MessageBox.Show("Can not set or remove ROI while auto focusing. Please cancel auto focus first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
        private void cmdMeasure_Focus_Click(object sender, EventArgs e)
        {
            int v;
            if (sender == measureFocus1btn) { v = 1; }
            else { v = 2; }
            m_FocusViewModel.MeasureFocus(v, false);
        }
        private void cmdAuto_Focus_Click(object sender, EventArgs e)
        {
            int v;
            if (sender == findBestFocus1btn) { v = 1; }
            else { v = 2; }
            m_FocusViewModel.StartAutoFocus(v);
        }
        private void cmdAuto_Focus_Cancel_Click(object sender, EventArgs e)
        {
            if( sender == cancelFocus1btn && m_FocusViewModel.AutofocusRunningV1)
            {
                m_FocusViewModel.UserCancelV1 = true;
            }
            else if(sender == cancelFocus2btn && m_FocusViewModel.AutofocusRunningV2)
            {
                m_FocusViewModel.UserCancelV2 = true;
            }
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
        public String ServiceName { get { return " Focus "; } }
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
        private void VM_ExceptionRaised(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ErrorOrStatusMessageEvent.Instance.Publish(new ErrorOrStatusMessage
            {
                messageType = MessageType.Error,
                Message = msg
            });
        }
        #endregion event sinks

    }
}
