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

using System.Windows.Media;       // PixelFormats, Color
using System.Windows.Media.Imaging; // BitmapPalette; reqs ref to WindowsBase, PresentationCore

using Cryoview_ModuleMessages;
using Cryoview_Tools;   // logging
using LLE.Util; // logging
using Target_Filling;
using MCM_Interface;

namespace Target_Filling_ViewModel
{
    public class TargetFillingViewModel : BindableBase, IDisposable
    {
        #region delegates, events
        public delegate void DelExceptionRaised(string msg, bool ClosePopOutWindow);    // notification that something went wrong
        public event DelExceptionRaised ExceptionRaised;
        #endregion delegates, events

        #region backing vars
        private object m_ObjLock = new object();
        private TargetFilling m_tfilling = null;
        private MCMInterface m_MCM = null;
        private Thread m_targetFillThread = null;
        private bool m_UserCancelledTargetFill = false;
        private Single m_LSPrefillTargetTemp = 0.0f;
        private Single m_LSFillTargetTemp = 0.0f;
        private Single m_LSPostfillTargetTemp = 0.0f;
        private Single m_CFEPrefillTaretTemp = 0.0f;
        private Single m_CFEFillTargetTemp = 0.0f;
        private Single m_CFEPostfillTargetTemp = 0.0f;
        private Single m_TargetIceThickness = 0.0f;
        private Single m_PIDTempAdjust = 0.001f;
        private Single m_PIDLastDiff = 0.0f;
        private int m_ScrubTime = 0;
        private int m_NumScrubs = 0;
        private bool m_CanWithdrawValveConrol = false;

        private bool m_TestValveStatus = false;
        private Thread m_TestValveStatusThread = null;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public TargetFillingViewModel()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            m_tfilling = new TargetFilling();
            MCMObjectMessageEvent.Instance.Subscribe(GetMCMObject);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        /// <summary>
        /// Got here by user code.
        /// </summary>
        public void Dispose()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            Dispose(true);
            GC.SuppressFinalize(this);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        /// <summary>
        /// Got here either by user code or by garbage collector. If param false, then gc.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (disposing)
            {       // managed resources here
                m_tfilling.ExceptionRaised -= Filling_Exception_Raised;
                m_tfilling.UpdateGUI -= Update_GUI_Raised;
                m_TestValveStatus = false;
                if(m_TestValveStatusThread != null) { m_TestValveStatusThread.Join(); }
            }
            // unmanaged resources here
            {
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion ctors/dtors/dispose

        #region factory method
        #endregion factory method

        #region initialization
        public void Initialize()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            m_tfilling.ExceptionRaised += Filling_Exception_Raised;
            m_tfilling.UpdateGUI += Update_GUI_Raised;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        public Single LSPrefillTargetTemp { get; set; }
        public Single LSFillTargetTemp { get; set; }
        public Single LSPostfillTargetTemp { get; set; }
        public Single CFEPrefillTaretTemp { get; set; }
        public Single CFEFillTargetTemp { get; set; }
        public Single CFEPostfillTargetTemp { get; set; }
        public Single TargetIceThickness { get; set; }
        public int ExGasPressureMin { get; set; }
        public int ExGasPressureMax { get; set; }
        public int ScrubTime { get; set; }
        public int NumScrubs { get; set; }
        public string FillComments { get; set; }

        public int ScrubNumber { get; set; }
        public Single EstIceThickness
        {
            get { return m_tfilling.IceThickness; }
        }
        public Single Delta { get; set; }
        public string DTValve
        {
            get
            {
                if(m_MCM != null)
                {
                    if (m_MCM.ControlOverValves) { return "Cryoview Controlled"; }
                    else { return "MCM Controlled"; }
                }
                else { return "MCM Controlled"; }
            }
        }
        public string LeakDetection
        {
            get
            {
                if (m_MCM != null) 
                {
                    if (m_MCM.IsLeak) { return "Yes"; }
                    else { return "No"; }
                }
                else { return "No"; }
            }
        }
        public string LeakDetectionAtTarget
        {
            get
            {
                if (m_MCM != null)
                {
                    if (m_MCM.IsLeakAtTarget) { return "Yes"; }
                    else { return "No"; }
                }
                else { return "No"; }
            }
        }
        public string FillStatus { get; set; }

        public Valve_Status ValveToTarget 
        {
            get
            {
                if (m_MCM != null) { return m_MCM.ValveToTarget; }
                else { return Valve_Status.MCMCONTROLLED; }
            }
        }
        public Valve_Status ValveToDT
        {
            get
            {
                if (m_MCM != null) { return m_MCM.ValveToDT; }
                else { return Valve_Status.MCMCONTROLLED; }
            }
        }
        public Valve_Status ValveToUBed
        {
            get
            {
                if (m_MCM != null) { return m_MCM.ValveToUBed; }
                else { return Valve_Status.MCMCONTROLLED; }
            }
        }

        public BitmapSource TestImg1
        {
            get
            {
                return m_tfilling.bmp1;
            }
        }
        public BitmapSource TestImg2
        {
            get
            {
                return m_tfilling.bmp2;
            }
        }
        #endregion properties

        #region bindable properties
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        private void ScrubTarget()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            //if a leak is detected we will lose control over the valves
            m_CanWithdrawValveConrol = false;
            //start with all valves closed
            m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);     
            while(m_MCM.ControlOverValves && (m_MCM.ValveToTarget != Valve_Status.CLOSED || m_MCM.ValveToDT != Valve_Status.CLOSED || m_MCM.ValveToUBed != Valve_Status.CLOSED))
            {
                Thread.Sleep(500);
            }

