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
using Illuminator_Interface;
using Illuminator_Gardasoft_Model;
using Illuminator_Virtual_Model;
using DB_Model;

namespace Illuminator_Control_ViewModel
{
    public class IlluminatorControlViewModel : BindableBase, IDisposable
    {

        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private DBModel m_db = null;
        private IlluminatorInterface illuminator = null;
        public bool m_IlluninatorConnected = false;

        private Single m_CH1DiodeBrightness = 0.0f;
        private Single m_CH2DiodeBrightness = 0.0f;
        private Single m_CH1DidodeBrightnessCurrent = 0.0f;
        private Single m_CH2DidodeBrightnessCurrent = 0.0f;
        private Single m_CH1DiodeBrightnessLow = 0.0f;
        private Single m_CH2DiodeBrightnessLow = 0.0f;
        private Single m_CH1DiodeBrightnessHigh = 0.0f;
        private Single m_CH2DiodeBrightnessHigh = 0.0f;

        private Single m_CH1PulseDuration = 0.0f;
        private Single m_CH2PulseDuration = 0.0f;
        private Single m_CH1PulseDurationCurrent = 0.0f;
        private Single m_CH2PulseDurationCurrent = 0.0f;
        private Single m_CH1PulseDurationLow = 0.0f;
        private Single m_CH2PulseDurationLow = 0.0f;
        private Single m_CH1PulseDurationHigh = 0.0f;
        private Single m_CH2PulseDurationHigh = 0.0f;

        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public IlluminatorControlViewModel()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Creating view model for Illuminator");
            m_db = new DBModel();
            CommandMessageEvent.Instance.Subscribe(CommandRecieved);
            PushDataMessageEvent.Instance.Subscribe(PushMsgRecieved);
            CryoviewTools.LogMessage(LogLevel.Debug7, "Illuminator view model created");
        }

        /// <summary>
        /// Got here by user code.
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Disposing view model for Illuminator");
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
                m_db.Dispose();
                CommandMessageEvent.Instance.Unsubscribe(CommandRecieved);
                PushDataMessageEvent.Instance.Unsubscribe(PushMsgRecieved);
                illuminator.ExceptionRaised -= Illuminator_ExceptionRaised;
                illuminator.Dispose();
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
            SetUpIlluminator();
            GetSettingsFromIlluminator();
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region properties
        #endregion properties

