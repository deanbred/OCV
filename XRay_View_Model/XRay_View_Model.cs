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


namespace XRay_View_Model
{
     public class XRayViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private bool isDisposing = false;
        private WriteableBitmap m_XRayImg = null;
        private ImageDataMessage m_FullImage = null;
        private int m_ImagesToAverate = 1;
        private Single m_Exposure = 0.0f;
        private string m_ROISetIndicator = "";
        private int m_ImgIntegrationTime = 1;
        private int m_MaxIntegrationTime = 1;
        private int m_Gain = 0;
        private int m_MinGain = 0;
        private int m_MaxGain = 1;
        private int m_DigitalGain = 0;
        private int m_MinDigitalGain = 0;
        private int m_MaxDigitalGain = 1;

        private string ID = "";
        private string TwinID = "";

        int m_imgDefaultWidth = 20; // pixels
        int m_imgDefaultHeight = 20;

        List<BitmapPalette> m_listPalettes = null;
        List<string> m_listPaletteNames = null;
        private String m_paletteSelected = "Default";

        private Thread getImages = null;

        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        /// <summary>
        /// For creation of vars, etc.
        /// Any network or hardware communication is handled in the Initialization() method.
        /// </summary>
        public XRayViewModel(string id, string twinID)
        {
            ID = id;
            TwinID = twinID;
            /* we need a startup img of something else, the image control in the view has size (0,0). */
            WriteableBitmap bmp = new WriteableBitmap(500, 500, 96, 96, PixelFormats.Gray8, null);
            m_XRayImg = bmp;
            RaisePropertyChanged(nameof(XRayImg));
            ROISetIndicator = "Not Set";
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
                isDisposing = true;
                if(getImages != null) { getImages.Join(); }
                ImageViewMessageEvent.Instance.Unsubscribe(ChangeXRayViewSettings);
                ImageViewROIMessageEvent.Instance.Unsubscribe(ChangeROISettings);
                ImageDataMessageEvent.Instance.Unsubscribe(ImageDataMessageRecieved);
                CameraSettingsMessageEvent.Instance.Unsubscribe(CameraSettings);
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
            LoadPalettes();
            WriteableBitmap bmp = new WriteableBitmap(m_imgDefaultWidth, m_imgDefaultHeight, 96, 96, PixelFormats.Rgb24, m_listPalettes[0]);
            System.Windows.Int32Rect rect = new System.Windows.Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight);
            // msdn says width is stride in pixels, but that does not seem to be true.
            // Int32Rect, System.Array, stride in pixels, input buffer offset
            int pixel_stride = 3;
            Byte[] palettizedImage = new byte[bmp.PixelHeight * bmp.PixelWidth * pixel_stride];
            SetInitialImage( m_imgDefaultWidth, m_imgDefaultHeight, ID, pixel_stride, ref palettizedImage);
            bmp.WritePixels(rect, palettizedImage, bmp.PixelWidth * pixel_stride, 0);
            bmp.Freeze();   // Freeze allows us to update the UI while not being on the UI thread.
            m_XRayImg = bmp;
            RaisePropertyChanged(nameof(XRayImg));

            ImageViewMessageEvent.Instance.Subscribe(ChangeXRayViewSettings);
            ImageViewROIMessageEvent.Instance.Subscribe(ChangeROISettings);
            ImageDataMessageEvent.Instance.Subscribe(ImageDataMessageRecieved);
            CameraSettingsMessageEvent.Instance.Subscribe(CameraSettings);

