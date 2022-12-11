using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.ComponentModel.Composition; // MEF, reqs ref to System.ComponentModel.Composition
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Prism.Commands;
using Prism.Mvvm;

using ICryoview;
using Target_Filling_ViewModel;
using Cryoview_Tools;   // logging
using LLE.Util; // logging

namespace Target_Filling_View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TargetFillingView : UserControl, IDisposable, ICryoviewWindow
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars
        private bool m_bUserControlLoaded = false;
        private TargetFillingViewModel m_viewModel = null;
        private Valve_State_View m_Popout = null;
        private bool m_IsFilling = false;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public TargetFillingView()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            InitializeComponent();
            m_viewModel = new TargetFillingViewModel();
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        /// <summary>
        /// Called by user-code. 
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            Dispose(true);
            GC.SuppressFinalize(this);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        /// <summary>
        /// Called by either user-code or the runtime. If runtime, disposing = false;
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (disposing)
            {       // managed resources here
                m_Popout?.Dispose();
                m_viewModel.ExceptionRaised -= vm_ExceptionRaised;
                m_viewModel.Dispose();
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
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
        private void TargetFillingView_Loaded(object sender, RoutedEventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            //only load once
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;
                m_viewModel.Initialize();
                this.DataContext = m_viewModel;
                m_viewModel.ExceptionRaised += vm_ExceptionRaised;

                //m_Popout = new Window1(m_viewModel);
                //m_Popout.Show();
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        private void cmdFill_Start_Click(object sender, EventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (!m_IsFilling)
            {
                m_IsFilling = true;
                m_Popout = new Valve_State_View(m_viewModel);
                m_Popout.Show();
                m_viewModel.Start_Target_Fill();
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        private void cmdFill_Stop_Click(object sender, EventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (m_IsFilling)
            {
                m_viewModel.Stop_Target_Fill();
                if(m_Popout != null) { m_Popout.Close(); }
                m_IsFilling = false;
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
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
        public String ServiceName { get { return " Target Filling "; } }
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
        private void vm_ExceptionRaised(string msg, bool b)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (b && m_Popout != null) { m_Popout.Close(); }
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion event sinks
    }
}
