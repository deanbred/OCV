using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Cryoview_Tools;   // logging
using LLE.Util;         // logging
using Cryoview_ModuleMessages;
using XRay_Interface;
using CsLibIf;

namespace XRay_Physical_Model
{
    public class XRayPhysicalModel : IDisposable, XRayInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private object m_objectLock = new object();
        private bool m_connected = false;
        private bool hasTriesReconnect = false;
        private string m_DeviceName = "";
        private string m_PluginName = "";
        private int m_CamHandle = -1;
        private LibIf m_pLib = null;
        private int m_MinGain = 0;
        private int m_MaxGain = 0;
        private int m_MinDigitalGain = 0;
        private int m_MaxDigitalGain = 0;
        private int m_MaxIntegrationTime = 0;
        private int m_HorizontalBinning = 0;
        private int m_VerticalBinning = 0;
        private int m_BitsPerPixel = 0;

        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public XRayPhysicalModel(string device, string plugin)
        {
            m_DeviceName = device;
            m_PluginName = plugin;
            m_pLib = new LibIf();
            Imgs2Average = 1;
            IntegrationTime = 1;
            Gain = 0;
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
                if (m_connected)
                {
                    ViewReadyMessageEvent.Instance.Unsubscribe(ViewReadyRecieved);
                    CommandMessageEvent.Instance.Unsubscribe(CommandRecieved);
                    ImageViewMessageEvent.Instance.Unsubscribe(CameraSettingsRecieved);
                    if(m_CamHandle >= 0) { m_pLib.CloseCamera(m_CamHandle); m_CamHandle = -1; }
                    m_connected = false;
                }
            }
            // unmanaged resources here
            {
            }
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        public bool Initialize()
        {
            bool m_RetVal = true;

            CryoviewTools.LogMessage(LogLevel.Info, "Connecting to X-Ray camera: " + m_DeviceName + " Plugin: " + m_PluginName);
            try
            {
                //for getting the device name and plugin name for the camera, one time thing.
                string camsAvailable = m_pLib.ScanInterfaces(false);
                CryoviewTools.LogMessage(LogLevel.Info, camsAvailable);

                m_CamHandle = m_pLib.OpenCamera(m_DeviceName, m_PluginName);
            }
            catch(Exception ex)
            {
                OnExceptionRaised(ex.Message);
            }

            if (m_CamHandle >= 0)
            {
                CryoviewTools.LogMessage(LogLevel.Info, "X-Ray camera connected");
                string ParamaterName = "Exposure Time";
                string valStr = "";
                string minStr = "";
                string maxStr = "";
                string unitStr = "";
                string stepStr = "";
                LibIf.UnitType uType = 0;

                int error = m_pLib.GetParameterItem(m_CamHandle, ParamaterName, ref valStr, ref minStr, ref maxStr, ref uType, ref unitStr, ref stepStr);
                if(error == 0)
                {
                    IntegrationTime = Convert.ToInt32(valStr);
                    m_MaxIntegrationTime = Convert.ToInt32(maxStr);
                }
                else 
                { 
                    CryoviewTools.LogMessage(LogLevel.Err, "Failed to get parameters for " + ParamaterName); 
                    OnExceptionRaised("Failed to get camera parameter: " + ParamaterName); 
                    m_RetVal = false; 
                }

                ParamaterName = "Gain";
                error = m_pLib.GetParameterItem(m_CamHandle, ParamaterName, ref valStr, ref minStr, ref maxStr, ref uType, ref unitStr, ref stepStr);
                if (error == 0)
                {
                    Gain = Convert.ToInt32(valStr);
                    m_MaxGain = Convert.ToInt32(maxStr);
                    m_MinGain = Convert.ToInt32(minStr);
                }
                else
                {
                    CryoviewTools.LogMessage(LogLevel.Err, "Failed to get parameters for " + ParamaterName);
                    OnExceptionRaised("Failed to get camera parameter: " + ParamaterName);
                    m_RetVal = false;
                }

                ParamaterName = "Digital Gain";
                error = m_pLib.GetParameterItem(m_CamHandle, ParamaterName, ref valStr, ref minStr, ref maxStr, ref uType, ref unitStr, ref stepStr);
                if (error == 0)
                {
                    DigitalGain = Convert.ToInt32(valStr);
                    m_MaxDigitalGain = Convert.ToInt32(maxStr);
                    m_MinDigitalGain = Convert.ToInt32(minStr);
                }
                else
                {
                    CryoviewTools.LogMessage(LogLevel.Err, "Failed to get parameters for " + ParamaterName);
                    OnExceptionRaised("Failed to get camera parameter: " + ParamaterName);
                    m_RetVal = false;
                }

                ParamaterName = "Parallel Binning";
                error = m_pLib.GetParameterItem(m_CamHandle, ParamaterName, ref valStr, ref minStr, ref maxStr, ref uType, ref unitStr, ref stepStr);
                if (error == 0)
                {
                    m_HorizontalBinning = Convert.ToInt32(valStr);
                }
                else
                {
                    CryoviewTools.LogMessage(LogLevel.Err, "Failed to get parameters for " + ParamaterName);
                    OnExceptionRaised("Failed to get camera parameter: " + ParamaterName);
                    m_RetVal = false;
                }

                ParamaterName = "Serial Binning";
                error = m_pLib.GetParameterItem(m_CamHandle, ParamaterName, ref valStr, ref minStr, ref maxStr, ref uType, ref unitStr, ref stepStr);
                if (error == 0)
                {
                    m_VerticalBinning = Convert.ToInt32(valStr);
                }
                else
                {
                    CryoviewTools.LogMessage(LogLevel.Err, "Failed to get parameters for " + ParamaterName);
                    OnExceptionRaised("Failed to get camera parameter: " + ParamaterName);
                    m_RetVal = false;
                }

                ParamaterName = "Bits Per Pixel";
                error = m_pLib.GetParameterItem(m_CamHandle, ParamaterName, ref valStr, ref minStr, ref maxStr, ref uType, ref unitStr, ref stepStr);
                if (error == 0)
                {
                    m_BitsPerPixel = Convert.ToInt32(valStr);
                }
                else
                {
                    CryoviewTools.LogMessage(LogLevel.Err, "Failed to get parameters for " + ParamaterName);
                    OnExceptionRaised("Failed to get camera parameter: " + ParamaterName);
                    m_RetVal = false;
                }
            }
            else
            {
                m_RetVal = false;
                CryoviewTools.LogMessage(LogLevel.Err, "Failed to connect to X-Ray camera: " + m_DeviceName + " Plugin: " + m_PluginName);
            }

            if (m_RetVal)
            {
                m_connected = true;
                ViewReadyMessageEvent.Instance.Subscribe(ViewReadyRecieved);
                CommandMessageEvent.Instance.Subscribe(CommandRecieved);
                ImageViewMessageEvent.Instance.Subscribe(CameraSettingsRecieved);
            }
            else { m_pLib.CloseCamera(m_CamHandle); }
            return m_RetVal;
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        public int Gain { get; set; }
        public int DigitalGain { get; set; }
        public int IntegrationTime { get; set; }
        public int Imgs2Average { get; set; }
        #endregion properties

        #region bindable properties
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        public ImageDataMessage AverageImages(ImageDataMessage[] images)
        {
            int numPixels = images[0].Height * images[0].Width;
            int[] averageImage = new int[numPixels];
            Array.Copy(images[0].Image, averageImage, numPixels);

            foreach (ImageDataMessage msg in images)
            {
                ushort[] workingImage = new ushort[numPixels];
                Array.Copy(msg.Image, workingImage, numPixels);
                for (int i = 1; i < numPixels; i++)
                {
                    averageImage[i] += workingImage[i];
                }
            }

            for (int i = 0; i < numPixels; i++)
            {
                averageImage[i] = averageImage[i] / images.Length;
            }

            ushort[] finalImage = new ushort[numPixels];
            var arr = Array.ConvertAll(averageImage, val => (ushort)val);
            Array.Copy(arr, finalImage, numPixels);

            ImageDataMessage img = new ImageDataMessage
            {
                Image = finalImage,
                Height = images[0].Height,
                Width = images[0].Width,
                HorizontalBinning = m_HorizontalBinning,
                VerticalBinning = m_VerticalBinning,
                BitsPerPixel = m_BitsPerPixel,
                Stride = images[0].Stride,
                Timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ff tt"),
                Id = "XRay",
                Gain = images[0].Gain,
                Exposure = images[0].Exposure
            };

            return img;
        }
        #endregion algorithm code

        #region hardware code
        public void AcquireImage() 
        {
            lock (m_objectLock)
            {
                if (m_CamHandle >= 0)
                {
                    ImageDataMessage[] images = new ImageDataMessage[Imgs2Average];
                    bool hasFailed = false;

                    for (int i = 0; i < Imgs2Average; i++)
                    {
                        string errorString = "";
                        UInt32 imgSerLen = 0;
                        UInt32 imgParLen = 0;
                        UInt32 is16 = 0;
                        UInt32 nSerCCD = 0;
                        UInt32 nParCCD = 0;
                        UInt32 nSerSect = 0;
                        UInt32 nParSect = 0;
                        UInt16 nBuffers = 1;
                        UInt16 pctRead = 0;
                        UInt32 CurrentFrame = 0;
                        UInt32 FrameReady = 0xffffffff;
                        UInt32 prevFrame = 0;
                        int j = 0;

                        m_pLib.SetParameterValue(m_CamHandle, "Frame Count", nBuffers.ToString());
                        m_pLib.SetParameterValue(m_CamHandle, "Exposure Time", IntegrationTime.ToString());
                        m_pLib.SendParameters(m_CamHandle);

                        m_pLib.GetImageSize(m_CamHandle, ref imgSerLen, ref imgParLen, ref is16, ref nSerCCD, ref nParCCD, ref nSerSect, ref nParSect);
                        m_pLib.PrepareAcqFilm(m_CamHandle, (UInt16)imgSerLen, (UInt16)imgParLen, nBuffers, is16);
                        errorString = m_pLib.IssueCommand(m_CamHandle, "ACQUIRE", "0");

                        if( errorString.Contains("ERROR"))
                        {
                            if (!hasFailed)
                            {
                                hasFailed = true;
                                i--;
                            }
                            else
                            {
                                CryoviewTools.LogMessage(LogLevel.Err, "Failed to acquire X-Ray image: " + errorString);
                                OnExceptionRaised("Failed to acquire X-Ray image: " + errorString);
                                return;
                            }

                        }
                        else
                        {
                            for (j = 0; j < 2000; j++)
                            {
                                m_pLib.AcqStatus(m_CamHandle, ref pctRead, ref CurrentFrame, ref FrameReady);
                                if (j == 0) { prevFrame = FrameReady; }
                                Thread.Sleep(10);
                                if (prevFrame != FrameReady) { break; }
                                prevFrame = FrameReady;
                            }

                            m_pLib.EndAcq(m_CamHandle, true);       // note: this MUST be called after every acquisition !!
                            ushort[] imArrU = m_pLib.GetUshortImage();  // get the image; image buffer is controlled by LibIf.cs

                            ushort max = 0;
                            ushort min = 0xFFFF;
                            ushort sVal;
                            double lPixval;
                            double factor;

                            for (j = 0; j < imArrU.Length; j++)
                            {
                                if (imArrU[j] > max) max = imArrU[j];
                                if (imArrU[j] < min) min = imArrU[j];
                            }

                            if ((max - min) != 0) { factor = (double)65536 / ((double)max - (double)min); }   // prevent div by 0
                            else { factor = 1; }

                            for(j = 0; j < imArrU.Length; j++)
                            {
                                sVal = (imArrU[j]);
                                lPixval = ((double)sVal - (double)min) * factor; // Convert to a 65535 value range
                                if (lPixval > 65535) { lPixval = 65535; }
                                if (lPixval < 0) { lPixval = 0; }
                                imArrU[j] = (ushort)lPixval;
                            }

                            ImageDataMessage img = new ImageDataMessage
                            {
                                Image = imArrU,
                                Height = (int)imgParLen,
                                Width = (int)imgSerLen,
                                HorizontalBinning = m_HorizontalBinning,
                                VerticalBinning = m_VerticalBinning,
                                BitsPerPixel = m_BitsPerPixel,
                                Stride = (int)imgSerLen * (int)Math.Ceiling((double)m_BitsPerPixel / 8.0f),
                                Timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ff tt"),
                                Id = "XRay",
                                Gain = this.Gain,
                                Exposure = IntegrationTime
                            };

                            images[i] = img;
                        } //else 
                    } //for (int i = 0; i < Imgs2Average; i++)

                    ImageDataMessage averagedImage = null;
                    if (images.Length > 1)
                    {
                        averagedImage = AverageImages(images);
                        ImageDataMessageEvent.Instance.Publish(averagedImage);
                    }
                    else if (images.Length > 0)
                    {
                        averagedImage = images[0];
                        ImageDataMessageEvent.Instance.Publish(averagedImage);
                    }
                }
            }
            
        }
        #endregion hardware code

        #region utility functions
        private void reconnect()
        {
            bool connected = false;
            if(m_CamHandle >= 0)
            {
                int error = m_pLib.GetStatus(m_CamHandle);
                if(error == 0) 
                { 
                    connected = true; 
                    hasTriesReconnect = false;
                    return;
                } //Already connected
                else 
                {
                    m_pLib.CloseCamera(m_CamHandle); 
                    m_CamHandle = -1;
                }
            }

            if (!connected)
            {
                try
                {
                    m_CamHandle = m_pLib.OpenCamera(m_DeviceName, m_PluginName);
                }
                catch (Exception ex)
                {
                    hasTriesReconnect = true;
                    OnExceptionRaised(ex.Message);
                }

                if(m_CamHandle >= 0)
                {
                    CryoviewTools.LogMessage(LogLevel.Info, "X-Ray camera connected");
                    hasTriesReconnect = false;
                }
                else
                {
                    hasTriesReconnect = true;
                    OnExceptionRaised("Failed to reconnect to X-Ray camera");
                }
            }
        }
        private void ViewReadyRecieved(object State)
        {
            ViewReadyMessage msg = (ViewReadyMessage)State;
            if (msg.ID == "XRayView2" || msg.ID == "XRayView1")
            {
                CameraSettingsMessageEvent.Instance.Publish(new CameraSettingsMessage
                {
                    ID = "XRay",
                    IntegrationTime = this.IntegrationTime,
                    MaxIntegrationTime = m_MaxIntegrationTime,
                    Gain = this.Gain,
                    MaxGain = m_MaxGain,
                    MinGain = m_MinGain,
                    DigitalGain = this.DigitalGain,
                    MaxDigitalGain = m_MaxDigitalGain,
                    MinDigitalGain = m_MinDigitalGain,
                    ImagesToAverage = Imgs2Average
                });
            }
        }
        private void CommandRecieved(object state)
        {
            CommandMessage msg = (CommandMessage)(state);
            if (msg.ID == "xray" && msg.Command == "Take Image") { AcquireImage(); }
        }
        private void CameraSettingsRecieved(object state)
        {
            ImageViewMessage msg = (ImageViewMessage)(state);
            if(msg.ID == "XRayView1" || msg.ID == "XRayView2")
            {
                lock (m_objectLock)
                {
                    bool changesMade = false;
                    if (msg.IntegrationTime != this.IntegrationTime) 
                    {
                        changesMade = true;
                        IntegrationTime = msg.IntegrationTime;
                        m_pLib.SetParameterValue(m_CamHandle, "Exposure Time", IntegrationTime.ToString());
                    }
                    if(msg.Gain != this.Gain) 
                    {
                        changesMade = true;
                        Gain = msg.Gain;
                        m_pLib.SetParameterValue(m_CamHandle, "Gain", Gain.ToString());
                    }
                    if(msg.DigitalGain != this.DigitalGain) 
                    {
                        changesMade = true;
                        DigitalGain = msg.DigitalGain;
                        m_pLib.SetParameterValue(m_CamHandle, "Digital Gain", DigitalGain.ToString());
                    }
                    if(changesMade == true) { m_pLib.SendParameters(m_CamHandle); }
                    if(msg.ImagesToAverage != Imgs2Average) { Imgs2Average = msg.ImagesToAverage; }
                }
            }
        }
        private void OnExceptionRaised(string msg)
        {
            if (!hasTriesReconnect) 
            { 
                if (ExceptionRaised != null)    // check for subscribers
                {
                    ExceptionRaised(msg);   // pass it on (to the view)
                }
                reconnect(); 
            }
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