        #region bindable properties
        public Single CH1DiodeBrightness
        {
            get
            {
                return m_CH1DiodeBrightness;
            }
            set
            {
                if (value < 0.0f) { SetProperty<Single>(ref m_CH1DiodeBrightness, 0.0f); }
                else if (value > 100.0f) { SetProperty<Single>(ref m_CH1DiodeBrightness, 100.0f); }
                else { SetProperty<Single>(ref m_CH1DiodeBrightness, value); }
                RaisePropertyChanged(nameof(CH1DiodeBrightness));
            }
        }
        public Single CH2DiodeBrightness
        {
            get
            {
                return m_CH2DiodeBrightness;
            }
            set
            {
                if (value < 0.0f) { SetProperty<Single>(ref m_CH2DiodeBrightness, 0.0f); }
                else if (value > 100.0f) { SetProperty<Single>(ref m_CH2DiodeBrightness, 100.0f); }
                else { SetProperty<Single>(ref m_CH2DiodeBrightness, value); }
                RaisePropertyChanged(nameof(CH2DiodeBrightness));
            }
        }
        public Single CH1DiodeBrightnessCurrent
        {
            get
            {
                return m_CH1DidodeBrightnessCurrent;
            }
            set
            {
                SetProperty<Single>(ref m_CH1DidodeBrightnessCurrent, value);
                RaisePropertyChanged(nameof(CH1DiodeBrightnessCurrent));
            }
        }
        public Single CH2DiodeBrightnessCurrent
        {
            get
            {
                return m_CH2DidodeBrightnessCurrent;
            }
            set
            {
                SetProperty<Single>(ref m_CH2DidodeBrightnessCurrent, value);
                RaisePropertyChanged(nameof(CH2DiodeBrightnessCurrent));
            }
        }
        public Single CH1DiodeBrightnessLow
        {
            get
            {
                return m_CH1DiodeBrightnessLow;
            }
            set
            {
                if (value < 0.0f) { SetProperty<Single>(ref m_CH1DiodeBrightnessLow, 0.0f); }
                else if (value > 100.0f) { SetProperty<Single>(ref m_CH1DiodeBrightnessLow, 100.0f); }
                else { SetProperty<Single>(ref m_CH1DiodeBrightnessLow, value); }
                RaisePropertyChanged(nameof(CH1DiodeBrightnessLow));
            }
        }
        public Single CH2DiodeBrightnessLow
        {
            get
            {
                return m_CH2DiodeBrightnessLow;
            }
            set
            {
                if (value < 0.0f) { SetProperty<Single>(ref m_CH2DiodeBrightnessLow, 0.0f); }
                else if (value > 100.0f) { SetProperty<Single>(ref m_CH2DiodeBrightnessLow, 100.0f); }
                else { SetProperty<Single>(ref m_CH2DiodeBrightnessLow, value); }
                RaisePropertyChanged(nameof(CH2DiodeBrightnessLow));
            }
        }
        public Single CH1DiodeBrightnessHigh
        {
            get
            {
                return m_CH1DiodeBrightnessHigh;
            }
            set
            {
                if (value < 0.0f) { SetProperty<Single>(ref m_CH1DiodeBrightnessHigh, 0.0f); }
                else if (value > 100.0f) { SetProperty<Single>(ref m_CH1DiodeBrightnessHigh, 100.0f); }
                else { SetProperty<Single>(ref m_CH1DiodeBrightnessHigh, value); }
                RaisePropertyChanged(nameof(CH1DiodeBrightnessHigh));
            }
        }
        public Single CH2DiodeBrightnessHigh
        {
            get
            {
                return m_CH2DiodeBrightnessHigh;
            }
            set
            {
                if (value < 0.0f) { SetProperty<Single>(ref m_CH2DiodeBrightnessHigh, 0.0f); }
                else if (value > 100.0f) { SetProperty<Single>(ref m_CH2DiodeBrightnessHigh, 100.0f); }
                else { SetProperty<Single>(ref m_CH2DiodeBrightnessHigh, value); }
                RaisePropertyChanged(nameof(CH2DiodeBrightnessHigh));
            }
        }
        public Single CH1PulseDuration
        {
            get
            {
                return m_CH1PulseDuration;
            }
            set
            {
                if (value < 0) { SetProperty<Single>(ref m_CH1PulseDuration, 0.0f); }
                else if (value > 100) { SetProperty<Single>(ref m_CH1PulseDuration, 10.0f); }
                else { SetProperty<Single>(ref m_CH1PulseDuration, value); }
                RaisePropertyChanged(nameof(CH1PulseDuration));
            }
        }
        public Single CH2PulseDuration
        {
            get
            {
                return m_CH2PulseDuration;
            }
            set
            {
                if (value < 0) { SetProperty<Single>(ref m_CH2PulseDuration, 0.0f); }
                else if (value > 100) { SetProperty<Single>(ref m_CH2PulseDuration, 10.0f); }
                else { SetProperty<Single>(ref m_CH2PulseDuration, value); }
                RaisePropertyChanged(nameof(CH2PulseDuration));
            }
        }
        public Single CH1PulseDurationCurrent
        {
            get
            {
                return m_CH1PulseDurationCurrent;
            }
            set
            {
                SetProperty<Single>(ref m_CH1PulseDurationCurrent, value);
                RaisePropertyChanged(nameof(CH1PulseDurationCurrent));
            }
        }
        public Single CH2PulseDurationCurrent
        {
            get
            {
                return m_CH2PulseDurationCurrent;
            }
            set
            {
                SetProperty<Single>(ref m_CH2PulseDurationCurrent, value);
                RaisePropertyChanged(nameof(CH2PulseDurationCurrent));
            }
        }
        public Single CH1PulseDurationLow
        {
            get
            {
                return m_CH1PulseDurationLow;
            }
            set
            {
                if (value < 0) { SetProperty<Single>(ref m_CH1PulseDurationLow, 0.0f); }
                else if (value > 100) { SetProperty<Single>(ref m_CH1PulseDurationLow, 10.0f); }
                else { SetProperty<Single>(ref m_CH1PulseDurationLow, value); }
                RaisePropertyChanged(nameof(CH1PulseDurationLow));
            }
        }
        public Single CH2PulseDurationLow
        {
            get
            {
                return m_CH2PulseDurationLow;
            }
            set
            {
                if (value < 0) { SetProperty<Single>(ref m_CH2PulseDurationLow, 0.0f); }
                else if (value > 100) { SetProperty<Single>(ref m_CH2PulseDurationLow, 10.0f); }
                else { SetProperty<Single>(ref m_CH2PulseDurationLow, value); }
                RaisePropertyChanged(nameof(CH2PulseDurationLow));
            }
        }
        public Single CH1PulseDurationHigh
        {
            get
            {
                return m_CH1PulseDurationHigh;
            }
            set
            {
                if (value < 0) { SetProperty<Single>(ref m_CH1PulseDurationHigh, 0.0f); }
                else if (value > 100) { SetProperty<Single>(ref m_CH1PulseDurationHigh, 10.0f); }
                else { SetProperty<Single>(ref m_CH1PulseDurationHigh, value); }
                RaisePropertyChanged(nameof(CH1PulseDurationHigh));
            }
        }
        public Single CH2PulseDurationHigh
        {
            get
            {
                return m_CH2PulseDurationHigh;
            }
            set
            {
                if (value < 0) { SetProperty<Single>(ref m_CH2PulseDurationHigh, 0.0f); }
                else if (value > 100) { SetProperty<Single>(ref m_CH2PulseDurationHigh, 10.0f); }
                else { SetProperty<Single>(ref m_CH2PulseDurationHigh, value); }
                RaisePropertyChanged(nameof(CH2PulseDurationHigh));
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
        private void SetUpIlluminator()
        {
            CryoviewTools.LogMessage(LLE.Util.LogLevel.Debug7, "Setting up illuminator");
            ConcurrentDictionary<string, string> properties = CryoviewTools.GetAppProperties();
            string Location = "";
            if (properties.ContainsKey("Location")) { Location = properties["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.

            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();
            //retrieve Illuminator settings from database
            m_db.RetrieveSettings(Location, "Illuminator", configParams);
            CryoviewTools.LogMessage(LLE.Util.LogLevel.Info, "Settings retrieved for illuminator");

            foreach (KeyValuePair<string, string> kvp in configParams)
            {
                CryoviewTools.LogMessage(LLE.Util.LogLevel.Info, kvp.Key.ToString() + " / " + kvp.Value.ToString());
            }

            string serialNumber;
            configParams.TryGetValue("SerialNumber", out serialNumber);
            string model;
            configParams.TryGetValue("Model", out model);

            CryoviewTools.LogMessage(LLE.Util.LogLevel.Debug7, "Creating Illuminator");
            if (model == "Gardasoft")
            {
                illuminator = new IlluminatorGardasoftModel(serialNumber);   
            }
            else
            {
                illuminator = new IlluminatorVirtualModel();
            }

            illuminator.ExceptionRaised += Illuminator_ExceptionRaised;
            m_IlluninatorConnected = illuminator.Initialize();
            if(m_IlluninatorConnected) 
            { 
                CryoviewTools.LogMessage(LLE.Util.LogLevel.Debug7, "Illuminator Created");
            }
        }

        private void GetSettingsFromIlluminator()
        {
            if (m_IlluninatorConnected)
            {
                CH1DiodeBrightnessCurrent = illuminator.CH1DiodeBrightness;
                CH2DiodeBrightnessCurrent = illuminator.CH2DiodeBrightness;
                CH1PulseDurationCurrent = illuminator.CH1PulseDurration;
                CH2PulseDurationCurrent = illuminator.CH2PulseDurration;
            }
            else
            {
                CH1DiodeBrightnessCurrent = 0.0f;
                CH2DiodeBrightnessCurrent = 0.0f;
                CH1PulseDurationCurrent = 0.0f;
                CH2PulseDurationCurrent = 0.0f;
            }
            SendIlluminatorSettings();
        }

        public void SetCh1Settings()
        {
            illuminator.CH1DiodeBrightness = CH1DiodeBrightness;
            illuminator.CH1PulseDurration = CH1PulseDuration;
            GetSettingsFromIlluminator();
        }
        
        public void SetCh2Settings()
        {
            illuminator.CH2DiodeBrightness = CH2DiodeBrightness;
            illuminator.CH2PulseDurration = CH2PulseDuration;
            GetSettingsFromIlluminator();
        }
        
        private void SendIlluminatorSettings()
        {
            ConcurrentDictionary<string, string> settings = new ConcurrentDictionary<string, string>();
            string Name = "Ch1Brightness"; string Value = CH1DiodeBrightnessCurrent.ToString();
            settings.AddOrUpdate(Name, Value, (k, v) => Value);
            Name = "Ch2Brightness"; Value = CH2DiodeBrightnessCurrent.ToString();
            settings.AddOrUpdate(Name, Value, (k, v) => Value);

            DataReportableSettingsEvent.Instance.Publish(new DataReportableSettings
            {
                ReportableSettings = settings
            });
        }

        private void PushMsgRecieved(object state)
        {
            SendIlluminatorSettings();
        }
        private void CommandRecieved(object state) 
        {
            CommandMessage msg = (CommandMessage)(state);
            if (msg.ID == "Illuminator" && m_IlluninatorConnected)
            {
                switch (msg.Command)
                {
                    case "SetCH1High":
                        illuminator.CH1DiodeBrightness = CH1DiodeBrightnessHigh;
                        illuminator.CH1PulseDurration = CH1PulseDurationHigh;
                        GetSettingsFromIlluminator();
                        break;
                    case "SetCH1Low":
                        illuminator.CH1DiodeBrightness = CH1DiodeBrightnessLow;
                        illuminator.CH1PulseDurration = CH1PulseDurationLow;
                        GetSettingsFromIlluminator();
                        break;
                    case "SetCH2High":
                        illuminator.CH2DiodeBrightness = CH2DiodeBrightnessHigh;
                        illuminator.CH2PulseDurration = CH2PulseDurationHigh;
                        GetSettingsFromIlluminator();
                        break;
                    case "SetCH2Low":
                        illuminator.CH2DiodeBrightness = CH2DiodeBrightnessLow;
                        illuminator.CH2PulseDurration = CH2PulseDurationLow;
                        GetSettingsFromIlluminator();
                        break;
                }
            }
        }
        #endregion utility functions

        #region event sinks
        private void Illuminator_ExceptionRaised(string msg)
        {
            m_IlluninatorConnected = false;
            if (ExceptionRaised != null)    // check for subscribers
            {
                ExceptionRaised(msg);   // pass it on (to the view)
            }
        }
        #endregion event sinks
    }
}
