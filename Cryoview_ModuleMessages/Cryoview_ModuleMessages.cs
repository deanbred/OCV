using System;
using System.Windows;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Prism.Events;

using MCM_Interface;

namespace Cryoview_ModuleMessages
{
    /// <summary>
    /// Instructs each Image view that the others had a change in data
    /// </summary>
    public class ImageViewMessage: PubSubEvent<ImageViewMessage>
    {
        public string ID { get; set; }
        public int ImagesToAverage { get; set; }
        public Single Exposure { get; set; }
        public int IntegrationTime { get; set; }
        public int DigitalGain { get; set; }
        public int Gain { get; set; }
        public int RawGain { get; set; }
        public int FrameRate { get; set; }
    }

    public class ImageViewMessageEvent : PubSubEvent<ImageViewMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly ImageViewMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static ImageViewMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<ImageViewMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static ImageViewMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Instructs each Image view that the others had a change in ROI
    /// </summary>
    public class ImageViewROIMessage : PubSubEvent<ImageViewROIMessage>
    {
        public string ID { get; set; }
        public string ROISetIndicator { get; set; }
        public bool ROISet { get; set; }
        public bool ROIAdjusted { get; set; }
        public Int32Rect ROIRect { get; set; }
    }

    public class ImageViewROIMessageEvent : PubSubEvent<ImageViewROIMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly ImageViewROIMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static ImageViewROIMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<ImageViewROIMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static ImageViewROIMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Sends camrea image data to image views
    /// </summary>
    public class ImageDataMessage : PubSubEvent<ImageDataMessage>
    {
        public Array Image { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int HorizontalBinning { get; set; }
        public int VerticalBinning { get; set; }
        public int BitsPerPixel { get; set; }
        public int Stride { get; set; }
        public string Timestamp { get; set; }
        public string Id { get; set; }
        public int Gain { get; set; }
        public Single Exposure { get; set; }
    }

    public class ImageDataMessageEvent : PubSubEvent<ImageDataMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly ImageDataMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static ImageDataMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<ImageDataMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static ImageDataMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Sends camera settings to image views
    /// </summary>
    public class CameraSettingsMessage : PubSubEvent<CameraSettingsMessage>
    {
        public string ID { get; set; }
        public Single Exposure { get; set; }
        public int IntegrationTime { get; set; }
        public int MaxIntegrationTime { get; set; }
        public int MinGain { get; set; }
        public int MaxGain { get; set; }
        public int Gain { get; set; }
        public int MinDigitalGain { get; set; }
        public int MaxDigitalGain { get; set; }
        public int DigitalGain { get; set; }
        public int ImagesToAverage { get; set; }
    }

    public class CameraSettingsMessageEvent : PubSubEvent<CameraSettingsMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly CameraSettingsMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static CameraSettingsMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<CameraSettingsMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static CameraSettingsMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Sends a message to say the view is instantiated and ready
    /// </summary>
    public class ViewReadyMessage : PubSubEvent<ViewReadyMessage>
    {
        public string ID { get; set; }
    }

    public class ViewReadyMessageEvent : PubSubEvent<ViewReadyMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly ViewReadyMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static ViewReadyMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<ViewReadyMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static ViewReadyMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Sends a message to send a command to listening objects
    /// </summary>
    public class CommandMessage : PubSubEvent<CommandMessage>
    {
        public string ID { get; set; }
        public string Command { get; set; }
    }

    public class CommandMessageEvent : PubSubEvent<CommandMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly CommandMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static CommandMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<CommandMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static CommandMessageEvent Instance { get { return m_push; } }
    }

    public enum MessageType
    {
        Error,
        Status
    }
    /// <summary>
    /// Sends a message to the main view with a status or error message
    /// </summary>
    public class ErrorOrStatusMessage : PubSubEvent<ErrorOrStatusMessage>
    {
        public MessageType messageType { get; set; } //status or error
        public string Message { get; set; } //the message to be displayed on the GUI
    }

