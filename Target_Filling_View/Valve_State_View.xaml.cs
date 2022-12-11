using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Target_Filling_ViewModel;
using MCM_Interface;
using Cryoview_Tools;   // logging
using LLE.Util; // logging

namespace Target_Filling_View
{
    /// <summary>
    /// Interaction logic for Valve_State_View.xaml
    /// </summary>
    public partial class Valve_State_View : Window, IDisposable
    {
        #region delegates, events
        #endregion delegates, events

        #region backing vars
        private bool m_bUserControlLoaded = false;
        private TargetFillingViewModel m_ViewModel = null;
        private Thread updateThread = null;
        private bool isDisposing = false;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public Valve_State_View(TargetFillingViewModel model)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            InitializeComponent();
            m_ViewModel = model;
            updateThread = new Thread(new ThreadStart(UpdateGUI));
            updateThread.Start();
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
                isDisposing = true;
                if (updateThread != null) { updateThread.Join(); }
                this.Close();
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
        private void ValveStateView_Loaded(object sender, RoutedEventArgs e)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (!m_bUserControlLoaded)
            {
                m_bUserControlLoaded = true;
                this.DataContext = m_ViewModel;
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
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
        private void UpdateGUI()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            while (!isDisposing)
            {
                Thread.Sleep(2000);

                m_ViewModel.Get_Valve_Status();

                if(m_ViewModel.ValveToTarget == Valve_Status.MCMCONTROLLED) 
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TargetPipeRect.Fill = Brushes.Gray; 
                        TargetValveRect.Fill = Brushes.Gray;
                    }); 
                }
                else if (m_ViewModel.ValveToTarget == Valve_Status.CLOSED) 
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TargetPipeRect.Fill = Brushes.Black; 
                        TargetValveRect.Fill = Brushes.Black;
                    }); 
                }
                else if (m_ViewModel.ValveToTarget == Valve_Status.OPEN) 
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TargetPipeRect.Fill = Brushes.Green; 
                        TargetValveRect.Fill = Brushes.Green;
                    });   
                }

                if (m_ViewModel.ValveToDT == Valve_Status.MCMCONTROLLED)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DTResPipeRect.Fill = Brushes.Gray;
                        DTResValveRect.Fill = Brushes.Gray;
                    });
                }
                else if (m_ViewModel.ValveToDT == Valve_Status.CLOSED)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DTResPipeRect.Fill = Brushes.Black;
                        DTResValveRect.Fill = Brushes.Black;
                    });
                }
                else if (m_ViewModel.ValveToDT == Valve_Status.OPEN)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DTResPipeRect.Fill = Brushes.Green;
                        DTResValveRect.Fill = Brushes.Green;
                    });
                }

                if (m_ViewModel.ValveToUBed == Valve_Status.MCMCONTROLLED)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        UBedPipeRect.Fill = Brushes.Gray;
                        UBedValveRect.Fill = Brushes.Gray;
                    });
                }
                else if (m_ViewModel.ValveToUBed == Valve_Status.CLOSED)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        UBedPipeRect.Fill = Brushes.Black;
                        UBedValveRect.Fill = Brushes.Black;
                    });
                }
                else if (m_ViewModel.ValveToUBed == Valve_Status.OPEN)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        UBedPipeRect.Fill = Brushes.Green;
                        UBedValveRect.Fill = Brushes.Green;
                    });
                }
            }
            Thread.Sleep(500);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