            if(ID == "XRayView2")
            {
                ViewReadyMessageEvent.Instance.Publish(new ViewReadyMessage()
                {
                    ID = this.ID
                }); //Lets the camera know this view is ready to recive messages

                //For testing only! Should be commented out.
                //getImages = new Thread(new ThreadStart(SendGetImage));
                //getImages.Start();
            }


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
        public int ImagesToAverage
        {
            get { return m_ImagesToAverate; }
            set 
            { 
                if(value >= 100) { SetProperty<int>(ref m_ImagesToAverate, 100); }
                else if(value <= 1) { SetProperty<int>(ref m_ImagesToAverate, 1); }
                else { SetProperty<int>(ref m_ImagesToAverate, value); }
                RaisePropertyChanged(nameof(ImagesToAverage));
                UpdateXRayViewSettings();
            }
        }
        public string ROISetIndicator
        {
            get { return m_ROISetIndicator; }
            set 
            { 
                SetProperty<string>(ref m_ROISetIndicator, value); 
                RaisePropertyChanged(nameof(ROISetIndicator));
                UpdateXRayViewSettings();
            }
        }
        public bool ROISet { get; set; }
        public bool ROIAdjusted { get; set; }
        public Int32Rect ROIRect { get; set; }
        public int ImgIntegrationTime
        {
            get { return m_ImgIntegrationTime; }
            set 
            { 
                if(value >= m_MaxIntegrationTime) { SetProperty<int>(ref m_ImgIntegrationTime, m_MaxIntegrationTime); }
                else if (value <= 1) { SetProperty<int>(ref m_ImgIntegrationTime, 1); }
                else { SetProperty<int>(ref m_ImgIntegrationTime, value); } 
                RaisePropertyChanged(nameof(ImgIntegrationTime));
                UpdateXRayViewSettings();
            }
        }
        public int Gain
        {
            get { return m_Gain; }
            set 
            {
                if (value >= m_MaxGain) { SetProperty<int>(ref m_Gain, m_MaxGain); }
                else if (value <= m_MinGain) { SetProperty<int>(ref m_Gain, m_MinGain); }
                else { SetProperty<int>(ref m_Gain, value); }
                RaisePropertyChanged(nameof(Gain));
                UpdateXRayViewSettings();
            }
        }
        public int DigitalGain
        {
            get { return m_DigitalGain; }
            set
            {
                if (value >= m_MaxDigitalGain) { SetProperty<int>(ref m_DigitalGain, m_MaxDigitalGain); }
                else if (value <= m_MinDigitalGain) { SetProperty<int>(ref m_DigitalGain, m_MinDigitalGain); }
                else { SetProperty<int>(ref m_DigitalGain, value); }
                RaisePropertyChanged(nameof(DigitalGain));
                UpdateXRayViewSettings();
            }
        }
        /// <summary>
        /// The image from the camera
        /// </summary>
        public BitmapSource XRayImg
        {
            get
            {
                return m_XRayImg;
            }
        }
        /// <summary>
        /// active palette
        /// </summary>
        public String PaletteSelected
        {
            get { return m_paletteSelected; }
            set { m_paletteSelected = value; }
        }
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
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
            {
                return null;
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
                case 16:
                    ushort[] workingImage = new ushort[width * height];
                    Array.Copy(image, workingImage, width * height);
                    if(bitsPerPixel == 12)
                    {
                        for (int i = 0; i < workingImage.Length; i++) 
                        { 
                            ushort tempVal = (ushort)(workingImage[i]); 
                            workingImage[i] = (ushort)(tempVal << 4); 
                        }
                    }


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
                    ushort[] arr = new ushort[width * height];
                    Array.Copy(image, arr, width * height);
                    byte[] workingImage2 = new byte[width * height];
                    for(int i = 0; i < image.Length; i++)
                    {
                        workingImage2[i] = Convert.ToByte(arr[i]);
                    }                   

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
        //this function is for testing only
        private void SendGetImage()
        {
            while (!isDisposing)
            {
                CommandMessageEvent.Instance.Publish(new CommandMessage
                {
                    ID = "xray",
                    Command = "Take Image"
                });
                Thread.Sleep(10000);
            }
            Thread.Sleep(500);
        }
        public void ChangeROICurrentImage()
        {
            WriteableBitmap bmp = AdjustImageForDisplay(m_FullImage.Image, m_FullImage.Width, m_FullImage.Height, m_FullImage.BitsPerPixel);
            if (bmp != null)
            {
                m_XRayImg = bmp;
                RaisePropertyChanged(nameof(XRayImg));
            }
        }
        ///<summary>
        /// Update the GUI to reflect the current camera settings
        /// </summary>
        private void CameraSettings(object state)
        {
            CameraSettingsMessage msg = (CameraSettingsMessage)(state);
            if(msg.ID == "XRay")
            {
                m_MaxIntegrationTime = msg.MaxIntegrationTime;
                m_MinGain = msg.MinGain;
                m_MaxGain = msg.MaxGain;
                m_MinDigitalGain = msg.MinDigitalGain;
                m_MaxDigitalGain = msg.MaxDigitalGain;

                if(ImgIntegrationTime != msg.IntegrationTime) { ImgIntegrationTime = msg.IntegrationTime; }
                if(ImagesToAverage != msg.ImagesToAverage) { ImagesToAverage = msg.ImagesToAverage; }
                if(Gain != msg.Gain) { Gain = msg.Gain; }
                if(DigitalGain != msg.DigitalGain) { DigitalGain = msg.DigitalGain; }
            }
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

        private void ChangeROISettings(object state)
        {
            ImageViewROIMessage msg = (ImageViewROIMessage)(state);
            if(msg.ID == TwinID)
            {
                ROIRect = msg.ROIRect; 
                ROIAdjusted = msg.ROIAdjusted;
                ROISetIndicator = msg.ROISetIndicator;
                ROISet = msg.ROISet;
                ChangeROICurrentImage();
            }
        }

        private void ChangeXRayViewSettings(object state)
        {
            ImageViewMessage msg = (ImageViewMessage)(state);
            if(msg.ID != TwinID) { return; }
            if (msg.ImagesToAverage != ImagesToAverage) { ImagesToAverage = msg.ImagesToAverage; }
            if (msg.Gain != Gain) { Gain = msg.Gain; }
            if(msg.DigitalGain != DigitalGain) { DigitalGain = msg.DigitalGain; }
            if(msg.IntegrationTime != ImgIntegrationTime) { ImgIntegrationTime = msg.IntegrationTime; }
        }

        private void UpdateXRayViewSettings()
        {
            ImageViewMessageEvent.Instance.Publish(new ImageViewMessage() 
            {
                ID = this.ID, 
                ImagesToAverage = this.ImagesToAverage, 
                Gain = this.Gain,
                DigitalGain = this.DigitalGain,
                IntegrationTime = ImgIntegrationTime
            });
        }

        private void ImageDataMessageRecieved(object state)
        {
            ImageDataMessage msg = (ImageDataMessage)state;
            if(msg.Id == "XRay")
            {
                CryoviewTools.LogMessage(LogLevel.Debug6, ID + ": image recieved from camera at " + msg.Timestamp);
                m_FullImage = msg;
                WriteableBitmap bmp = AdjustImageForDisplay(msg.Image, msg.Width, msg.Height, msg.BitsPerPixel);
                if(bmp != null)
                {
                    m_XRayImg = bmp;
                    RaisePropertyChanged(nameof(XRayImg));
                }
            }
        }

        /// <summary>
        /// Find the palettes in the file system and bring them into the app for use while displaying images.
        /// </summary>
        private void LoadPalettes()
        {
            m_listPalettes = new List<BitmapPalette>();
            m_listPaletteNames = new List<string>();
            string palettesPath = Environment.CurrentDirectory + "\\palettes";  // must be a sub-dir under the install dir
            try
            {
                string[] paletteNames = Directory.GetFiles(palettesPath, "*.csv");  // get all file that are comma separated values
                foreach (string name in paletteNames)   // one file at a time
                {
                    BitmapPalette pal = null;
                    string name2 = System.IO.Path.GetFileNameWithoutExtension(name);
                    m_listPaletteNames.Add(name2);  // for the dropdown box
                    var reader = new StreamReader(File.OpenRead(name));
                    List<Color> colors = new List<Color>();
                    reader.ReadLine();   // first line of each pallette file is the column headers.
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        Color color = new Color();
                        color.R = Convert.ToByte(values[0]);
                        color.B = Convert.ToByte(values[1]);
                        color.G = Convert.ToByte(values[2]);
                        colors.Add(color);
                    }
                    pal = new BitmapPalette(colors);
                    if (pal != null) m_listPalettes.Add(pal);   // add to the dropdown box
                }
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
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
        private void SetInitialImage( int width, int height, string ViewID, int pixel_stride, ref byte[] palettizedImage)
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
