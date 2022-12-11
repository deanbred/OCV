using System;
using System.Collections.Generic;   // dictionary
using System.Collections.Concurrent;     // ConcurrentDictionary<T, T>
using System.Runtime.CompilerServices;  // [CallerFilePath], etc. Reqs .net 4.5.
using System.IO;    // check hdf file existence
using System.Threading; // CancellationTokens
using System.Threading.Tasks;   // parallel operation
using Prism;

using Cryoview_ModuleMessages;
using DB_Model;
using Cryoview_Tools;   // logging
using LLE.Util;         // logging
using LLE.HDF4;

namespace HDF_File_Model
{
    public class HDFFileModel : IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // asnyc notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private Object m_objLock = new Object();
        private DBModel m_db = null;
        private ConcurrentDictionary<string, string> m_reportableSettings = null; 
        private ConcurrentDictionary<string, string> m_appSettings = null;
        private SD m_sdFile = null; // handle to the hdf file
        private ImageDataMessage m_imgData = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        private HDFFileModel() { }

        public HDFFileModel(string id)
        {
            Id = id;
            m_reportableSettings = new ConcurrentDictionary<string, string>();
            m_db = new DBModel();
            FileName = "";
            Initialize();
        }

        /// <summary>
        /// got here by user code; clean up any file handles
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }  

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param keyToRetrieve="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)  // managed resources here 
            {
                DataReportableSettingsEvent.Instance.Unsubscribe(DataReportableSettingsUpdated);
                ImageDataMessageEvent.Instance.Unsubscribe(ImageDataUpdated);
                if (m_sdFile != null) { m_sdFile.Dispose(); }
            }
            { } // unmanaged resources here
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        public void Initialize()
        {
            FileIsOpen = false;
            m_appSettings = CryoviewTools.GetAppProperties();
            GetAttributesFromDB();
            DataReportableSettingsEvent.Instance.Subscribe(DataReportableSettingsUpdated);
            ImageDataMessageEvent.Instance.Subscribe(ImageDataUpdated);
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        public string FileID { get; set; }
        public string FileName { get; set; }

        public bool FileIsOpen { get; set; }

        /// <summary>
        /// number of datasets currently in the hdf file
        /// </summary>
        public int DatasetCount { get; set; }

        /// <summary>
        ///  where to write the hdf file
        /// </summary>
        public string HdfDir { get; set; }

        /// <summary>
        /// hdf dir and file keyToRetrieve
        /// </summary>
        public string FullPathToFile { get; set; }

        /// <summary>
        /// discriminator to allow for multiple instances of this class
        /// </summary>
        public string Id { get; set; }  // differentiates btwn instances of the HdfModel.

        //public OperationalStatusMessage.TypeOfOperation OpInProgress { get; set; }
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
        public bool CreateHDFFile() 
        {
            bool m_bRetVal = true;
            SDAttributes fileAttributes = null;
            FileName = Id + FileID + ".hdf";

            try
            {
                if (Directory.Exists(HdfDir)) { } //no need to do anything
                else { Directory.CreateDirectory(HdfDir); }
                FullPathToFile = HdfDir + "\\" + FileName;
                CryoviewTools.LogMessage(LogLevel.Info, " Creating HDF File: " + FullPathToFile);
                m_reportableSettings.AddOrUpdate("FullPathToFile", FullPathToFile, (k, v) => FullPathToFile);
                m_sdFile = new SD(FullPathToFile, DFACC.ALL);
                fileAttributes = m_sdFile.GetAttributes();
            }
            catch(Exception ex)
            {
                HandleExceptions(ex);
                m_bRetVal = false;
            }

            if (m_bRetVal)
            {
                string name;
                string value;
                name = "Timestamp";
                value = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
                m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);
                m_reportableSettings.AddOrUpdate("File_Name", FileName, (k, v) => value);
                m_reportableSettings.AddOrUpdate("Axis", Id, (k, v) => value);

                foreach(var kvp in m_reportableSettings)
                {
                    switch (kvp.Key.ToString())
                    {
                        case "Axis":
                        case "File_Name":
                        case "TargetID":
                        case "Comments":
                        case "Timestamp":
                            WriteAttribute(kvp.Key.ToString(), kvp.Key.ToString(), "string", m_reportableSettings, fileAttributes);
                            break;
                        default:
                            //do nothing
                            break;
                    }
                }
                FileIsOpen = true;
                DatasetCount = 0;
            }

            /// Because hdf files do not provide locking against multiple sources writing to the file, each op in this class
            /// disposes of the SD resource. This way if some other program, matlab or whatever, accesses the hdf file
            /// there is a reduced possibility of file corruption.
            if (m_sdFile != null) m_sdFile.Dispose();
            return m_bRetVal;
        }

        public void CloseHDFFile()
        {
            if (m_sdFile != null) m_sdFile.Dispose(); // shouldn't happen because each op disposes of the SD resource.
            FileIsOpen = false;
            DatasetCount = 0;
        }

        public bool OpenHDFFile()
        {
            bool m_bRetVal = true;
            SDAttributes attrs = null;
            string msg = "";

            try
            {
                msg = "can't open file";
                m_sdFile = new SD(FullPathToFile, DFACC.WRITE);
                msg = "Can't get file attributes";
                attrs = m_sdFile.GetAttributes();
                DatasetCount = m_sdFile.GetDataSetCount();
                FileIsOpen = true;

                //Compare data in file to actual settings to ensure we are only opening 
                //and appending datasets if the external conditions are the same.
                //string targetIDHardware = ""; string targetIDHDFFile = "";
                //m_reportableSettings.TryGetValue("Target Id", out targetIDHardware);
                //if(targetIDHardware == null) { targetIDHardware = ""; }
                //msg = "missing target ID from file";
                //targetIDHDFFile = attrs.GetString("Target Id");
                //if (targetIDHDFFile.Equals(targetIDHardware))
                //{
                //    FileIsOpen = true;
                //}
                //else
                //{
                //    m_bRetVal = false;
                //    FileIsOpen = false;
                //    HandleExceptions(new Exception("adding data to this location is not allowed \r\n Target ID's are different"));
                //}
            }
            catch(Exception ex)
            {
                m_bRetVal = false;
                FileIsOpen = false;
                HandleExceptions(new Exception("Opening hdf file: " + FullPathToFile + Environment.NewLine + "Failed: " + msg + Environment.NewLine + ex.ToString()));
            }
            finally
            {
                if (m_sdFile != null) { m_sdFile.Dispose(); }
            }

            return m_bRetVal;
        }

        /// <summary>
        /// Write the dataset info to the hdf file.
        /// Need lock object in case user clicks save data rapidly. reportable settings is async updated based on msgs from hardware code.
        /// No simo writing/reading allowed.
        /// </summary>
        public bool StoreDataHDFFile( int datasetId) 
        {
            bool m_bRetVal = true;

            if (FullPathToFile == null || !FileIsOpen) // for some reason there is no hdf file open
            {
                m_bRetVal = false;
                CryoviewTools.LogMessage(LogLevel.Err, " For some reason, no file specified ... ");
                return m_bRetVal;
            }

            lock (m_objLock)
            {
                ConcurrentDictionary<object, object> data = new ConcurrentDictionary<object, object>();
                SDDataSet ds = null;
                SDAttributes dsAttrs = null;
                string name = "";
                string value = "";
                Array imgData = null;

                try
                {
                    m_sdFile = new SD(FullPathToFile, DFACC.WRITE);
                }
                catch (Exception ex)
                {
                    HandleExceptions(new Exception(" Failed to open " + FullPathToFile + Environment.NewLine + ex.Message));
                    m_bRetVal = false;
                    if (m_sdFile != null) { m_sdFile.Dispose(); }
                    return m_bRetVal;
                }

                DatasetCount = datasetId;
                string dsName = "Image " + DatasetCount.ToString();
                name = "DatasetId"; value = DatasetCount.ToString(); m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);

                try
                {
                    if (m_imgData == null)
                    {
                        //No image to save
                    }
                    else
                    {
                        ds = m_sdFile.CreateDataSet(dsName, DFNT.UINT16, new[] { m_imgData.Width, m_imgData.Height });
                        ds.SetCompressDeflate(6);   // gzip
                        dsAttrs = ds.GetAttributes();
                        imgData = (Array)m_imgData.Image;
                        name = "Image_Width"; value = m_imgData.Width.ToString(); m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);
                        name = "Image_Height"; value = m_imgData.Height.ToString(); m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);
                        name = "Camera Binning"; value = Convert.ToString(m_imgData.HorizontalBinning + ", " + m_imgData.VerticalBinning);
                        m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);
                        name = "Gain"; value = m_imgData.Gain.ToString(); m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);
                        name = "Exposure"; value = m_imgData.Exposure.ToString(); m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);
                        name = "Timestamp"; value = m_imgData.Timestamp; m_reportableSettings.AddOrUpdate(name, value, (k, v) => value);

                        StoreDatasetAttributesToDataset(ref dsAttrs);
                        StoreCameraAttributesToDataset(ref dsAttrs);

                        if (m_imgData.Image.GetType() == typeof(System.UInt16[]) || m_imgData.Image.GetType() == typeof(System.Byte[]))
                        {
                            ushort[] pixels = new ushort[m_imgData.Width * m_imgData.Height];
                            Array.Copy(imgData, pixels, m_imgData.Width * m_imgData.Height);
                            ds.WriteData(pixels, new int[] { 0, 0 }, new int[] { m_imgData.Width, m_imgData.Height });
                        }
                        else
                        {
                            short[] pixels = new short[m_imgData.Width * m_imgData.Height];
                            Array.Copy(imgData, pixels, m_imgData.Width * m_imgData.Height);
                            ds.WriteData(pixels, new int[] { 0, 0 }, new int[] { m_imgData.Width, m_imgData.Height });
                        }
                    }

                    //name = "Comments"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs); value = m_reportableSettings["Comments"];
                }
                catch (Exception ex)
                {
                    HandleExceptions(ex);
                }
                if (dsAttrs != null) { dsAttrs.Dispose(); }
                if (ds != null) { ds.Dispose(); }
                if (m_sdFile != null) { m_sdFile.Dispose(); }
            }

                return m_bRetVal;
        }


        #endregion hardware code

        #region utility functions
        private void GetAttributesFromDB()
        {
            string Name = "";
            string Value = "";
            string Location = "";
            if (m_appSettings.ContainsKey("Location")) { Location = m_appSettings["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.

            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();

            if(Id.Contains("X") || Id.Contains("Y"))
            {
                //retrieve camera settings from database
                m_db.RetrieveSettings(Id, "OpticalCamera", configParams);

                Name = "Camera Model"; configParams.TryGetValue("Model", out Value);
                m_reportableSettings.AddOrUpdate(Name, Value, (k, v) => Value);

                Name = "Camera Serial Number"; configParams.TryGetValue("SerialNumber", out Value);
                m_reportableSettings.AddOrUpdate(Name, Value, (k, v) => Value );

                Name = "Pixel Format"; configParams.TryGetValue("PixelFormat", out Value);
                m_reportableSettings.AddOrUpdate(Name, Value, (k, v) => Value);

                configParams = new ConcurrentDictionary<string, string>();
                //retrieve Navitar settings from database
                m_db.RetrieveSettings(Id, "Navitar", configParams);

                Name = "Objective Model"; configParams.TryGetValue("Model", out Value);
                m_reportableSettings.AddOrUpdate(Name, Value, (k, v) => Value);

                Name = "Objective Serial Number"; configParams.TryGetValue("SerialNumber", out Value);
                m_reportableSettings.AddOrUpdate(Name, Value, (k, v) => Value);

                configParams = new ConcurrentDictionary<string, string>();
                //retrieve illuminator settings from database
                m_db.RetrieveSettings(Location, "Illuminator", configParams);

                Name = "Illuminator Model"; configParams.TryGetValue("Model", out Value);
                m_reportableSettings.AddOrUpdate(Name, Value, (k, v) => Value);

                Name = "Illuminator Serial Number"; configParams.TryGetValue("SerialNumber", out Value);
                m_reportableSettings.AddOrUpdate(Name, Value, (k, v) => Value);
            }


            //get relevant information from the database
            //target id
        }

        private void StoreDatasetAttributesToDataset(ref SDAttributes dsAttrs)
        {
            string name = ""; 
            name = "DatasetId"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
            name = "Timestamp"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
        }
        private void StoreCameraAttributesToDataset(ref SDAttributes dsAttrs)
        {
            string name = "";
            name = "Image_Width"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
            name = "Image_Height"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
            name = "Camera Model"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
            name = "Camera Serial Number"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
            name = "Camera Binning"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);

            //X-Ray
            if (Id.Contains("Z"))
            {

            }
            //Optical
            else
            {
                name = "Gain"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
                name = "Exposure"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
                name = "Pixel Format"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
                name = "Illuminator Model"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
                name = "Illuminator Serial Number"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);     
                name = "Objective Model"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);
                name = "Objective Serial Number"; WriteAttribute(name, name, "string", m_reportableSettings, dsAttrs);

                if (Id.Contains("X"))
                {
                    name = "Zoom Motor Position"; WriteAttribute("Ch1Zoom", name, "string", m_reportableSettings, dsAttrs);
                    name = "Focus Motor Position"; WriteAttribute("Ch1Focus", name, "string", m_reportableSettings, dsAttrs);
                    name = "Brightness"; WriteAttribute("Ch1Brightness", name, "string", m_reportableSettings, dsAttrs);
                }
                else
                {
                    name = "Zoom Motor Position"; WriteAttribute("Ch2Zoom", name, "string", m_reportableSettings, dsAttrs);
                    name = "Focus Motor Position"; WriteAttribute("Ch2Focus", name, "string", m_reportableSettings, dsAttrs);
                    name = "Brightness"; WriteAttribute("Ch2Brightness", name, "string", m_reportableSettings, dsAttrs);
                }
            }
        }

        /// <summary>
        /// new set of image data available
        /// </summary>
        /// <param keyToRetrieve="imgData">the new image</param>
        private void ImageDataUpdated(object state)
        {
            ImageDataMessage msg = (ImageDataMessage)(state);
            if (this.Id.Equals(msg.Id)) // multiple axes are sending img data; 
            {
                lock (m_objLock)
                {
                    m_imgData = msg;
                }
            }
        }

        private void DataReportableSettingsUpdated(object state)
        {
            DataReportableSettings msg = (DataReportableSettings)state;
            lock (m_objLock)
            {
                try
                {
                    foreach (KeyValuePair<string, string> kvp in msg.ReportableSettings)
                    {
                        switch (kvp.Key)
                        {
                            default:
                                string tempVal = (kvp.Value == null) ? "" : kvp.Value.ToString();
                                m_reportableSettings.AddOrUpdate(kvp.Key, kvp.Value, (k, v) => tempVal);
                                break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param keyToRetrieve="keyToRetrieve"></param>
        /// <param keyToRetrieve="typeOfAttr"></param>
        /// <param keyToRetrieve="m_reportableSettings"></param>
        /// <param keyToRetrieve="dsAttrs"></param>
        private void WriteAttribute(string keyToRetrieve, string keyToWrite, string typeOfAttr, ConcurrentDictionary<string, string> m_reportableSettings, SDAttributes dsAttrs)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, " Writing attr " + keyToWrite);
            string value = "";
            try
            {
                m_reportableSettings.TryGetValue(keyToRetrieve, out value); // if key not found, value is null.
                if (dsAttrs == null) { } // skip writing the attribute; unknown cause sometimes makes dsAttrs null
                else
                {
                    switch (typeOfAttr)
                    {
                        case "string": 
                            if (value.Equals("")) dsAttrs.Set(keyToWrite, "  ");
                            else dsAttrs.Set(keyToWrite, value);
                            break;
                        case "single":
                            Single sValue = Convert.ToSingle(value);
                            dsAttrs.Set(keyToWrite, sValue);
                            break;
                        case "integer":
                            int iValue = Convert.ToInt32(value);
                            dsAttrs.Set(keyToWrite, iValue);
                            break;
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                CryoviewTools.LogMessage(LogLevel.Err, " Key not found for attr: " + keyToRetrieve);
                HandleExceptions(new Exception("Hdf dataset attr keyToRetrieve = " + keyToRetrieve + " Value = null ", ex));
            }
            catch (Exception ex) // hdf exceptions
            {
                CryoviewTools.LogMessage(LogLevel.Err, " Failed to write attr " + keyToWrite + " to hdf file " + Environment.NewLine +
                    ex.Message.ToString() + Environment.NewLine);
                if (ex.InnerException != null)
                    CryoviewTools.LogMessage(LogLevel.Err, "    ...   " + ex.InnerException.ToString() + Environment.NewLine);
                CryoviewTools.LogMessage(LogLevel.Err, " Set dataset  attr: " + keyToRetrieve + " / " + value);
                HandleExceptions(new Exception("Hdf dataset attr keyToRetrieve = " + keyToRetrieve + " Value = " + value, ex));
            }
        }

        /// <summary>
        /// Aspect-oriented programming
        /// Break down program logic into distinct parts, aka concerns, one of which is handling exceptions.
        /// </summary>
        /// <param keyToRetrieve="ex"></param>
        private void HandleExceptions(Exception ex)
        {
            if (ex.GetType() == typeof(InvalidCastException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised(" invalid cast: " + ex.Message);
            }
            else if (ex.GetType() == typeof(IndexOutOfRangeException))
            {
                CryoviewTools.LogMessage(LogLevel.Err, " --> " + ex.ToString());
                OnExceptionRaised(" index out of range: " + ex.Message);
            }
            else if (ex.GetType() == typeof(DirectoryNotFoundException))   // creating the hdf file dir, creating file, opening hdf file, etc
            {
                CryoviewTools.LogMessage(LogLevel.Err, " --> " + ex.ToString());
                OnExceptionRaised(" file io exception: " + ex.Message);
            }
            else if (ex.GetType() == typeof(IOException))   // creating the hdf file dir, creating file, opening hdf file, etc
            {
                CryoviewTools.LogMessage(LogLevel.Err, " --> " + ex.ToString());
                OnExceptionRaised(" file io exception: " + ex.Message);
            }
            else
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.ToString());
                OnExceptionRaised("Error: " + ex.Message);
            }
        }
        private void OnExceptionRaised(string msg)
        {
            if (ExceptionRaised != null)
            {
                CryoviewTools.LogMessage(LogLevel.Err, msg);
                ExceptionRaised(msg);
            }
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
