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
using Objective_Interface;
using Objective_Navitar_Model;
using Objective_Virtual_Model;
using DB_Model;


namespace Objective_ViewModel
{
    public class ObjectiveViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private DBModel m_db = null;
        private bool isDisposing = false;
        private Thread GetSettings = null;
        private bool XObjectiveConnected = false;
        private bool YObjectiveConnected = false;

        private ObjectiveInterface XObjective = null;
        private ObjectiveInterface YObjective = null;

        private int m_XZoomCurPosition = 0;
        private int m_YZoomCurPosition = 0;
        private int m_XFocusCurPosition = 0;
        private int m_YFocusCurPosition = 0;

        private int m_XZoomGoToPos = 0;
        private int m_YZoomGoToPos = 0;
        private int m_XFocusGoToPos = 0;
        private int m_YFocusGoToPos = 0;
        #endregion backing vars

        #region enums
        public enum Axis { XAxis, YAxis}
        public enum Function { Zoom, Focus}
        #endregion enums

        #region ctors/dtors/dispose
        public ObjectiveViewModel() 
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Creating view model for zoom and focus");
            m_db = new DBModel();
            PushDataMessageEvent.Instance.Unsubscribe(PushMsgRecieved);
            CryoviewTools.LogMessage(LogLevel.Debug7, "Zoom and focus view model created");
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
                CryoviewTools.LogMessage(LogLevel.Info, "Disposing view model for zoom and focus");
                isDisposing = true;
                GetSettings.Join();
                PushDataMessageEvent.Instance.Unsubscribe(PushMsgRecieved);
                FocusSettingLimitsMessageEvent.Instance.Unsubscribe(SendFocusLimits);
                FocusSettingMessageEvent.Instance.Subscribe(FocusSettingMessageRecieved);
                m_db.Dispose();
                XObjective.ExceptionRaised -= XObjective_ExceptionRaised;
                YObjective.ExceptionRaised -= YObjective_ExceptionRaised;
                XObjective.Dispose();
                YObjective.Dispose();
                CryoviewTools.LogMessage(LogLevel.Info, "View model for zoom and focus disposed");
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
            SetUPObjective(Axis.XAxis);
            XObjective.ExceptionRaised += XObjective_ExceptionRaised;
            SetUPObjective(Axis.YAxis);
            YObjective.ExceptionRaised += YObjective_ExceptionRaised;
            XObjectiveConnected = XObjective.Initialize();
            YObjectiveConnected = YObjective.Initialize();
            if (!XObjectiveConnected) { OnExceptionRaised("Failed to connect to X axis objective"); }
            if (!YObjectiveConnected) { OnExceptionRaised("Failed to connect to Y axis objective"); }

            GetSettings = new Thread(new ThreadStart(GetSettingsFromObjectives));
            GetSettings.Start();
            FocusSettingLimitsMessageEvent.Instance.Subscribe(SendFocusLimits);
            FocusSettingMessageEvent.Instance.Subscribe(FocusSettingMessageRecieved);
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        #endregion properties

        #region bindable properties
        public int XZoomLimit
        {
            get 
            {
                if (!XObjectiveConnected) { return 0; }
                return XObjective.ZoomLimit; 
            }
        }

        public int XZoomHome
        {
            get { return 0; }
        }

        public int XZoomCurPosition
        {
            get 
            {
                return m_XZoomCurPosition;
            }
            set
            {
                SetProperty<int>(ref m_XZoomCurPosition, value);
                RaisePropertyChanged(nameof(XZoomCurPosition));
            }
        }

        public int XZoomGoToPosition
        {
            get { return m_XZoomGoToPos; }
            set
            {
                if(value < 0) { SetProperty<int>(ref m_XZoomGoToPos, 0); }
                else if(value > XZoomLimit) { SetProperty<int>(ref m_XZoomGoToPos, XZoomLimit); }
                else { SetProperty<int>(ref m_XZoomGoToPos, value); }   
                RaisePropertyChanged(nameof(XZoomGoToPosition));
            }
        }

        public int YZoomLimit
        {
            get 
            { 
                if (!YObjectiveConnected) { return 0; } 
                return YObjective.ZoomLimit; 
            }
        }

