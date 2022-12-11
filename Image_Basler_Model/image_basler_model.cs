using System;
using System.Runtime.CompilerServices;  // CallerFilePath, etc
using System.Collections.Generic;   // KeyValuePair
using System.Collections.Concurrent;    // ConcurrentDictionary<T, T>
using System.Threading;
using System.Net;
using Prism.Commands;
using Prism.Mvvm;
using Basler.Pylon;

using Cryoview_ModuleMessages;
using DB_Model;
using Cryoview_Tools;   // logging
using LLE.Util;         // logging


namespace Image_Basler_Model
{
    public class ImageBaslerModel : IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private object m_objLock = new object();
        private Camera camera = null;
        private DBModel m_db = null;
        private bool m_isAcquiring = false;
        private Thread startAcquiring;
        private bool isDisposing = false;
        private int reacquireCount = 0;
        private bool hasTriedReconnect = false;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public ImageBaslerModel(string id)
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Creating Camera");
            m_db = new DBModel();
            ID = id;
            Initialize();
            CryoviewTools.LogMessage(LogLevel.Debug7, "Camera Created");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Disposing camera");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)  // managed resources here 
            {
                isDisposing = true;
                if (startAcquiring != null) { startAcquiring.Join(); }
                ImageViewMessageEvent.Instance.Unsubscribe(ChangeCameraSettings);
                ViewReadyMessageEvent.Instance.Unsubscribe(ImgViewReady); 
                CommandMessageEvent.Instance.Unsubscribe(CommandRecieved);
                if (camera != null && camera.IsOpen) { camera.Close(); camera.Dispose(); }
                else if(camera != null) { camera.Dispose(); }
                m_db.Dispose();
            }
            { } // unmanaged resources here
        }
        #endregion ctors/dtors/dispose

        #region initialization
        public bool Initialize()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Initializing Camera");
            bool bRetVal = true;
            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();

            //retrieve camera settings from database
            m_db.RetrieveSettings(ID, "OpticalCamera", configParams);
            CryoviewTools.LogMessage(LogLevel.Info, "Settings retrieved for optical camera " + ID);

            foreach (KeyValuePair<string, string> kvp in configParams)
            {
                CryoviewTools.LogMessage(LogLevel.Info, kvp.Key.ToString() + " / " + kvp.Value.ToString());
            }

            string tempVal = "";
            configParams.TryGetValue("SerialNumber", out tempVal); SerialNumber = tempVal;
            configParams.TryGetValue("IPAddress", out tempVal); IPAddress = tempVal;
            configParams.TryGetValue("MaxGain", out tempVal); MaxGain = Convert.ToInt32(tempVal);
            configParams.TryGetValue("MinGain", out tempVal); MinGain = Convert.ToInt32(tempVal);
            configParams.TryGetValue("PixelFormat", out tempVal);
            FramesPerSecond = 5;

            switch (tempVal)
            {
                case "Mono8":
                    PixelFormat = PixelType.Mono8;
                    break;
                case "Mono12":
                    PixelFormat = PixelType.Mono12;
                    break;
                default:
                    //alert issue
                    break;
            }

            //List<ICameraInfo> listcameras = CameraFinder.Enumerate(); //used for debugging
            
            try
            {
                camera = new Camera(SerialNumber);
                camera.Open();
                camera.Parameters[PLCamera.PixelFormat].SetValue(PixelFormat.ToString());
                camera.Parameters[PLCamera.GainRaw].SetValue(MinGain);
                camera.Close();

                AcquireImage();
                startAcquiring = new Thread(new ThreadStart(StartAcquiring));
                startAcquiring.Start();
            }
            catch(Exception ex)
            {
                CryoviewTools.LogMessage(LogLevel.Err, "Failed to connect to or open camera: " + ex.ToString());
                OnExceptionRaised("Failed to connect to or open camera: " + ex.ToString());
                if (camera != null ) { camera.Dispose(); camera = null; }
                bRetVal = false;
            }

            ViewReadyMessageEvent.Instance.Subscribe(ImgViewReady); //tells the camera that one of the image views is ready to recieve settings and images
            ImageViewMessageEvent.Instance.Subscribe(ChangeCameraSettings); //settings changed in image view
            CommandMessageEvent.Instance.Subscribe(CommandRecieved); //recieved a command from the image view (take image is currently the only command accepted)
            CryoviewTools.LogMessage(LogLevel.Debug7, "Camera Initialized");

            return bRetVal;
        }
        #endregion initialization

        #region properties

        public string SerialNumber { get; set; }
        public string ID { get; set; }
        public string IPAddress { get; set; }
        public int MaxGain { get; set; }
        public int MinGain { get; set; }
        public PixelType PixelFormat { get; set; }
        public int FramesPerSecond { get; set; }
        public ImageDataMessage LastImage { get; set; }


        #endregion properties

        #region algorithm code
        #endregion algorithm code

        #region hardware code

        public void AcquireImage()
        {
            try
            {
                m_isAcquiring = true;
                camera.Open();
                camera.StreamGrabber.Start();
                IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
                camera.ExecuteSoftwareTrigger();
                Array CameraData = null;
                int bitsPerPixel = 0;

                using (grabResult)
                {
                    if (grabResult.GrabSucceeded)
                    {
                        switch (grabResult.PixelTypeValue)
                        {
                            case PixelType.Mono8:
                                bitsPerPixel = 8;
                                CameraData = Array.CreateInstance(typeof(byte), grabResult.Width * grabResult.Height);
                                Buffer.BlockCopy(grabResult.PixelData as Array, 0, CameraData, 0, grabResult.Width * grabResult.Height);
                                break;

                            case PixelType.Mono12:
                                bitsPerPixel = 12;
                                CameraData = Array.CreateInstance(typeof(ushort), grabResult.Width * grabResult.Height);
                                Buffer.BlockCopy(grabResult.PixelData as Array, 0, CameraData, 0, grabResult.Width * grabResult.Height * 2);
                                break;
                        }
                    }
                    if (CameraData == null)
                    {
                        CryoviewTools.LogMessage(LogLevel.Err, ID + ": " + grabResult.ErrorDescription);
                        ReaqcuireImage(grabResult.ErrorDescription);
                        return;
                    }

                    reacquireCount = 0;
                    LastImage = new ImageDataMessage
                    {
                        Image = CameraData,
                        Height = grabResult.Height,
                        Width = grabResult.Width,
                        HorizontalBinning = 1,
                        VerticalBinning = 1,
                        BitsPerPixel = bitsPerPixel,
                        Stride = grabResult.Width * (int)Math.Ceiling((double)bitsPerPixel / 8.0f),
                        Timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ff tt"),
                        Id = ID,
                        Gain = (int)camera.Parameters[PLCamera.GainRaw].GetValue(),
                        Exposure = (Single)camera.Parameters[PLCamera.ExposureTimeAbs].GetValue()
                    };

                    ImageDataMessageEvent.Instance.Publish(LastImage);
                }
                camera.StreamGrabber.Stop();
                camera.Close();
                m_isAcquiring = false;
            }
            catch (Exception ex)
            {
                CryoviewTools.LogMessage(LogLevel.Err, ID + ": " + ex.Message.ToString() + "/n" + ex.StackTrace.ToString());
                if (!camera.IsConnected && !hasTriedReconnect)
                {
                    OnExceptionRaised("Lost connection to camera with serial number " + SerialNumber);
                    bool success = reconnect();
                    if (!success) { OnExceptionRaised("Failed to reconnected to camera with serial number " + SerialNumber); }
                    else
                    {
                        camera.StreamGrabber.Stop();
                        camera.Close();
                    }
                }
                else
                {
                    camera.StreamGrabber.Stop();
                    camera.Close();
                }

                m_isAcquiring = false;
            }
        }
        #endregion hardware code

        #region utility functions
        private bool reconnect()
        {
            bool bRetVal = true;

            if (camera != null) 
            { 
                camera.Close(); 
                camera.Dispose(); 
                camera = null; 
            }

            try
            {
                camera = new Camera(SerialNumber);
                camera.Open();
                if (!camera.IsConnected) { bRetVal = false; hasTriedReconnect = true; }
                camera.Parameters[PLCamera.PixelFormat].SetValue(PixelFormat.ToString());
                camera.Parameters[PLCamera.GainRaw].SetValue(MinGain);
                camera.Close();
            }
            catch (Exception ex)
            {
                CryoviewTools.LogMessage(LogLevel.Err, "Failed to connect to or open camera: " + ex.ToString());
                OnExceptionRaised("Failed to connect to or open camera: " + ex.ToString());
                if (camera != null) { camera.Dispose(); camera = null; }
                bRetVal = false;
                hasTriedReconnect = true;
            }

            if (!bRetVal) { isDisposing = true; }
            return bRetVal;
        }
        private void ReaqcuireImage(string err)
        {
            if (reacquireCount >= 3) { OnExceptionRaised("Unable to acquire images: " + err); }
            else 
            { 
                reacquireCount++;
                AcquireImage();
            }
        }

        private void ChangeCameraSettings(object state)
        {
            ImageViewMessage msg = (ImageViewMessage)(state);
            if(camera != null && ((ID.Contains("X") && (msg.ID == "OpticalView1" || msg.ID == "OpticalView2")) || (ID.Contains("Y") && (msg.ID == "OpticalView3" || msg.ID == "OpticalView4"))))
            {

                try
                {
                    camera.Open();
                    if (camera.Parameters[PLCamera.GainRaw].GetValue() != msg.RawGain) 
                    {
                        camera.Parameters[PLCamera.GainRaw].SetValue(msg.RawGain);
                        CryoviewTools.LogMessage(LogLevel.Debug5, "Camera " + ID + " gain set to " + msg.RawGain.ToString());
                    }
                    if(camera.Parameters[PLCamera.ExposureTimeAbs].GetValue() != msg.Exposure) 
                    {
                        camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(msg.Exposure);
                        CryoviewTools.LogMessage(LogLevel.Debug5, "Camera " + ID + " exposure set to " + msg.Exposure.ToString());
                    }
                    if (!m_isAcquiring) { camera.Close(); }
                }
                catch(Exception ex)
                {
                    CryoviewTools.LogMessage(LogLevel.Err, ID + ": " + ex.Message.ToString() + "/n" + ex.StackTrace.ToString());
                    if (!camera.IsConnected && !hasTriedReconnect)
                    {
                        OnExceptionRaised("Lost connection to camera with serial number " + SerialNumber);
                        bool success = reconnect();
                        if (!success) { OnExceptionRaised("Failed to reconnected to camera with serial number " + SerialNumber); }
                    }
                }

                lock (m_objLock)
                {
                    if(FramesPerSecond != msg.FrameRate) 
                    { 
                        FramesPerSecond = msg.FrameRate;
                        CryoviewTools.LogMessage(LogLevel.Debug5, "Camera " + ID + " frame rate set to " + msg.FrameRate.ToString() + " frames per second");
                    }
                }

            }
            else { return; }
        }

        private void ImgViewReady(object state)
        {
            ViewReadyMessage msg = (ViewReadyMessage)(state);
            if ((ID.Contains("X") && (msg.ID == "OpticalView1" || msg.ID == "OpticalView2")) || (ID.Contains("Y") && (msg.ID == "OpticalView3" || msg.ID == "OpticalView4")))
            {
                if(camera != null)
                {
                    SendCameraSettings();
                }
                
            }
        }

        private void CommandRecieved(object state)
        {
            CommandMessage msg = (CommandMessage)(state);
            if (camera != null && ((ID.Contains("X") && (msg.ID == "OpticalView1" || msg.ID == "OpticalView2")) || (ID.Contains("Y") && (msg.ID == "OpticalView3" || msg.ID == "OpticalView4"))))
            { 
                if(msg.Command == "Take Image") { AcquireImage(); }
            }
        }

        private void SendCameraSettings()
        {
            try
            {
                camera.Open();
                Single exposure = (Single)camera.Parameters[PLCamera.ExposureTimeAbs].GetValue();
                int gain = Convert.ToInt32(camera.Parameters[PLCamera.GainRaw].GetValue());
                if (!m_isAcquiring) { camera.Close(); }

                CameraSettingsMessageEvent.Instance.Publish(new CameraSettingsMessage()
                {
                    ID = this.ID,
                    Exposure = exposure,
                    MinGain = this.MinGain,
                    MaxGain = this.MaxGain,
                    Gain = gain
                }) ;   
            }
            catch(Exception ex)
            {
                CryoviewTools.LogMessage(LogLevel.Err, ID + ": " + ex.Message.ToString() + "/n" + ex.StackTrace.ToString());
                if (!camera.IsConnected && !hasTriedReconnect)
                {
                    OnExceptionRaised("Lost connection to camera with serial number " + SerialNumber);
                    bool success = reconnect();
                    if (!success) { OnExceptionRaised("Failed to reconnected to camera with serial number " + SerialNumber); }
                }
            }
        }

        private void StartAcquiring()
        {
            CryoviewTools.LogMessage(LogLevel.Debug6, "Camera " + ID + " acquire thread started");
            while (!isDisposing)
            {
                int sleepTime = 0;
                lock (m_objLock) {sleepTime = 1000 / FramesPerSecond; }
                Thread.Sleep(sleepTime);
                AcquireImage();
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug6, "Camera " + ID + " acquire thread stopping");
            Thread.Sleep(500);
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
        #endregion event sinks
    }
}
