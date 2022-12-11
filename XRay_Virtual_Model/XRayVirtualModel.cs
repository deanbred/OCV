using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using Cryoview_Tools;   // logging
using LLE.Util;         // logging
using LLE.HDF4;
using XRay_Interface;
using DB_Model;
using Cryoview_ModuleMessages;

namespace XRay_Virtual_Model
{
    public class XRayVirtualModel : IDisposable, XRayInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        List<CameraImageData> lstImages = new List<CameraImageData>();
        int lstPtr = 0;
        bool m_isAcquiring = false;
        private DBModel m_dbModel = null; 
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public XRayVirtualModel()
        {
            m_dbModel = new DBModel();
            Gain = 0;
            DigitalGain = 0;
            Imgs2Average = 1;
            IntegrationTime = 1;
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
                m_dbModel.Dispose();
                CommandMessageEvent.Instance.Unsubscribe(CommandRecieved);
                ViewReadyMessageEvent.Instance.Unsubscribe(ViewReadyRecieved);
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
            ConcurrentDictionary<string, string> properties = CryoviewTools.GetAppProperties();
            string Location = "";
            if (properties.ContainsKey("Location")) { Location = properties["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.

            try
            {
                ConcurrentDictionary<string, string> configParams = CryoviewTools.GetAppProperties();
                m_dbModel.RetrieveSettings(Location, "XRaySimData", configParams);

                string fileSimData = "";
                configParams.TryGetValue("SimData", out fileSimData);
                SD hdfFile = new SD(fileSimData, DFACC.RDONLY);

                for (int i = 0; i < hdfFile.GetDataSetCount(); i++)
                {
                    SDDataSet hdfDataset = hdfFile.GetDataSet(i);
                    SDAttributes attrs = hdfDataset.GetAttributes();

                    CameraImageData data = new CameraImageData();
                    byte[] val = new byte[4];
                    attrs.GetArray("Image_Width", val);
                    string strVal = System.Text.Encoding.Default.GetString(val);
                    data.Width = Convert.ToInt32(strVal);
                    attrs.GetArray("Image_Height", val);
                    strVal = System.Text.Encoding.Default.GetString(val);
                    data.Height = Convert.ToInt32(strVal);
                    data.BitsPerPixel = 16;
                    ushort[] img = new ushort[data.Width * data.Height];
                    hdfDataset.ReadData(img, new int[] { 0, 0 }, new int[] { data.Width, data.Height });
                    data.Image = img;

                    lstImages.Add(data);
                }
            }
            catch(Exception ex)
            {
                HandleExceptions(ex);
            }

            ViewReadyMessageEvent.Instance.Subscribe(ViewReadyRecieved);
            CommandMessageEvent.Instance.Subscribe(CommandRecieved);

            return true;
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
        public int Imgs2Average { get; set; }
        public int IntegrationTime { get; set; }
        public ImageDataMessage LastImage { get; set; }
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
        public void AcquireImage()
        {
            m_isAcquiring = true;

            if(lstPtr >= lstImages.Count()) { lstPtr = 0; }

            LastImage = new ImageDataMessage
            {
                Image = lstImages[lstPtr].Image,
                Height = lstImages[lstPtr].Height,
                Width = lstImages[lstPtr].Width,
                HorizontalBinning = 1,
                VerticalBinning = 1,
                BitsPerPixel = lstImages[lstPtr].BitsPerPixel,
                Stride = lstImages[lstPtr].Width * sizeof(ushort),
                Timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ff tt"),
                Id = "XRay",
                Gain = this.Gain,
                Exposure = IntegrationTime
            };
            ImageDataMessageEvent.Instance.Publish(LastImage);
            lstPtr++;
            m_isAcquiring = false;
        }
        #endregion hardware code

        #region utility functions
        private void ViewReadyRecieved(object State)
        {
            ViewReadyMessage msg = (ViewReadyMessage)State;
            if (msg.ID == "XRayView2" || msg.ID == "XRayView1")
            {
                CameraSettingsMessageEvent.Instance.Publish(new CameraSettingsMessage
                {
                    ID = "XRay",
                    IntegrationTime = this.IntegrationTime,
                    MaxIntegrationTime = 10000,
                    Gain = this.Gain,
                    MaxGain = 100,
                    MinGain = 0,
                    DigitalGain = this.DigitalGain,
                    MaxDigitalGain = 10,
                    MinDigitalGain = 0,
                    ImagesToAverage = Imgs2Average
                });
            }
        }
        private void CommandRecieved(object state)
        {
            CommandMessage msg = (CommandMessage)(state);
            if(msg.ID == "xray" && msg.Command == "Take Image") { AcquireImage(); }
        }

        /// <summary>
        /// Aspect-oriented programming
        /// Break down program logic into distinct parts, aka concerns, one of which is handling exceptions.
        /// </summary>
        /// <param name="ex"></param>
        private void HandleExceptions(Exception ex,
                        [CallerFilePath] string sourceFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            CryoviewTools.LogMessage(LogLevel.Err, " Entering ... ");
            string trace = sourceFile.Substring(sourceFile.LastIndexOf('\\') + 1) + "(" + lineNumber + "): " + memberName + " --> ";
            int stopAt = 0;
            int len = ex.ToString().Length;
            if (len < 100) stopAt = len - 1; else stopAt = 99;
            if (ex.GetType() == typeof(InvalidCastException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(trace + " " + ex.ToString().Substring(0, stopAt));
            }
            else if (ex.GetType() == typeof(IndexOutOfRangeException))
            {   // thrown if reader["..."] attempts to get a value not retrieved by the sql statement. Could be catastrophic.
                // Require that code or db be fixed before continuing.
                // catching something like reader["xxx"] or reader[27] does not exist. In a data-correct table, this should not happen. 
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(ex.ToString());
            }
            else if (ex.GetType() == typeof(FormatException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(ex.ToString());
            }
            else
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(trace + ex.ToString().Substring(0, stopAt));
            }
            CryoviewTools.LogMessage(LogLevel.Err, " Exiting ... ");
        }
        private void OnExceptionRaised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, " Entering ... ");
            if (ExceptionRaised != null)
            {
                ExceptionRaised(msg);
            }
            CryoviewTools.LogMessage(LogLevel.Debug, " Exiting ... ");
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }

    public class CameraImageData
    {
        public Array Image { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int BitsPerPixel { get; set; }
    }
}