    public class ErrorOrStatusMessageEvent : PubSubEvent<ErrorOrStatusMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly ErrorOrStatusMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static ErrorOrStatusMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<ErrorOrStatusMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static ErrorOrStatusMessageEvent Instance { get { return m_push; } }
    }
    public class DataReportableSettings : PubSubEvent<DataReportableSettings>
    {
        public ConcurrentDictionary<string, string> ReportableSettings { get; set; }
    }  

    public class DataReportableSettingsEvent : PubSubEvent<DataReportableSettings>
    {
        private static readonly EventAggregator _eventAggregator = null;
        private static readonly DataReportableSettingsEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static DataReportableSettingsEvent()
        {
            _eventAggregator = new EventAggregator();
            m_push = _eventAggregator.GetEvent<DataReportableSettingsEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static DataReportableSettingsEvent Instance { get { return m_push; } }

    }
    /// <summary>
    /// Very simple msg to pass along to other modules that something is ready to receive data.
    /// Recipients of this msg are to publish whatever data they have available.
    /// </summary>
    public class PushDataMessage : PubSubEvent<PushDataMessage>
    {
    }

    public class PushDataMessageEvent : PubSubEvent<PushDataMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly PushDataMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static PushDataMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<PushDataMessageEvent>();
        } 

        /// <summary>
        /// 
        /// </summary>
        public static PushDataMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Sends the MCM object out to classes that need access.
    /// </summary>
    public class MCMObjectMessage : PubSubEvent<MCMObjectMessage>
    {
        public MCMInterface MCM { get; set; }
    }

    public class MCMObjectMessageEvent : PubSubEvent<MCMObjectMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly MCMObjectMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static MCMObjectMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<MCMObjectMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static MCMObjectMessageEvent Instance { get { return m_push; } }
    }

    public enum MCMTarget
    {
        LS,
        CFE
    }
    public enum MCMTempDirection
    {
        PLUS,
        MINUS,
        SET
    }
    /// <summary>
    /// Sends a message to send a command to listening objects
    /// </summary>
    public class MCMTemperatureMessage : PubSubEvent<MCMTemperatureMessage>
    {
        public MCMTarget Target { get; set; }
        public MCMTempDirection Direction { get; set; }
        public Single Temperature { get; set; }
    }

    public class MCMTemperatureMessageEvent : PubSubEvent<MCMTemperatureMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly MCMTemperatureMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static MCMTemperatureMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<MCMTemperatureMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static MCMTemperatureMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Tells the Pop-out view when the ROI for the automated focus routine is set or removed
    /// </summary>
    public class FocusROIMessage : PubSubEvent<FocusROIMessage>
    {
        public string ViewAxis { get; set; }
        public bool ROIisSet { get; set; } //true if set, false if removed
    }

    public class FocusROIMessageEvent : PubSubEvent<FocusROIMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly FocusROIMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static FocusROIMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<FocusROIMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static FocusROIMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Automated Focus routine request for marker location and pop-out view response with the location of the marker.
    /// </summary>
    public class FocusROIMarkerMessage : PubSubEvent<FocusROIMarkerMessage>
    {
        public string ViewAxis { get; set; }
        public bool IsRequest { get; set; } //true if the focus model is requesting the location from the view, false if response from pop-out
        public Point MarkerPoint { get; set; }
    }

    public class FocusROIMarkerMessageEvent : PubSubEvent<FocusROIMarkerMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly FocusROIMarkerMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static FocusROIMarkerMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<FocusROIMarkerMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static FocusROIMarkerMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Automated Focus routine will indicate when the marker can be moved.
    /// </summary>
    public class FocusROIMarkerMovableMessage : PubSubEvent<FocusROIMarkerMovableMessage>
    {
        public string ViewAxis { get; set; }
        public bool CanMoveMarker { get; set; } 
    }

    public class FocusROIMarkerMovableMessageEvent : PubSubEvent<FocusROIMarkerMovableMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly FocusROIMarkerMovableMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static FocusROIMarkerMovableMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<FocusROIMarkerMovableMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static FocusROIMarkerMovableMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Enables or disables user control over the navitar objective
    /// </summary>
    public class EnableUserObjectiveControlMessage : PubSubEvent<EnableUserObjectiveControlMessage>
    {
        public string ViewAxis { get; set; }
        public bool Enabled { get; set; } 
    }

    public class EnableUserObjectiveControlMessageEvent : PubSubEvent<EnableUserObjectiveControlMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly EnableUserObjectiveControlMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static EnableUserObjectiveControlMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<EnableUserObjectiveControlMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static EnableUserObjectiveControlMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Send the limits of the focus motors.
    /// </summary>
    public class FocusSettingLimitsMessage : PubSubEvent<FocusSettingLimitsMessage>
    {
        public bool IsRequest { get; set; } //is this the view model requesting the information.
        public int focusLimit1 { get; set; }
        public int focusLimit2 { get; set; }
    }

    public class FocusSettingLimitsMessageEvent : PubSubEvent<FocusSettingLimitsMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly FocusSettingLimitsMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static FocusSettingLimitsMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<FocusSettingLimitsMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static FocusSettingLimitsMessageEvent Instance { get { return m_push; } }
    }

    /// <summary>
    /// Tells the objective view model to set the focus on an axis.
    /// </summary>
    public class FocusSettingMessage : PubSubEvent<FocusSettingMessage>
    {
        public string ViewAxis { get; set; }
        public int focus { get; set; }
    }

    public class FocusSettingMessageEvent : PubSubEvent<FocusSettingMessage>
    {
        private static readonly EventAggregator m_eventAggregator = null;
        private static readonly FocusSettingMessageEvent m_push = null;

        /// <summary>
        /// 
        /// </summary>
        static FocusSettingMessageEvent()
        {
            m_eventAggregator = new EventAggregator();
            m_push = m_eventAggregator.GetEvent<FocusSettingMessageEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static FocusSettingMessageEvent Instance { get { return m_push; } }
    }
}

