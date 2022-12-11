using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Concurrent;    // ConcurrentDictionary<T, T>
using System.Collections.ObjectModel;
//using System.Data.Odbc;     // read palettes in from file; reqs ref to System.Data
using System.IO;
using System.Reflection;
using System.Windows;   // Visibility
using System.Windows.Media;       // PixelFormats, Color
using System.Windows.Media.Imaging; // BitmapPalette; reqs ref to WindowsBase, PresentationCore
using System.Windows.Input; // for ICommand; requires ref to PresentationCore
using System.Threading;
using Prism.Commands;
using Prism.Mvvm;
using System.Runtime.CompilerServices;

using Cryoview_ModuleMessages;
using Cryoview_Tools;   // logging
using LLE.Util; // logging

namespace Optical_View_Model
{
    public class OpticalViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars

        object m_ObjectLock = new object();

        private WriteableBitmap m_OpticalImg = null;
        private WriteableBitmap m_LiveOpticalImg = null;
        private ImageDataMessage m_FullImage = null;
        private WriteableBitmap m_backupImg = null;
        int m_imgDefaultWidth = 20; // pixels
        int m_imgDefaultHeight = 20;
        
        private string m_ROISetIndicator = "";
        private int m_FramesPerSecond = 5;
        private int m_Gain = 0;
        private int m_RawGain = 0;
        private Single m_Exposure = 0.0f;
        
        private string ID = "";
        private string TwinID = "";
        private string CameraID = "";

        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose

