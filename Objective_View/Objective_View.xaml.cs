using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition; // MEF, reqs ref to System.ComponentModel.Composition
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using ICryoview;
using Objective_ViewModel;
using Cryoview_ModuleMessages;

namespace Objective_View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ObjectiveView : UserControl, IDisposable, ICryoviewWindow
    {

        #region delegates, events
        #endregion delegates, events

        #region backing vars
        bool m_bUserControlLoaded = false;
        ObjectiveViewModel m_ObjectiveViewModel = null;
        private bool XObjectiveUserInputEnabled = true;
        private bool YObjectiveUserInputEnabled = true;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public ObjectiveView()
        {
            InitializeComponent();
            m_ObjectiveViewModel = new ObjectiveViewModel();
            m_ObjectiveViewModel.ExceptionRaised += Objective_ExceptionRaised;

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
                EnableUserObjectiveControlMessageEvent.Instance.Unsubscribe(EnableDisableObjectiveControl);
                m_ObjectiveViewModel.ExceptionRaised -= Objective_ExceptionRaised;
                m_ObjectiveViewModel.Dispose();
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
        private void ObjectiveView_Loaded(object sender, RoutedEventArgs e)
        {
            //only load once
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;

                m_ObjectiveViewModel.Initialize();
                this.DataContext = m_ObjectiveViewModel;
                EnableUserObjectiveControlMessageEvent.Instance.Subscribe(EnableDisableObjectiveControl);
            }
        }

        private void cmd_Increase_Zoom_Click(object sender, EventArgs e)
        {
            if ((Button)sender == XZoomIncrease1btn && XObjectiveUserInputEnabled) 
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XZoomCurPosition + 1, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Zoom); 
            }
            else if ((Button)sender == XZoomIncrease10btn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XZoomCurPosition + 10, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == YZoomIncrease1btn && YObjectiveUserInputEnabled) 
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YZoomCurPosition + 1, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == YZoomIncrease10btn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YZoomCurPosition + 10, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Zoom);
            }
            else { MessageBox.Show("Unable to move objective while auto focus is running.", "Auto Focus Is Running", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private void cmd_Decrease_Zoom_Click(object sender, EventArgs e)
        {
            if ((Button)sender == XZoomDecrease1btn && XObjectiveUserInputEnabled) 
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XZoomCurPosition - 1, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Zoom); 
            }
            else if ((Button)sender == XZoomDecrease10btn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XZoomCurPosition - 10, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == YZoomDecrease1btn && YObjectiveUserInputEnabled) 
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YZoomCurPosition - 1, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Zoom); 
            }
            else if ((Button)sender == YZoomDecrease10btn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YZoomCurPosition - 10, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Zoom);
            }
            else { MessageBox.Show("Unable to move objective while auto focus is running.", "Auto Focus Is Running", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }
       
        private void cmd_Increase_Focus_Click(object sender, EventArgs e)
        {
            if ((Button)sender == XFocusIncrease1btn && XObjectiveUserInputEnabled) 
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XFocusCurPosition + 1, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Focus); 
            }
            else if ((Button)sender == XFocusIncrease10btn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XFocusCurPosition + 10, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == YFocusIncrease1btn && YObjectiveUserInputEnabled) 
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YFocusCurPosition + 1, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Focus); 
            }
            else if ((Button)sender == YFocusIncrease10btn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YFocusCurPosition + 10, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Focus);
            }
            else { MessageBox.Show("Unable to move objective while auto focus is running.", "Auto Focus Is Running", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private void cmd_Decrease_Focus_Click(object sender, EventArgs e)
        {
            if ((Button)sender == XFocusDecrease1btn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XFocusCurPosition - 1, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == XFocusDecrease10btn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XFocusCurPosition - 10, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == YFocusDecrease1btn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YFocusCurPosition - 1, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == YFocusDecrease10btn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YFocusCurPosition - 10, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Focus);
            }
            else { MessageBox.Show("Unable to move objective while auto focus is running.", "Auto Focus Is Running", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private void cmd_Zoom_Go_Click(object sender, EventArgs e)
        {
            if ((Button)sender == XZoomGoToPositionbtn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XZoomGoToPosition, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == XZoomGoToLimitbtn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XZoomLimit, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == XZoomGoToHomebtn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XZoomHome, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == YZoomGoToPositionbtn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YZoomGoToPosition, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == YZoomGoToLimitbtn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YZoomLimit, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Zoom);
            }
            else if ((Button)sender == YZoomGoToHomebtn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YZoomHome, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Zoom);
            }
            else { MessageBox.Show("Unable to move objective while auto focus is running.", "Auto Focus Is Running", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private void cmd_Focus_Go_Click(object sender, EventArgs e)
        {
            if ((Button)sender == XFocusGoToPositionbtn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XFocusGoToPosition, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == XFocusGoToLimitbtn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XFocusLimit, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == XFocusGoToHomebtn && XObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.XFocusHome, ObjectiveViewModel.Axis.XAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == YFocusGoToPositionbtn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YFocusGoToPosition, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == YFocusGoToLimitbtn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YFocusLimit, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Focus);
            }
            else if ((Button)sender == YFocusGoToHomebtn && YObjectiveUserInputEnabled)
            {
                m_ObjectiveViewModel.GoToPosition(m_ObjectiveViewModel.YFocusHome, ObjectiveViewModel.Axis.YAxis, ObjectiveViewModel.Function.Focus);
            }
            else { MessageBox.Show("Unable to move objective while auto focus is running.", "Auto Focus Is Running", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
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
        public String ServiceName { get { return " Zoom/Focus "; } }
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
        private void EnableDisableObjectiveControl( EnableUserObjectiveControlMessage msg)
        {
            if (msg.ViewAxis == "OpticalView1")
            {
                XObjectiveUserInputEnabled = msg.Enabled;
            }
            else { YObjectiveUserInputEnabled = msg.Enabled; }
        }
        #endregion utility functions

        #region event sinks
        private void Objective_ExceptionRaised(string msg)
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
