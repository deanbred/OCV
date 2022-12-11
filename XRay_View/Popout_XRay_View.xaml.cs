using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using XRay_View_Model;

namespace XRay_View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PopoutXRayView : Window, IDisposable
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars
        bool m_bUserControlLoaded = false;
        XRayViewModel m_ViewModel = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        /// <summary>
        /// </summary>
        public PopoutXRayView (XRayViewModel model)
        {
            m_ViewModel = model;
            InitializeComponent();
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
                this.Close();
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
        private void PopoutXRayView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;
                this.DataContext = m_ViewModel;
            }
        }

        private void cmdZoomIn_Click(object sender, RoutedEventArgs e)
        {
            controlZoomPan.ContentScale += 0.1;
        }

        private void cmdZoomOut_Click(object sender, RoutedEventArgs e)
        {
            controlZoomPan.ContentScale -= 0.1;
        }

        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
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
        #endregion event sinks}
    }
}