        public int YZoomHome
        {
            get { return 0; }
        }

        public int YZoomCurPosition
        {
            get
            {
                return m_YZoomCurPosition;
            }
            set
            {
                SetProperty<int>(ref m_YZoomCurPosition, value);
                RaisePropertyChanged(nameof(YZoomCurPosition));
            }
        }

        public int YZoomGoToPosition
        {
            get { return m_YZoomGoToPos; }
            set
            {
                if (value < 0) { SetProperty<int>(ref m_YZoomGoToPos, 0); }
                else if (value > YZoomLimit) { SetProperty<int>(ref m_YZoomGoToPos, YZoomLimit); }
                else { SetProperty<int>(ref m_YZoomGoToPos, value); }
                RaisePropertyChanged(nameof(YZoomGoToPosition));
            }
        }

        public int XFocusLimit
        {
            get 
            {
                if (!XObjectiveConnected) { return 0; }
                return XObjective.FocusLimit; 
            }
        }

        public int XFocusHome
        {
            get { return 0; }
        }

        public int XFocusCurPosition
        {
            get
            {
                return m_XFocusCurPosition;
            }
            set
            {
                SetProperty<int>(ref m_XFocusCurPosition, value);
                RaisePropertyChanged(nameof(XFocusCurPosition));
            }
        }

        public int XFocusGoToPosition
        {
            get { return m_XFocusGoToPos; }
            set
            {
                if (value < 0) { SetProperty<int>(ref m_XFocusGoToPos, 0); }
                else if (value > XFocusLimit) { SetProperty<int>(ref m_XFocusGoToPos, XFocusLimit); }
                else { SetProperty<int>(ref m_XFocusGoToPos, value); }
                RaisePropertyChanged(nameof(XFocusGoToPosition));
            }
        }

        public int YFocusLimit
        {
            get 
            {
                if (!YObjectiveConnected) { return 0; }
                return YObjective.FocusLimit; 
            }
        }

        public int YFocusHome
        {
            get { return 0; }
        }

        public int YFocusCurPosition
        {
            get
            {
                return m_YFocusCurPosition;
            }
            set
            {
                SetProperty<int>(ref m_YFocusCurPosition, value);
                RaisePropertyChanged(nameof(YFocusCurPosition));
            }
        }