        /// <summary>
        /// For creation of vars, etc.
        /// Any network or hardware communication is handled in the Initialization() method.
        /// </summary>
        public OpticalViewModel(string id, string twinID)
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Creating view model for " + ID);
            ID = id;
            TwinID = twinID;
            if(ID == "OpticalView1" || ID == "OpticalView2") { CameraID = "X"; }
            else { CameraID = "Y"; }
            /* we need a startup img of something else, the image control in the view has size (0,0). */
            WriteableBitmap bmp = new WriteableBitmap(500, 500, 96, 96, PixelFormats.Gray8, null);
            m_OpticalImg = bmp;
            m_LiveOpticalImg = bmp;
            RaisePropertyChanged(nameof(OpticalImg));
            RaisePropertyChanged(nameof(LiveOpticalImg));
            ROISetIndicator = "Not Set";
            LiveViewMode = true;
            UpdateToLatestImg = false;
            MinGain = 0;
            MaxGain = 100;
            ROIRect = new Int32Rect();
            ROIAdjusted = false;
            ROISet = false;
            CryoviewTools.LogMessage(LogLevel.Debug7, ID + " model created");
        }
        /// <summary>
        /// Got here by user code.
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Disposing view model for " + ID);
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
                ImageViewMessageEvent.Instance.Unsubscribe(ChangeOpticalViewSettings);
                ImageViewROIMessageEvent.Instance.Unsubscribe(ChangeROISettings);
                ImageDataMessageEvent.Instance.Unsubscribe(ImageDataRecieved);
                CameraSettingsMessageEvent.Instance.Unsubscribe(ChangeViewSettingsFromCamera);
            }
            // unmanaged resources here
            {
            }
        }

        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        /// <summary>
        /// </summary>
        public void Initialize()
        {
            //create an initial image
            WriteableBitmap bmp = new WriteableBitmap(m_imgDefaultWidth, m_imgDefaultHeight, 96, 96, PixelFormats.Rgb24, null);
            System.Windows.Int32Rect rect = new System.Windows.Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight);
            // msdn says width is stride in pixels, but that does not seem to be true.
            // Int32Rect, System.Array, stride in pixels, input buffer offset
            int pixel_stride = 3;
            Byte[] palettizedImage = new byte[bmp.PixelHeight * bmp.PixelWidth * pixel_stride];
            SetInitialImage(m_imgDefaultWidth, m_imgDefaultHeight, ID, pixel_stride, ref palettizedImage);
            bmp.WritePixels(rect, palettizedImage, bmp.PixelWidth * pixel_stride, 0);
            bmp.Freeze();   // Freeze allows us to update the UI while not being on the UI thread.
            m_OpticalImg = bmp;
            m_LiveOpticalImg = bmp;
            m_backupImg = bmp;
            RaisePropertyChanged(nameof(OpticalImg));
            RaisePropertyChanged(nameof(LiveOpticalImg));
            RaisePropertyChanged(nameof(FramesPerSecond));
            m_Gain = 0;
            RaisePropertyChanged(nameof(Gain));

            //subscribe to messages
            ImageViewMessageEvent.Instance.Subscribe(ChangeOpticalViewSettings); //changes to settings from twin view
            ImageViewROIMessageEvent.Instance.Subscribe(ChangeROISettings); //changes to ROI from twin view
            ImageDataMessageEvent.Instance.Subscribe(ImageDataRecieved); //Image data from the camera
            CameraSettingsMessageEvent.Instance.Subscribe(ChangeViewSettingsFromCamera); //Recieve initial settings from the camera
            ViewReadyMessageEvent.Instance.Publish(new ViewReadyMessage()
            {
                ID = this.ID
            }); //Lets the camera know this view is ready to recive messages
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region properties
        #endregion properties

        #region bindable properties
        public Single Exposure
        {
            get { return m_Exposure; }
            set
            {
                //time in microseconds
                if(value < 0.0f) 
                {
                    SetProperty<Single>(ref m_Exposure, 0.0f);
                }
                else if(value > 10000.0f)
                {
                    SetProperty<Single>(ref m_Exposure, 10000.0f);
                }
                else { SetProperty<Single>(ref m_Exposure, value); }
                RaisePropertyChanged(nameof(Exposure));
                UpdateOpticalViewSettings();
                CryoviewTools.LogMessage(LogLevel.Debug7, ID + " information changed: /n Exposure: " + Exposure.ToString());
            }
        }
        public string ROISetIndicator
        {
            get { return m_ROISetIndicator; }
            set
            {
                SetProperty<string>(ref m_ROISetIndicator, value);
                RaisePropertyChanged(nameof(ROISetIndicator));
                CryoviewTools.LogMessage(LogLevel.Debug7, ID + " RIO is now " + ROISetIndicator);
            }
        }
        public int FramesPerSecond
        {
            get { return m_FramesPerSecond; }
            set 
            { 
                if(value > 15) { SetProperty<int>(ref m_FramesPerSecond, 15); }
                else if(value < 1) { SetProperty<int>(ref m_FramesPerSecond, 1); }
                else { SetProperty<int>(ref m_FramesPerSecond, value); }
                RaisePropertyChanged(nameof(FramesPerSecond));
                UpdateOpticalViewSettings();
                CryoviewTools.LogMessage(LogLevel.Debug7, ID + " information changed: /n Frame Rate: " + FramesPerSecond.ToString());
            }
        }
        public int MaxGain { get; set; }
        public int MinGain { get; set; }
        public int Gain
        {
            get { return m_Gain; }
            set
            {
                if(value < 0) { SetProperty<int>(ref m_Gain, 0); }
                else if(value > 100) { SetProperty<int>(ref m_Gain, 100); }
                else { SetProperty<int>(ref m_Gain, value); }
                RaisePropertyChanged(nameof(Gain));
                CalculateRawGain();
                UpdateOpticalViewSettings();
                CryoviewTools.LogMessage(LogLevel.Debug7, ID + " information changed: /n Gain: " + Gain.ToString() + "/nRaw Gain: " + m_RawGain.ToString());
            }
        }
        public bool LiveViewMode { get; set; }
        public bool UpdateToLatestImg { get; set; }
        public bool ROISet { get; set; }
        public bool ROIAdjusted { get; set; }
        public Int32Rect ROIRect { get; set; }

        /// <summary>
        /// The image from the camera
        /// </summary>
        public BitmapSource OpticalImg
        {
            get
            {
                return m_OpticalImg;
            }
        }
        public BitmapSource LiveOpticalImg
        {
            get
            {
                return m_LiveOpticalImg;
            }
        }

        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code

        private void CalculateRawGain()
        {
            if(Gain == 0) { m_RawGain = MinGain; }
            else if(Gain == 100) { m_RawGain = MaxGain; }
            else
            {
                int whole = MaxGain - MinGain;
                Single part = Gain / 100.0f; part = part * whole;
                m_RawGain = (int)part + MinGain;
            }
        }

        /// <summary>
        /// converts the image for display
        /// </summary>
        /// <param name="image">Raw data from camera. This is copied to memory. The raw data is not modified. The copy is modified.</param>
        /// <returns></returns>
        private WriteableBitmap AdjustImageForDisplay(Array image, int Width, int Height, int bitsPerPixel)
        {
            int width = Width;
            int height = Height;
            Int32Rect rect;
            WriteableBitmap bmpForUI = null;
            if (image == null)
            { //handle no image return null? return default image or starting image? 
                return m_backupImg;
            }

            if (ROISet && !ROIAdjusted)//first time only
            {
                //need to adjust the width and height of the ROI to scale to the full image
                int ROIWidth = (width * ROIRect.Width) / 500;
                int ROIHeight = (height * ROIRect.Height) / 500;
                int ROIX = (width * ROIRect.X) / 500;
                int ROIY = (height * ROIRect.Y) / 500;
                ROIRect = new Int32Rect(ROIX, ROIY, ROIWidth, ROIHeight);
                ROIAdjusted = true;
                UpdateROISettings();
            }

            switch (bitsPerPixel)
            {
                case 12:
                    ushort[] workingImage = new ushort[width * height];
                    Array.Copy(image, workingImage, width * height);
                    for (int i = 0; i < workingImage.Length; i++) { ushort tempVal = (ushort)(workingImage[i]); workingImage[i] = (ushort)(tempVal << 4); }

                    if (ROISet)
                    {
                        ushort[] ROIImage = new ushort[ROIRect.Width * ROIRect.Height];
                        long StartIndex = width * (ROIRect.Y - 1) + ROIRect.X;
                        long IndexesBetween = width - ROIRect.Width;
                        long i = StartIndex;
                        int count = 0;
                        int rounds = 0;
                        while (rounds < ROIRect.Height)
                        {
                            for (int j = 0; j < ROIRect.Width; j++)
                            {
                                ROIImage.SetValue(workingImage.GetValue(i), count);
                                i++; count++;
                            }
                            for (int j = 0; j < IndexesBetween; j++)
                            {
                                i++;
                                //skip, do not add to array
                            }
                            rounds++;
                        }
                        workingImage = ROIImage;
                        width = ROIRect.Width;
                        height = ROIRect.Height;
                    }

                    rect = new Int32Rect(0, 0, width, height);
                    bmpForUI = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray16, null); // Gray16 allows 16 bits per pixel. (0 - 65535)
                    bmpForUI.WritePixels(rect, workingImage, width * sizeof(ushort), 0);
                    break;
                case 8:

                    byte[] workingImage2 = new byte[width * height];
                    Array.Copy(image, workingImage2, width * height);

                    if (ROISet)
                    {
                        byte[] ROIImage = new byte[ROIRect.Width * ROIRect.Height];
                        long StartIndex = width * (ROIRect.Y - 1) + ROIRect.X;
                        long IndexesBetween = width - ROIRect.Width;
                        long i = StartIndex;
                        int count = 0;
                        int rounds = 0;
                        while (rounds < ROIRect.Height)
                        {
                            for (int j = 0; j < ROIRect.Width; j++)
                            {
                                ROIImage.SetValue(workingImage2.GetValue(i), count);
                                i++; count++;
                            }
                            for (int j = 0; j < IndexesBetween; j++)
                            {
                                i++;
                                //skip, do not add to array
                            }
                            rounds++;
                        }
                        workingImage2 = ROIImage;
                        width = ROIRect.Width;
                        height = ROIRect.Height;
                    }

                    rect = new Int32Rect(0, 0, width, height);
                    bmpForUI = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                    bmpForUI.WritePixels(rect, workingImage2, width, 0);
                    break;
            }

            bmpForUI.Freeze();   // Freeze allows us to update the UI while not being on the UI thread.
            return bmpForUI;

        }
        #endregion algorithm code

        #region hardware code
        #endregion hardware code

        #region utility functions
        public void ChangeROICurrentImage()
        {
            WriteableBitmap bmp = AdjustImageForDisplay(m_FullImage.Image, m_FullImage.Width, m_FullImage.Height, m_FullImage.BitsPerPixel);
            if (bmp != null)
            {
                m_OpticalImg = bmp;
                RaisePropertyChanged(nameof(OpticalImg));
            }
        }

        /// <summary>
        /// Settings changed in another image view
        /// </summary>
        private void ChangeOpticalViewSettings(object state)
        {
            ImageViewMessage msg = (ImageViewMessage)(state);
            if (msg.ID != TwinID) { return; }
            if (msg.Exposure != Exposure) { Exposure = msg.Exposure; }
            if (msg.Gain != Gain) { Gain = msg.Gain; }
            if (msg.FrameRate != FramesPerSecond) { FramesPerSecond = msg.FrameRate; }
        }

        /// <summary>
        /// Update settings for twin view
        /// </summary>
        private void UpdateOpticalViewSettings()
        {
            ImageViewMessageEvent.Instance.Publish(new ImageViewMessage()
            {
                ID = this.ID,
                ImagesToAverage = 1,
                Exposure = this.Exposure,
                RawGain = m_RawGain,
                Gain = this.Gain,
                FrameRate = FramesPerSecond 
            });
        }

        /// <summary>
        /// ROI changed in another image view
        /// </summary>
        private void ChangeROISettings(object state)
        {
            ImageViewROIMessage msg = (ImageViewROIMessage)(state);
            if (msg.ID != TwinID) { return; }
            if (msg.ROISetIndicator != ROISetIndicator) { ROISetIndicator = msg.ROISetIndicator; }
            if (msg.ROIRect != ROIRect) { ROIRect = msg.ROIRect; }
            if (msg.ROIAdjusted != ROIAdjusted) { ROIAdjusted = msg.ROIAdjusted; }
            if (msg.ROISet != ROISet) { ROISet = msg.ROISet; }
            ChangeROICurrentImage();
        }

        /// <summary>
        /// Update ROI for twin view
        /// </summary>
        private void UpdateROISettings()
        {
            ImageViewROIMessageEvent.Instance.Publish(new ImageViewROIMessage()
            {
                ID = this.ID,
                ROISetIndicator = this.ROISetIndicator,
                ROISet = this.ROISet,
                ROIRect = this.ROIRect,
                ROIAdjusted = this.ROIAdjusted
            });
        }

        /// <summary>
        /// Settings recieved from camera
        /// </summary>
        private void ChangeViewSettingsFromCamera(object state)
        {
            CameraSettingsMessage msg = (CameraSettingsMessage)(state);
            if (msg.ID.Contains(CameraID))
            {
                m_Exposure = msg.Exposure;
                RaisePropertyChanged(nameof(Exposure));
                MaxGain = msg.MaxGain;
                MinGain = msg.MinGain;
                m_RawGain = MinGain;
            }
        }

        /// <summary>
        /// Image recieved from camera
        /// </summary>
        private void ImageDataRecieved (object state)
        {
            ImageDataMessage msg = (ImageDataMessage)(state);
            if (msg.Id.Contains(CameraID) && msg.Id != "XRay")
            {
                CryoviewTools.LogMessage(LogLevel.Debug6, ID + ": image recieved from camera at " + msg.Timestamp);
                WriteableBitmap bmp = AdjustImageForDisplay(msg.Image, msg.Width, msg.Height, msg.BitsPerPixel);
                m_LiveOpticalImg = bmp;
                RaisePropertyChanged(nameof(LiveOpticalImg));
                if(LiveViewMode || UpdateToLatestImg)
                {
                    m_FullImage = msg;
                    m_OpticalImg = bmp;
                    RaisePropertyChanged(nameof(OpticalImg));
                    if (UpdateToLatestImg) { UpdateToLatestImg = false; }
                }                
            }
        }

        /// <summary>
        /// Sets the initial image so we have something to display until the camera is connected.
        /// </summary>
        /// <param name="ViewID">The ID for the view so each view can get a different color initial image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixel_stride"> Offset from one pixel's data to the next pixel's data.</param>
        /// <param name="palettizedImage">Data manipulated to show in a colorized pallette.</param>
        private void SetInitialImage(int width, int height, string ViewID, int pixel_stride, ref byte[] palettizedImage)
        {
            int index = 0;
            byte R = 0; byte G = 0; byte B = 0;
            switch (ViewID)
            {
                case "XRayView1":
                case "XRayView2":
                    R = 252;
                    G = 128;
                    B = 252;
                    break;
                case "OpticalView1":
                case "OpticalView2":
                    R = 50;
                    G = 175;
                    B = 217;
                    break;
                case "OpticalView3":
                case "OpticalView4":
                    R = 156;
                    G = 99;
                    B = 214;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < width * height; i++)
            {
                index = i * pixel_stride;
                palettizedImage[index] = R;
                palettizedImage[index + 1] = G;
                palettizedImage[index + 2] = B;
            }
        }

        /// <summary>
        /// Aspect-oriented programming
        /// Break down program logic into distinct parts, aka concerns, one of which is handling exceptions.
        /// </summary>
        /// <param name="ex"></param>
        private void HandleExceptions(Exception ex)
        {
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks

    }
}
