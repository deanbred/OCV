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
using DB_Model;
using HDF_File_Model;

namespace HDF_ViewModel
{
    public class HDFViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private DBModel m_db = null;
        private string m_FileSaveLocation = "";
        private string m_OV1FileName = "";
        private string m_OV2FileName = "";
        private string m_XRVFileName = "";
        private string m_OV1Comments = "";
        private string m_OV2Comments = "";
        private string m_XRVComments = "";

        private HDFFileModel OV1HDFFileModel = null;
        private HDFFileModel OV2HDFFileModel = null;
        private HDFFileModel XRVHDFFileModel = null;
        #endregion backing vars

        #region enums
        public enum HDFFile { OpticalX, OpticalY, XRay}
        #endregion enums

        #region ctors/dtors/dispose
        public HDFViewModel()
        {
            CryoviewTools.LogMessage(LogLevel.Debug7, "Creating HDF view model");
            m_db = new DBModel();
            CryoviewTools.LogMessage(LogLevel.Debug7, "HDF view model created");
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
                OV1HDFFileModel.ExceptionRaised -= OV1HDFFile_ExceptionRaised;
                OV2HDFFileModel.ExceptionRaised -= OV2HDFFile_ExceptionRaised;
                XRVHDFFileModel.ExceptionRaised -= XRVHDFFile_ExceptionRaised;
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
            ConcurrentDictionary<string, string> m_appSettings = CryoviewTools.GetAppProperties();
            string Location = "";
            if (m_appSettings.ContainsKey("Location")) { Location = m_appSettings["Location"]; }
            else { Location = "1"; } //Location 1 is the default location for the production system.

            ConcurrentDictionary<string, string> configParams = new ConcurrentDictionary<string, string>();
            m_db.RetrieveSettings(Location, "HDFFile", configParams);
            string Value = "";
            configParams.TryGetValue("HDFDir", out Value);
            FileSaveLocation = Value;

            OV1HDFFileModel = new HDFFileModel(Location + "X");
            OV1HDFFileModel.HdfDir = FileSaveLocation;
            OV1HDFFileModel.ExceptionRaised += OV1HDFFile_ExceptionRaised;
            OV2HDFFileModel = new HDFFileModel(Location + "Y");
            OV2HDFFileModel.HdfDir = FileSaveLocation;
            OV2HDFFileModel.ExceptionRaised += OV2HDFFile_ExceptionRaised;
            XRVHDFFileModel = new HDFFileModel(Location + "Z");
            XRVHDFFileModel.HdfDir = FileSaveLocation;
            XRVHDFFileModel.ExceptionRaised += XRVHDFFile_ExceptionRaised;
            PushDataMessageEvent.Instance.Publish(new PushDataMessage());
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
        public string FileID { get; set; }
        public string FileSaveLocation
        {
            get
            {
                return m_FileSaveLocation;
            }
            set
            {
                SetProperty<string>(ref m_FileSaveLocation, value);
                RaisePropertyChanged(nameof(FileSaveLocation));
            }
        }
        public string OV1FileName
        {
            get 
            { 
                return m_OV1FileName; 
            }
            set
            {
                SetProperty<string>(ref m_OV1FileName, value);
                RaisePropertyChanged(nameof(OV1FileName));
            }
        }
        public string OV2FileName
        {
            get
            {
                return m_OV2FileName;
            }
            set
            {
                SetProperty<string>(ref m_OV2FileName, value);
                RaisePropertyChanged(nameof(OV2FileName));
            }
        }
        public string XRVFileName
        {
            get
            {
                return m_XRVFileName;
            }
            set
            {
                SetProperty<string>(ref m_XRVFileName, value);
                RaisePropertyChanged(nameof(XRVFileName));
            }
        }
        public string OV1Comments
        {
            get
            {
                return m_OV1Comments;
            }
            set
            {
                SetProperty<string>(ref m_OV1Comments, value);
                RaisePropertyChanged(nameof(OV1Comments));
            }
        }
        public string OV2Comments
        {
            get
            {
                return m_OV2Comments;
            }
            set
            {
                SetProperty<string>(ref m_OV2Comments, value);
                RaisePropertyChanged(nameof(OV2Comments));
            }
        }
        public string XRVComments
        {
            get
            {
                return m_XRVComments;
            }
            set
            {
                SetProperty<string>(ref m_XRVComments, value);
                RaisePropertyChanged(nameof(XRVComments));
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
        public void CreateHDFFile(HDFFile file)
        {
            if(FileID == null || FileID == "")
            {
                string fileID = "";
                m_db.RetrieveNextFileId(ref fileID); FileID = fileID;
            }
            switch (file)
            {
                case HDFFile.OpticalX:
                    if (OV1HDFFileModel.FileIsOpen)
                    {
                        //already have an open file.
                        OnExceptionRaised("HDF File is currently open");
                        return;
                    }
                    if(OV1HDFFileModel.FileID == "" || OV1HDFFileModel.FileID == FileID)
                    {
                        string fileID = "";
                        m_db.RetrieveNextFileId(ref fileID); FileID = fileID;
                    }
                    OV1HDFFileModel.FileID = FileID;
                    OV1HDFFileModel.CreateHDFFile();
                    OV1FileName = OV1HDFFileModel.FileName;
                    break;
                case HDFFile.OpticalY:
                    if (OV2HDFFileModel.FileIsOpen)
                    {
                        //already have an open file.
                        OnExceptionRaised("HDF File is currently open");
                        return;
                    }
                    if (OV2HDFFileModel.FileID == "" || OV2HDFFileModel.FileID == FileID)
                    {
                        string fileID = "";
                        m_db.RetrieveNextFileId(ref fileID); FileID = fileID;
                    }
                    OV2HDFFileModel.FileID = FileID;
                    OV2HDFFileModel.CreateHDFFile();
                    OV2FileName = OV2HDFFileModel.FileName;
                    break;
                case HDFFile.XRay:
                    if (XRVHDFFileModel.FileIsOpen)
                    {
                        //already have an open file.
                        OnExceptionRaised("HDF File is currently open");
                        return;
                    }
                    if (XRVHDFFileModel.FileID == "" || XRVHDFFileModel.FileID == FileID)
                    {
                        string fileID = "";
                        m_db.RetrieveNextFileId(ref fileID); FileID = fileID;
                    }
                    XRVHDFFileModel.FileID = FileID;
                    XRVHDFFileModel.CreateHDFFile();
                    XRVFileName = XRVHDFFileModel.FileName;
                    break;
            }
        }

        public void OpenHDFFile(HDFFile file)
        {
            bool m_bRetVal = false;
            switch (file)
            {
                case HDFFile.OpticalX:
                    if (OV1HDFFileModel.FileIsOpen) { m_bRetVal = true; }
                    else { m_bRetVal = OV1HDFFileModel.OpenHDFFile(); }
                    if (m_bRetVal) { OV1FileName = OV1HDFFileModel.FileName; }
                    break;
                case HDFFile.OpticalY:
                    if (OV2HDFFileModel.FileIsOpen) { m_bRetVal = true; }
                    else { m_bRetVal = OV2HDFFileModel.OpenHDFFile(); }
                    if (m_bRetVal) { OV2FileName = OV2HDFFileModel.FileName; }
                    break;
                case HDFFile.XRay:
                    if (XRVHDFFileModel.FileIsOpen) { m_bRetVal = true; }
                    else { m_bRetVal = XRVHDFFileModel.OpenHDFFile(); }
                    if (m_bRetVal) { XRVFileName = XRVHDFFileModel.FileName; }
                    break;
            }
        }

        public void CloseHDFFile(HDFFile file)
        {
            switch (file)
            {
                case HDFFile.OpticalX:
                    OV1HDFFileModel.CloseHDFFile();
                    OV1FileName = "";
                    break;
                case HDFFile.OpticalY:
                    OV2HDFFileModel.CloseHDFFile();
                    OV2FileName = "";
                    break;
                case HDFFile.XRay:
                    XRVHDFFileModel.CloseHDFFile();
                    XRVFileName = "";
                    break;
            }
        }

        public void SaveHDFFile(HDFFile file)
        {
            switch (file)
            {
                case HDFFile.OpticalX:
                    if (OV1HDFFileModel.FileIsOpen) { OV1HDFFileModel.StoreDataHDFFile(OV1HDFFileModel.DatasetCount + 1); }
                    break;
                case HDFFile.OpticalY:
                    if (OV2HDFFileModel.FileIsOpen) { OV2HDFFileModel.StoreDataHDFFile(OV2HDFFileModel.DatasetCount + 1); }
                    break;
                case HDFFile.XRay:
                    if (XRVHDFFileModel.FileIsOpen) { XRVHDFFileModel.StoreDataHDFFile(XRVHDFFileModel.DatasetCount + 1); }
                    break;
            }
        }
        #endregion hardware code

        #region utility functions
        private void OnExceptionRaised(string msg)
        {
            if(ExceptionRaised != null)
            {
                ExceptionRaised(msg);
            }
        }
        #endregion utility functions

        #region event sinks
        private void OV1HDFFile_ExceptionRaised(string msg)
        {
            OnExceptionRaised(msg);
        }
        private void OV2HDFFile_ExceptionRaised(string msg)
        {
            OnExceptionRaised(msg);
        }
        private void XRVHDFFile_ExceptionRaised(string msg)
        {
            OnExceptionRaised(msg);
        }
        #endregion event sinks
    }
}