            if (!m_UserCancelledTargetFill && m_MCM.ControlOverValves) 
            { 
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.OPEN, Valve_Status.CLOSED);
                while (m_MCM.ValveToDT != Valve_Status.OPEN && m_MCM.ControlOverValves)
                {
                    Thread.Sleep(500);
                }
            }
            if (!m_UserCancelledTargetFill && m_MCM.ControlOverValves)
            {
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);
                while (m_MCM.ValveToDT != Valve_Status.CLOSED && m_MCM.ControlOverValves)
                {
                    Thread.Sleep(500);
                }
            }
            if (!m_UserCancelledTargetFill && m_MCM.ControlOverValves)
            {
                m_MCM.SetValveStatus(Valve_Status.OPEN, Valve_Status.CLOSED, Valve_Status.CLOSED);
                while (m_MCM.ValveToTarget != Valve_Status.OPEN && m_MCM.ControlOverValves)
                {
                    Thread.Sleep(500);
                }
            }
            if (!m_UserCancelledTargetFill && m_MCM.ControlOverValves)
            {
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);
                while (m_MCM.ValveToTarget != Valve_Status.CLOSED && m_MCM.ControlOverValves)
                {
                    Thread.Sleep(500);
                } 
            }

            if (!m_MCM.ControlOverValves) 
            { 
                OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                return; 
            }

            int scrubTimeElapsed = 0;
            while (!m_UserCancelledTargetFill && m_MCM.ControlOverValves && scrubTimeElapsed < (m_ScrubTime * 60))
            {
                Thread.Sleep(1000);
                scrubTimeElapsed++;
            }

            if (!m_MCM.ControlOverValves) 
            { 
                OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                return; 
            }
            else if (m_UserCancelledTargetFill)
            {
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);
                while (m_MCM.ControlOverValves && (m_MCM.ValveToTarget != Valve_Status.CLOSED || m_MCM.ValveToDT != Valve_Status.CLOSED || m_MCM.ValveToUBed != Valve_Status.CLOSED))
                {
                    Thread.Sleep(500);
                }

                m_CanWithdrawValveConrol = true;
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                return;
            }
            else 
            {
                m_MCM.SetValveStatus(Valve_Status.OPEN, Valve_Status.CLOSED, Valve_Status.OPEN);
                while (m_MCM.ControlOverValves && (m_MCM.ValveToTarget != Valve_Status.OPEN || m_MCM.ValveToUBed != Valve_Status.OPEN))
                {
                    Thread.Sleep(500);
                }
                
                Thread.Sleep(60000); // 60 seconds. May change this later depending on how long the wait should be.
            }

            if (m_MCM.ControlOverValves) 
            {
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);
                while (m_MCM.ControlOverValves && (m_MCM.ValveToTarget != Valve_Status.CLOSED || m_MCM.ValveToDT != Valve_Status.CLOSED || m_MCM.ValveToUBed != Valve_Status.CLOSED))
                {
                    Thread.Sleep(500);
                }
            }
            else 
            { 
                OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ..."); 
                return; 
            }
            m_CanWithdrawValveConrol = true;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        private void Target_Fill()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            //set to starting temp for scrubbing
            if (m_UserCancelledTargetFill) { CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ..."); return; }
            m_MCM.SetLSTemp(m_LSPrefillTargetTemp);
            m_MCM.SetCFETemp(m_CFEPrefillTaretTemp);
            bool IsAtStartingTemp = false;
            while (!IsAtStartingTemp && !m_UserCancelledTargetFill)
            {
                Thread.Sleep(10000);
                Single LSTemp = m_MCM.GetLSTemp();
                Single CFETemp = m_MCM.GetCFETemp();
                if (Math.Abs(CFETemp - m_CFEPrefillTaretTemp) < 0.001 && Math.Abs(LSTemp - m_LSPrefillTargetTemp) < 0.001) { IsAtStartingTemp = true; }
            }

            //Target scrubbing sequence
            for(int i = 0; i < m_NumScrubs; i++)
            {
                if (m_UserCancelledTargetFill) { CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ..."); return; }
                ScrubNumber = i + 1;
                RaisePropertyChanged(nameof(ScrubNumber));
                ScrubTarget();
            }

            if (m_UserCancelledTargetFill) { CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ..."); return; }
            if (!m_MCM.ControlOverValves) 
            { 
                OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ..."); 
                return; 
            }

            //set to starting temp for filling
            m_MCM.SetLSTemp(m_LSFillTargetTemp);
            m_MCM.SetCFETemp(m_CFEFillTargetTemp);
            IsAtStartingTemp = false;
            while (!IsAtStartingTemp && !m_UserCancelledTargetFill)
            {
                Thread.Sleep(10000);
                Single LSTemp = m_MCM.GetLSTemp();
                Single CFETemp = m_MCM.GetCFETemp();
                if (Math.Abs(CFETemp - m_CFEFillTargetTemp) < 0.001 && Math.Abs(LSTemp - m_LSFillTargetTemp) < 0.001) { IsAtStartingTemp = true; }             
            }

            //target filling sequence
            m_tfilling.IsFilling = true;
            if (m_UserCancelledTargetFill) 
            { 
                m_tfilling.IsFilling = false;
                CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ..."); 
                return; 
            }

            m_CanWithdrawValveConrol = false;
            if (m_MCM.ControlOverValves) 
            {
                m_MCM.SetValveStatus(Valve_Status.OPEN, Valve_Status.OPEN, Valve_Status.CLOSED);
                while(m_MCM.ValveToTarget != Valve_Status.OPEN || m_MCM.ValveToDT != Valve_Status.OPEN)
                {
                    Thread.Sleep(500);
                }
            }
            else
            {
                OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                return;
            }

            Thread.Sleep(10000);
            if (m_UserCancelledTargetFill) 
            { 
                CancelFill();
                CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ..."); 
                return; 
            }

            Single adjustedTargetIceThickness = m_TargetIceThickness;
            while(Math.Abs(EstIceThickness - m_TargetIceThickness) > 2.0 && !m_UserCancelledTargetFill && m_MCM.ControlOverValves)
            {
                //fill to target fill level
                while (Math.Abs(EstIceThickness - adjustedTargetIceThickness) > 2.0 && !m_UserCancelledTargetFill && m_MCM.ControlOverValves)
                {
                    //take image
                    CommandMessageEvent.Instance.Publish(new CommandMessage
                    {
                        ID = "xray",
                        Command = "Take Image"
                    });
                    Thread.Sleep(5000); //give some time to take the image, process it, and update the estimated ice thickness
                    PID_Loop(adjustedTargetIceThickness - EstIceThickness);
                    Thread.Sleep(10000);
                }

                if (!m_MCM.ControlOverValves) 
                { 
                    OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                    CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                    CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                    return; 
                }
                if (m_UserCancelledTargetFill) 
                { 
                    CancelFill();
                    CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ..."); 
                    return; 
                }

                //form the ice plug
                m_MCM.SetCFETemp(m_CFEPostfillTargetTemp);
                IsAtStartingTemp = false;
                while (!IsAtStartingTemp && !m_UserCancelledTargetFill)
                {
                    Thread.Sleep(10000);
                    Single CFETemp = m_MCM.GetCFETemp();
                    if (Math.Abs(CFETemp - m_CFEPostfillTargetTemp) < 0.001 ) { IsAtStartingTemp = true; }
                }

                if (m_UserCancelledTargetFill) 
                {
                    //melt the ice plug
                    m_MCM.SetCFETemp(30);
                    IsAtStartingTemp = false;
                    while (!IsAtStartingTemp)
                    {
                        Thread.Sleep(10000);
                        Single CFETemp = m_MCM.GetCFETemp();
                        if (Math.Abs(CFETemp - 30) < 0.001) { IsAtStartingTemp = true; }
                    }

                    //cancel the fill
                    CancelFill();
                    CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ...");
                    return; 
                }
                if (!m_MCM.ControlOverValves)
                {
                    OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                    //melt the ice plug
                    m_MCM.SetCFETemp(m_CFEFillTargetTemp);
                    IsAtStartingTemp = false;
                    while (!IsAtStartingTemp)
                    {
                        Thread.Sleep(10000);
                        Single CFETemp = m_MCM.GetCFETemp();
                        if (Math.Abs(CFETemp - m_CFEFillTargetTemp) < 0.001) { IsAtStartingTemp = true; }
                    }
                    CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                    CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                    return;
                }

                //measure final fill lvl
                CommandMessageEvent.Instance.Publish(new CommandMessage
                {
                    ID = "xray",
                    Command = "Take Image"
                });

                //if not at desired fill level, make adjustment and try again
                Single diff = EstIceThickness - adjustedTargetIceThickness;
                if (diff > 2.0) 
                { 
                    adjustedTargetIceThickness -= diff;
                    m_MCM.SetCFETemp(m_CFEFillTargetTemp);
                    IsAtStartingTemp = false;
                    while (!IsAtStartingTemp)
                    {
                        Thread.Sleep(10000);
                        Single CFETemp = m_MCM.GetCFETemp();
                        if (Math.Abs(CFETemp - m_CFEFillTargetTemp) < 0.001) { IsAtStartingTemp = true; }
                    }
                }
                else if(diff < -2.0) 
                { 
                    adjustedTargetIceThickness += (diff * -1);
                    m_MCM.SetCFETemp(m_CFEFillTargetTemp);
                    IsAtStartingTemp = false;
                    while (!IsAtStartingTemp)
                    {
                        Thread.Sleep(10000);
                        Single CFETemp = m_MCM.GetCFETemp();
                        if (Math.Abs(CFETemp - m_CFEFillTargetTemp) < 0.001) { IsAtStartingTemp = true; }
                    }
                }
            }

            if (m_UserCancelledTargetFill)
            {
                //melt the ice plug
                m_MCM.SetCFETemp(30);
                IsAtStartingTemp = false;
                while (!IsAtStartingTemp)
                {
                    Thread.Sleep(10000);
                    Single CFETemp = m_MCM.GetCFETemp();
                    if (Math.Abs(CFETemp - 30) < 0.001) { IsAtStartingTemp = true; }
                }

                //cancel the fill
                CancelFill();
                CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ...");
                return;
            }

            if (!m_MCM.ControlOverValves)
            {
                OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true);
                //melt the ice plug
                m_MCM.SetCFETemp(m_CFEFillTargetTemp);
                IsAtStartingTemp = false;
                while (!IsAtStartingTemp)
                {
                    Thread.Sleep(10000);
                    Single CFETemp = m_MCM.GetCFETemp();
                    if (Math.Abs(CFETemp - m_CFEFillTargetTemp) < 0.001) { IsAtStartingTemp = true; }
                }
                CryoviewTools.LogMessage(LogLevel.Info, "MCM has taken back control over valves. Fill aborted.");
                CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                return;
            }

            m_tfilling.IsFilling = false;
            if (m_MCM.ControlOverValves) 
            { 
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED); 
                while(m_MCM.ValveToTarget != Valve_Status.CLOSED || m_MCM.ValveToUBed != Valve_Status.CLOSED || m_MCM.ValveToDT != Valve_Status.CLOSED)
                {
                    Thread.Sleep(500);
                }
            }

            //target is filled, withdraw valve control
            m_MCM.RequestValveControl(false);
            OnExceptionsRaised("Target is filled.", true);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        private void PID_Loop(Single diff)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            Single AdjustedTemp;
            if(Math.Abs(diff - m_PIDLastDiff) == 0)
            {
                m_PIDTempAdjust += 0.001f;
            }
            else if(( m_PIDLastDiff > 0 && diff < 0) || (m_PIDLastDiff < 0 && diff > 0))
            {
                m_PIDTempAdjust -= 0.001f;
            }
            m_PIDLastDiff = diff;

            if (diff > 0) //underfilled
            {
                AdjustedTemp = m_MCM.LSSetPoint - m_PIDTempAdjust;
            }
            else //overfilled
            {
                AdjustedTemp = m_MCM.LSSetPoint + m_PIDTempAdjust;        
            }

            m_MCM.SetLSTemp(AdjustedTemp);
            bool AtTemp = false;
            while (!AtTemp)
            {
                Single CurrentTemp = m_MCM.GetLSTemp();
                if (Math.Abs(CurrentTemp - AdjustedTemp) <= 0.001f) { AtTemp = true; }
                Thread.Sleep(1000);
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion algorithm code

        #region hardware code
        #endregion hardware code

        #region utility functions
        private void CancelFill()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            m_tfilling.IsFilling = false;         
            if (m_MCM.ControlOverValves) 
            {
                //close valve to DT and open valve to UBed
                m_MCM.SetValveStatus(Valve_Status.OPEN, Valve_Status.CLOSED, Valve_Status.OPEN); 
            }

            Thread.Sleep(60000); // 60 seconds. May change this later depending on how long the wait should be.

            //Close valves
            if (m_MCM.ControlOverValves) 
            {
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);
                while (m_MCM.ValveToTarget != Valve_Status.CLOSED || m_MCM.ValveToUBed != Valve_Status.CLOSED || m_MCM.ValveToDT != Valve_Status.CLOSED)
                {
                    Thread.Sleep(500);
                }
            }

            if (!m_MCM.ControlOverValves) { OnExceptionsRaised("MCM has taken back control over valves. Fill aborted.", true); return; }
            m_CanWithdrawValveConrol = true;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        public void Start_Target_Fill()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            //for testing use only
            //TestMCM(); return;
            //TestMCM2(); return;

            //Check all neccessary entries have been filled in by the user
            //set private variables to starting values to prevent being overwritten mid-fill
            m_UserCancelledTargetFill = false;
            if(LSPrefillTargetTemp <= 0) { OnExceptionsRaised("Layering Sphere prefill tempurature not set", true); return; }
            m_LSPrefillTargetTemp = LSPrefillTargetTemp;
            if (LSFillTargetTemp <= 0) { OnExceptionsRaised("Layering Sphere filling start tempurature not set", true); return; }
            m_LSFillTargetTemp = LSFillTargetTemp;
            if(LSPostfillTargetTemp <= 0) { OnExceptionsRaised("Layering Sphere postfill tempurature not set", true); return; }
            m_LSPostfillTargetTemp = LSPostfillTargetTemp;
            if(CFEPrefillTaretTemp <= 0) { OnExceptionsRaised("Cold Finger Extension prefill tempurature not set", true); return; }
            m_CFEPrefillTaretTemp = CFEPrefillTaretTemp;
            if(CFEFillTargetTemp <= 0) { OnExceptionsRaised("Cold Finger Extension filling start tempurature not set", true); return; }
            m_CFEFillTargetTemp = CFEFillTargetTemp;
            if(CFEPostfillTargetTemp <= 0) { OnExceptionsRaised("Cold Finger Extension postfill tempurature not set", true); return; }
            m_CFEPostfillTargetTemp = CFEPostfillTargetTemp;
            if(TargetIceThickness <= 0) { OnExceptionsRaised("Target ice thickness not set", true); return; }
            m_TargetIceThickness = TargetIceThickness;
            if(ScrubTime < 1) { OnExceptionsRaised("H scrub time not set", true); return; }
            m_ScrubTime = ScrubTime;
            if(NumScrubs < 1) { OnExceptionsRaised("Number of H scrubs not set", true); return; }
            m_NumScrubs = NumScrubs;

            if (m_UserCancelledTargetFill) { CryoviewTools.LogMessage(LogLevel.Debug, "User Cancelled. Exiting ..."); return; }
            lock (m_ObjLock)
            {
                bool success = m_MCM.RequestValveControl(true);
                if (!success) { OnExceptionsRaised("Unable to aquire control over valves from MCM", true); return; }
            }
            m_CanWithdrawValveConrol = true;

            m_targetFillThread = new Thread(new ThreadStart(Target_Fill));
            m_targetFillThread.Start();
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        public void Stop_Target_Fill()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            m_UserCancelledTargetFill = true;
            m_tfilling.IsFilling = false;          

                    //Testing purposes only
                    m_TestValveStatus = false;
                    if (m_TestValveStatusThread != null) { m_TestValveStatusThread.Join(); }

            while (!m_CanWithdrawValveConrol && m_MCM.ControlOverValves)
            {
                //need to wait until it is safe to withdraw control over valves
                Thread.Sleep(1000);
            }
            //check we still have control
            lock (m_ObjLock)
            {
                if (m_MCM.ControlOverValves) { m_MCM.RequestValveControl(false); }
            }
            m_CanWithdrawValveConrol = false;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        public void Get_Valve_Status()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            RaisePropertyChanged(nameof(LeakDetection));
            RaisePropertyChanged(nameof(LeakDetectionAtTarget));
            RaisePropertyChanged(nameof(DTValve));
            RaisePropertyChanged(nameof(ValveToTarget));
            RaisePropertyChanged(nameof(ValveToDT));
            RaisePropertyChanged(nameof(ValveToUBed));
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void GetMCMObject(object State)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            MCMObjectMessage msg = (MCMObjectMessage)State;
            m_MCM = msg.MCM;
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        private void OnExceptionsRaised(string msg, bool b)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (ExceptionRaised != null)
            {
                ExceptionRaised(msg, b);
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion utility functions

        #region event sinks
        private void Filling_Exception_Raised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            OnExceptionsRaised(msg, false);
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void Update_GUI_Raised()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            RaisePropertyChanged(nameof(TestImg1));
            RaisePropertyChanged(nameof(TestImg2));
            RaisePropertyChanged(nameof(EstIceThickness));
            RaisePropertyChanged(nameof(LeakDetection));
            RaisePropertyChanged(nameof(LeakDetectionAtTarget));
            RaisePropertyChanged(nameof(DTValve));
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion event sinks

        private void TestMCM()
        {
            bool success = false;
            while (!success)
            {
                success = m_MCM.RequestValveControl(true);
                if (!success) { OnExceptionsRaised("failed to take control of valves", false); }
                else { OnExceptionsRaised("Control over valves granted", false); }
                Thread.Sleep(2000);
            }

            m_MCM.SetValveStatus(Valve_Status.OPEN, Valve_Status.OPEN, Valve_Status.OPEN);

            while (m_MCM.ValveToTarget != Valve_Status.OPEN || m_MCM.ValveToDT != Valve_Status.OPEN || m_MCM.ValveToUBed != Valve_Status.OPEN)
            {
                Thread.Sleep(1000);
            }

            OnExceptionsRaised("All valves open", false);
            Thread.Sleep(2000);

            m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);

            while (m_MCM.ValveToTarget != Valve_Status.CLOSED || m_MCM.ValveToDT != Valve_Status.CLOSED || m_MCM.ValveToUBed != Valve_Status.CLOSED)
            {
                Thread.Sleep(1000);
            }

            OnExceptionsRaised("All valves closed", false);
            Thread.Sleep(2000);

            while (m_MCM.ControlOverValves)
            {
                Thread.Sleep(1000);
            }

            OnExceptionsRaised("Lost control over valves", false);
            Thread.Sleep(5000);

            success = false;
            while (!success)
            {
                success = m_MCM.RequestValveControl(true);
                Thread.Sleep(2000);
            }

            success = false;
            while (!success)
            {
                success = m_MCM.RequestValveControl(false);
                if (!success) { OnExceptionsRaised("failed to relinquish control of valves", false); }
                else { OnExceptionsRaised("Control over valves returned to MCM", false); }
                Thread.Sleep(2000);
            }
        }

        //rewrite this to get : min, max, and average open & close times out of 10 cycles.
        private void TestMCM2()
        {
            TimeSpan averageOpenTime = TimeSpan.Zero;
            TimeSpan averageCloseTime = TimeSpan.Zero;
            TimeSpan minTimeOpen = TimeSpan.MaxValue;
            TimeSpan minTimeClose = TimeSpan.MaxValue;
            TimeSpan maxTimeOpen = TimeSpan.MinValue;
            TimeSpan maxTimeClose = TimeSpan.MinValue;
            string msg = string.Empty;

            //request control of valves
            m_MCM.RequestValveControl(true);

            for(int i = 0; i < 1; i++)
            {
                DateTime time1;
                DateTime time2;
                TimeSpan timer;

                //write separate MCM code for requesting a valve open, just once, w/out checking for success
                m_MCM.SetValveStatus(Valve_Status.OPEN, Valve_Status.CLOSED, Valve_Status.CLOSED);

                //start timer
                time1 = DateTime.Now;

                //check is valve open repeatedly until valve returns as open
                while (m_MCM.ValveToTarget != Valve_Status.OPEN)
                {
                    Thread.Sleep(200);
                }

                //stop timer
                time2 = DateTime.Now;
                timer = time2 - time1;
                averageOpenTime += timer;
                if(minTimeOpen > timer) { minTimeOpen = timer; }
                if(maxTimeOpen < timer) { maxTimeOpen = timer; }

                //print time to screen & log file
                msg = "Time to open valve: " + timer.ToString();
                CryoviewTools.LogMessage(LogLevel.Info, msg);

                //close the valve using same new MCM method as to open
                m_MCM.SetValveStatus(Valve_Status.CLOSED, Valve_Status.CLOSED, Valve_Status.CLOSED);

                //start timer 
                time1 = DateTime.Now;

                //check is valve closed repeatedly until valve returns as closed
                while (m_MCM.ValveToTarget != Valve_Status.CLOSED)
                {
                    Thread.Sleep(200);
                }

                //stop timer
                time2 = DateTime.Now;
                timer = time2 - time1;
                averageCloseTime += timer;
                if (minTimeClose > timer) { minTimeClose = timer; }
                if (maxTimeClose < timer) { maxTimeClose = timer; }

                //print time to screen & log file
                msg = "Time to close valve: " + timer.ToString();
                CryoviewTools.LogMessage(LogLevel.Info, msg);
            }

            //relinquish control of valves
            m_MCM.RequestValveControl(false);

            //averageOpenTime= new TimeSpan(averageOpenTime.Ticks / 10);
            //averageCloseTime = new TimeSpan(averageCloseTime.Ticks / 10);

            msg = "Average time to open valve: " + averageOpenTime.ToString();
            CryoviewTools.LogMessage(LogLevel.Info, msg);
            msg = "Minimum time to open valve: " + minTimeOpen.ToString();
            CryoviewTools.LogMessage(LogLevel.Info, msg);
            msg = "Maximum time to open valve: " + maxTimeOpen.ToString();
            CryoviewTools.LogMessage(LogLevel.Info, msg);
            msg = "Average time to close valve: " + averageCloseTime.ToString();
            CryoviewTools.LogMessage(LogLevel.Info, msg);
            msg = "Minimum time to close valve: " + minTimeClose.ToString();
            CryoviewTools.LogMessage(LogLevel.Info, msg);
            msg = "Maximum time to close valve: " + maxTimeClose.ToString();
            CryoviewTools.LogMessage(LogLevel.Info, msg);

            OnExceptionsRaised( "Average Open Time: " + averageOpenTime.ToString() 
                + "\n Minimum Open Time: " + minTimeOpen.ToString()
                + "\n Maximum Open Time: " + maxTimeOpen.ToString()
                + "\n Average Close Time: " + averageCloseTime.ToString()
                + "\n Minimum Close Time: " + minTimeClose.ToString()
                + "\n Maximum Close Time: " + maxTimeClose.ToString(), false);

        }
    }
}