        public int YFocusGoToPosition
        {
            get { return m_YFocusGoToPos; }
            set
            {
                if (value < 0) { SetProperty<int>(ref m_YFocusGoToPos, 0); }
                else if (value > YFocusLimit) { SetProperty<int>(ref m_YFocusGoToPos, YFocusLimit); }
                else { SetProperty<int>(ref m_YFocusGoToPos, value); }
                RaisePropertyChanged(nameof(YFocusGoToPosition));
            }
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
        public void GoToPosition( int position, Axis axis, Function function)
        {
            if(axis == Axis.XAxis && function == Function.Zoom && XObjectiveConnected) 
            {
                if(position < XZoomHome || position > XZoomLimit) { return; }
                XObjective.ZoomTargetPos = position;
            }
            else if (axis == Axis.YAxis && function == Function.Zoom && YObjectiveConnected) 
            {
                if (position < YZoomHome || position > YZoomLimit) { return; }
                YObjective.ZoomTargetPos = position;
            }
            else if (axis == Axis.XAxis && function == Function.Focus && XObjectiveConnected) 
            {
                if (position < XFocusHome || position > XFocusLimit) { return; }
                XObjective.FocusTargetPos = position;
            }
            else if(axis == Axis.YAxis && function == Function.Focus && YObjectiveConnected) 
            {
                if (position < YFocusHome || position > YFocusLimit) { return; }
                YObjective.FocusTargetPos = position;
            }
        }

        private void GetSettingsFromObjectives()
        {
            while (!isDisposing)
            {
                Thread.Sleep(500);
            
                if (XObjectiveConnected)
                {
                    XZoomCurPosition = XObjective.ZoomCurrentPos;
                    XFocusCurPosition = XObjective.FocusCurrentPos;
                }
                else
                {
                    XZoomCurPosition = 0;
                    XFocusCurPosition = 0;
                }

                if(YObjectiveConnected)
                {
                    YZoomCurPosition = YObjective.ZoomCurrentPos;            
                    YFocusCurPosition = YObjective.FocusCurrentPos;
                }
                else
                {
                    YZoomCurPosition = 0;
                    YFocusCurPosition = 0;
                }
                SendObjectiveSettings();
            }
            Thread.Sleep(500);
        }
       
        private void SetUPObjective(Axis axis)
        {
            ConcurrentDictionary<string, string> properties = CryoviewTools.GetAppProperties();
            string Location = "";
            if (properties.ContainsKey("Location")) { Location = properties["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.
            
            if(axis == Axis.XAxis) { Location += "X"; }
            else { Location += "Y"; }

            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();
            m_db.RetrieveSettings(Location, "Navitar", configParams);
            CryoviewTools.LogMessage(LLE.Util.LogLevel.Info, "Settings retrieved for " + axis + " objective");

            foreach (KeyValuePair<string, string> kvp in configParams)
            {
                CryoviewTools.LogMessage(LLE.Util.LogLevel.Info, kvp.Key.ToString() + " / " + kvp.Value.ToString());
            }

            string serialNumber;
            configParams.TryGetValue("SerialNumber", out serialNumber);
            string model;
            configParams.TryGetValue("Model", out model);

            if (model == "Virtual")
            {
                CryoviewTools.LogMessage(LogLevel.Info, "Creating virtual objective");
                if(axis == Axis.XAxis) { XObjective = new ObjectiveVirtualModel(); }
                else { YObjective = new ObjectiveVirtualModel(); }
                CryoviewTools.LogMessage(LogLevel.Info, "Virtual objective created");
            }
            else
            {
                CryoviewTools.LogMessage(LogLevel.Info, "Creating Navitar objective");
                if (axis == Axis.XAxis) { XObjective = new ObjectiveNavitarModel(serialNumber); }
                else { YObjective = new ObjectiveNavitarModel(serialNumber); }
                CryoviewTools.LogMessage(LogLevel.Info, "Navitar objective created");

            }
        }

        private void SendObjectiveSettings()
        {
            ConcurrentDictionary<string, string> settings = new ConcurrentDictionary<string, string>();
            string Name = "Ch1Zoom"; string Value = XZoomCurPosition.ToString();
            settings.AddOrUpdate(Name, Value, (k, v) => Value);
            Name = "Ch1Focus"; Value = XFocusCurPosition.ToString();
            settings.AddOrUpdate(Name, Value, (k, v) => Value);
            Name = "Ch2Zoom";  Value = YZoomCurPosition.ToString();
            settings.AddOrUpdate(Name, Value, (k, v) => Value);
            Name = "Ch2Focus"; Value = YFocusCurPosition.ToString();
            settings.AddOrUpdate(Name, Value, (k, v) => Value);

            DataReportableSettingsEvent.Instance.Publish(new DataReportableSettings
            {
                ReportableSettings = settings
            });
        }

        private void PushMsgRecieved(object state)
        {
            SendObjectiveSettings();
        }
        private void SendFocusLimits(FocusSettingLimitsMessage msg)
        {
            if (msg.IsRequest)
            {
                FocusSettingLimitsMessageEvent.Instance.Publish(new FocusSettingLimitsMessage
                {
                    IsRequest = false,
                    focusLimit1 = XFocusLimit,
                    focusLimit2 = YFocusLimit
                });
            }
        }
        private void FocusSettingMessageRecieved(FocusSettingMessage msg)
        {
            if(msg.ViewAxis == "OpticalView1")
            {
                GoToPosition(msg.focus, Axis.XAxis, Function.Focus);
            }
            else { GoToPosition(msg.focus, Axis.YAxis, Function.Focus); }
        }
        private void OnExceptionRaised(string msg)
        {
            if (ExceptionRaised != null)    // check for subscribers
            {
                ExceptionRaised(msg);   // pass it on (to the view)
            }
        }
        #endregion utility functions

        #region event sinks
        private void XObjective_ExceptionRaised(string msg)
        {
            XObjectiveConnected = false;
            OnExceptionRaised(msg);
        }

        private void YObjective_ExceptionRaised(string msg)
        {
            YObjectiveConnected = false;
            OnExceptionRaised(msg);
        }
        #endregion event sinks
    }
}
