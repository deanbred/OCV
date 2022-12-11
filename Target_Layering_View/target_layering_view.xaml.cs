using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition; // MEF, reqs ref to System.ComponentModel.Composition
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using ICryoview;
using Cryoview_ModuleMessages;
using Target_Layering_ViewModel;

namespace Target_Layering_View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TargetLayeringView : UserControl, IDisposable, ICryoviewWindow
    {

        #region delegates, events
        #endregion delegates, events

        #region backing vars
        bool m_bUserControlLoaded = false;
        TargetLayeringViewModel m_targetLayeringModel = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public TargetLayeringView()
        {
            InitializeComponent();
            m_targetLayeringModel = new TargetLayeringViewModel();
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
        private void TargetLayeringView_Loaded(object sender, RoutedEventArgs e)
        {
            //only load once
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;
                m_targetLayeringModel.Initialize();
                this.DataContext = m_targetLayeringModel;
            }
        }
        private void cmdEnable_Crystal_Catch_Click(Object sender, EventArgs e)
        {

        }
        private void cmdClose_DT_Valve_Click(Object sender, EventArgs e)
        {

        }
        private void cmdSet_Cold_Finger_Click(object sender, EventArgs e)
        {
            MCMTarget target = MCMTarget.CFE;
            MCMTempDirection direction = MCMTempDirection.MINUS;
            Single tempurature = 0.0f;
            if(sender == SetColdFingerbtn)
            {
                direction = MCMTempDirection.SET;
                tempurature = m_targetLayeringModel.SetColdFingerTemp;
            }
            else if(sender == SetColdFingerPlus5btn)
            {
                direction = MCMTempDirection.PLUS;
                tempurature = 0.005f;
            }
            else if(sender == SetColdFingerPlus1btn)
            {
                direction = MCMTempDirection.PLUS;
                tempurature = 0.001f;
            }
            else if(sender == SetColdFingerMinus5btn)
            {
                tempurature = 0.005f;
            }
            else if (sender == SetColdFingerMinus1btn)
            {
                tempurature = 0.001f;
            }
            MCMTemperatureMessageEvent.Instance.Publish(new MCMTemperatureMessage 
            {
                Target = target,
                Direction = direction,
                Temperature = tempurature
            });
        }
        private void cmdSet_Layering_Sphere_Click(object sender, EventArgs e)
        {
            MCMTarget target = MCMTarget.LS;
            MCMTempDirection direction = MCMTempDirection.MINUS;
            Single tempurature = 0.0f;
            if (sender == SetLayeringSphereTempbtn)
            {
                direction = MCMTempDirection.SET;
                tempurature = m_targetLayeringModel.SetLayeringSphereTemp;
            }
            else if (sender == SetLayeringSphereTempPlus5btn)
            {
                direction = MCMTempDirection.PLUS;
                tempurature = 0.005f;
            }
            else if (sender == SetLayeringSphereTempPlus1btn)
            {
                direction = MCMTempDirection.PLUS;
                tempurature = 0.001f;
            }
            else if (sender == SetLayeringSphereTempMinus5btn)
            {
                tempurature = 0.005f;
            }
            else if (sender == SetLayeringSphereTempMinus1btn)
            {
                tempurature = 0.001f;
            }
            MCMTemperatureMessageEvent.Instance.Publish(new MCMTemperatureMessage
            {
                Target = target,
                Direction = direction,
                Temperature = tempurature
            });
        }
        private void cmdSingle_Crystal_Click(object sender, EventArgs e)
        {

        }
        private void cmdRing_Established_Click(object sender, EventArgs e)
        {

        }
        private void cmdLayering_Click(object sender, EventArgs e)
        {

        }
        private void cmdDefect_Click(object sender, EventArgs e)
        {

        }
        private void cmdDefect_Found_Click(object sender, EventArgs e)
        {

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
        public String ServiceName { get { return " Target Layering "; } }
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
        #endregion event sinks
    }
}
