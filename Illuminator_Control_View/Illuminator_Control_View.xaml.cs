using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition; // MEF, reqs ref to System.ComponentModel.Composition
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;    

using ICryoview;
using Illuminator_Control_ViewModel;
using Cryoview_ModuleMessages;

namespace Illuminator_Control_View
{
    public partial class IlluminatorControlView : UserControl, IDisposable, ICryoviewWindow
    {

        #region delegates, events
        #endregion delegates, events

        #region backing vars
        bool m_bUserControlLoaded = false;
        IlluminatorControlViewModel m_IlluminatorControlViewModel = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public IlluminatorControlView()
        {
            InitializeComponent();
            m_IlluminatorControlViewModel = new IlluminatorControlViewModel();
            m_IlluminatorControlViewModel.ExceptionRaised += Illuminator_ExceptionRaised;
            CommandMessageEvent.Instance.Subscribe(CommandRecieved);
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
                CommandMessageEvent.Instance.Unsubscribe(CommandRecieved);
                m_IlluminatorControlViewModel.ExceptionRaised -= Illuminator_ExceptionRaised;
                m_IlluminatorControlViewModel.Dispose();
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
        private void IlluminatorView_Loaded(object sender, RoutedEventArgs e)
        {
            //only load once
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;

                m_IlluminatorControlViewModel.Initialize();
                this.DataContext = m_IlluminatorControlViewModel;
            }
        }

        private void cmdSet_CH1_Click(object sender, EventArgs e)
        {
            if (m_IlluminatorControlViewModel.m_IlluninatorConnected) { m_IlluminatorControlViewModel.SetCh1Settings(); }
            else { MessageBox.Show("No illuminator connected", "Error", MessageBoxButton.OK,MessageBoxImage.Error); }
        }
        private void cmdSet_CH2_Click(object sender, EventArgs e)
        {
            if (m_IlluminatorControlViewModel.m_IlluninatorConnected) { m_IlluminatorControlViewModel.SetCh2Settings(); }
            else { MessageBox.Show("No illuminator connected", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void cmdSave_Low_CH1_Click(object sender, EventArgs e)
        {
            m_IlluminatorControlViewModel.CH1DiodeBrightnessLow = m_IlluminatorControlViewModel.CH1DiodeBrightness;
            m_IlluminatorControlViewModel.CH1PulseDurationLow = m_IlluminatorControlViewModel.CH1PulseDuration;
        }
        private void cmdSave_Low_CH2_Click(object sender, EventArgs e)
        {
            m_IlluminatorControlViewModel.CH2DiodeBrightnessLow = m_IlluminatorControlViewModel.CH2DiodeBrightness;
            m_IlluminatorControlViewModel.CH2PulseDurationLow = m_IlluminatorControlViewModel.CH2PulseDuration;
        }
        private void cmdSave_High_CH1_Click(object sender, EventArgs e)
        {
            m_IlluminatorControlViewModel.CH1DiodeBrightnessHigh = m_IlluminatorControlViewModel.CH1DiodeBrightness;
            m_IlluminatorControlViewModel.CH1PulseDurationHigh = m_IlluminatorControlViewModel.CH1PulseDuration;
        }
        private void cmdSave_High_CH2_Click(object sender, EventArgs e)
        {
            m_IlluminatorControlViewModel.CH2DiodeBrightnessHigh = m_IlluminatorControlViewModel.CH2DiodeBrightness;
            m_IlluminatorControlViewModel.CH2PulseDurationHigh = m_IlluminatorControlViewModel.CH2PulseDuration;
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
        public String ServiceName { get { return " Illuminator "; } }
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
        private void CommandRecieved(object state)
        {
            CommandMessage msg = (CommandMessage)(state);
            if (msg.ID == "Illuminator" && !m_IlluminatorControlViewModel.m_IlluninatorConnected)
            {
                MessageBox.Show("No illuminator connected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion utility functions

        #region event sinks
        private void Illuminator_ExceptionRaised(string msg)
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
