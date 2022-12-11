using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using MCM_Interface;
using Cryoview_Tools;   // logging
using LLE.Util;         // logging

namespace MCM_Physical_Model
{
    public class MCMPhysicalModel : IDisposable, MCMInterface
    {
        #region delegates, events
        public event DelExceptionRaised ExceptionRaised;
        public event DelConnectionLost ConnectionLost;
        #endregion delegates, events

        #region backing vars
        private object m_objLock = new object();
        private ushort m_timeout = 5000;
        private Socket SyncSocket;
        private byte[] SyncSocketBuffer = new byte[2048];
        private string IPAddr = "";
        private ushort Port = 0;
        private Single m_LSSetPoint = 0.0f;
        private Single m_CFESetPoint = 0.0f;
        private Valve_Status m_ValveToTarge = Valve_Status.MCMCONTROLLED;
        private Valve_Status m_ValveToDT = Valve_Status.MCMCONTROLLED;
        private Valve_Status m_ValveToUBed = Valve_Status.MCMCONTROLLED;
        private bool m_ControlOverValves = false;
        private bool m_IsLeak = false;
        private bool m_IsLeakAtTarget = false;
        private Thread m_monitorValvesThread = null;
        private bool isDisposing = false;
        #endregion backing vars

        #region enums
        #endregion enums

        #region ctors/dtors/dispose
        public MCMPhysicalModel(string ip, ushort port)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");            
            IPAddr = ip;
            Port = port;
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
                if (ControlOverValves) { RequestValveControl(false); }
                isDisposing = true;
                if(m_monitorValvesThread != null) { m_monitorValvesThread.Join(); }
                disconnect();
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
        public bool Initialize()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            bool m_bRetVal = true;
            connect(IPAddr, Port);
            if (SocketConnected())
            {
                try
                {
                    byte[] msg = Encoding.ASCII.GetBytes("170 32768 1 572 170");
                    SyncSocket.Send(msg);
                    int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                    if (bytesRec <= 0)
                    {
                        OnConnectionLost("Connection to the MCM lost.");
                    }
                    else { m_LSSetPoint = SyncSocketBuffer[2]; }

                    msg = Encoding.ASCII.GetBytes("170 32768 1 573 170");
                    SyncSocket.Send(msg);
                    bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                    if (bytesRec <= 0)
                    {
                        m_bRetVal = false;
                        OnConnectionLost("Connection to the MCM lost.");
                    }
                    else { m_CFESetPoint = SyncSocketBuffer[2]; }
                }
                catch (Exception ex)
                {
                    m_bRetVal = false;
                    OnConnectionLost(ex.Message);
                }
            }
            else
            {
                m_bRetVal = false;
                OnConnectionLost("No MCM connection.");
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return m_bRetVal;
        }
        #endregion initialization

        #region windows events
        #endregion windows events

        #region IDataErrorInfo
        #endregion IDataErrorInfo

        #region MEF
        #endregion MEF

        #region properties
        public Single LSSetPoint
        {
            get { return m_LSSetPoint; }
        }
        public Single CFESetPoint
        {
            get { return m_CFESetPoint; }
        }
        public Valve_Status ValveToTarget 
        {
            get { return m_ValveToTarge; }
        } 
        public Valve_Status ValveToDT 
        {
            get { return m_ValveToDT; }
        } 
        public Valve_Status ValveToUBed 
        { 
            get { return m_ValveToUBed; }
        } 
        public bool ControlOverValves 
        {
            get { return m_ControlOverValves; }
        } 
        public bool IsLeak
        {
            get { return m_IsLeak; }
        }
        public bool IsLeakAtTarget
        {
            get { return m_IsLeakAtTarget; }
        }
        #endregion properties

        #region bindable properties
        #endregion bindable properties

        #region dependency properties
        #endregion dependency properties

        #region ICommands
        #endregion ICommands

        #region algorithm code
        public void SetLSTemp(Single temp)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (SocketConnected())
            {
                try
                {
                    lock (m_objLock)
                    {
                        byte[] msg = Encoding.ASCII.GetBytes("170 0 1 773 " + temp.ToString("0.0000") + " 170");
                        SyncSocket.Send(msg);
                        int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                        if (bytesRec <= 0)
                        {
                            //try sending the message again
                            SyncSocket.Send(msg);
                        }
                    }
                    m_LSSetPoint = temp;
                }
                catch (Exception ex)
                {
                    OnExceptionRaised(ex.Message);
                }
            }
            else
            {
                OnConnectionLost("Layering Sphere temp not set, no connection.");
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        public void SetCFETemp(Single temp)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (SocketConnected())
            {
                try
                {
                    lock (m_objLock)
                    {
                        byte[] msg = Encoding.ASCII.GetBytes("170 0 1 774 " + temp.ToString("0.0000") + " 170");
                        SyncSocket.Send(msg);
                        int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                        if (bytesRec <= 0)
                        {
                            //try sending the message again
                            SyncSocket.Send(msg);
                        }
                    }
                    m_CFESetPoint = temp;
                }
                catch (Exception ex)
                {
                    OnExceptionRaised(ex.Message);
                }
            }
            else
            {
                OnConnectionLost("Cold Finger Extension temp not set, no connection.");
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        public Single GetLSTemp()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (SocketConnected())
            {
                try
                {
                    lock (m_objLock)
                    {
                        byte[] msg = Encoding.ASCII.GetBytes("170 32768 1 580 170");
                        SyncSocket.Send(msg);
                        int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                        if (bytesRec <= 0)
                        {
                            OnExceptionRaised("Unable to get tempurature for Layering Sphere, connection timeout.");
                        }
                        else
                        {
                            string[] reply = Encoding.ASCII.GetString(SyncSocketBuffer).Split(' ');
                            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                            return Convert.ToSingle(reply[2]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnExceptionRaised(ex.Message);
                }
            }
            else
            {
                OnConnectionLost("Unable to get tempurature for Layering Sphere, no connection.");
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return 0.0f;
        }
        public Single GetCFETemp()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (SocketConnected())
            {
                try
                {
                    lock (m_objLock)
                    {
                        byte[] msg = Encoding.ASCII.GetBytes("170 32768 1 581 170");
                        SyncSocket.Send(msg);
                        int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                        if (bytesRec <= 0)
                        {
                            OnExceptionRaised("Unable to get tempurature for Cold Finger Extension, connection timeout.");
                        }
                        else
                        {
                            string[] reply = Encoding.ASCII.GetString(SyncSocketBuffer).Split(' ');
                            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                            return Convert.ToSingle(reply[2]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnExceptionRaised(ex.Message);
                }
            }
            else
            {
                OnConnectionLost("Unable to get tempurature for Cold Finger Extension, no connection.");
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return 0.0f;
        }

        public bool RequestValveControl(bool control)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            bool success = false;
            if (SocketConnected())
            {
                if (control == true)
                {
                    char[] binary = checkControlStatus();
                    if (binary.Length < 8)
                    {
                        CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                        return success;
                    }

                    if (binary[7] == '0')
                    {
                        try
                        {
                            lock (m_objLock)
                            {
                                //request control over valves
                                byte[] msg = Encoding.ASCII.GetBytes("170 0 1 810 1 170");
                                SyncSocket.Send(msg);
                                int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                                if (bytesRec <= 0)
                                {
                                    OnExceptionRaised("Unable to request control over valves, connection timeout.");
                                }
                                else
                                {
                                    m_ControlOverValves = true;
                                    success = true;
                                    m_monitorValvesThread = new Thread(new ThreadStart(MonitorValves));
                                    m_monitorValvesThread.Start();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CryoviewTools.LogMessage(LogLevel.Err, ex.Message);
                            OnExceptionRaised(ex.Message);
                        }
                    }
                }
                else
                {
                    if (ControlOverValves == false) { CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ..."); return true; }

                    char[] binary = GetCurrentValveStatus();
                    bool mustCloseValves = false;
                    if (binary[3] == '1') { mustCloseValves = true; binary[3] = '0'; }
                    if (binary[2] == '1') { mustCloseValves = true; binary[2] = '0'; }
                    if (binary[1] == '1') { mustCloseValves = true; binary[1] = '0'; }

                    if (mustCloseValves)
                    {
                        int SetStatus = Convert.ToInt32(string.Concat(binary), 2);
                        setValveStatus(SetStatus);
                        Thread.Sleep(5000); //give the valves time to close

                        //check the valves closed
                        binary = GetCurrentValveStatus();
                        mustCloseValves = false;
                        if (binary[3] == '1') { mustCloseValves = true; binary[3] = '0'; }
                        if (binary[2] == '1') { mustCloseValves = true; binary[2] = '0'; }
                        if (binary[1] == '1') { mustCloseValves = true; binary[1] = '0'; }

                        if (mustCloseValves)
                        {
                            //retry to close valves
                            SetStatus = Convert.ToInt32(string.Concat(binary), 2);
                            setValveStatus(SetStatus);
                            Thread.Sleep(5000); //give the valves time to close
                            
                            //check the valves closed
                            binary = GetCurrentValveStatus();
                            mustCloseValves = false;
                            if (binary[3] == '1') { mustCloseValves = true; binary[3] = '0'; }
                            if (binary[2] == '1') { mustCloseValves = true; binary[2] = '0'; }
                            if (binary[1] == '1') { mustCloseValves = true; binary[1] = '0'; }

                            if (mustCloseValves) { OnExceptionRaised("Failed to close valves before returning control to the MCM."); }
                        }
                    }

                    try
                    {
                        lock (m_objLock)
                        {
                            byte[] msg = Encoding.ASCII.GetBytes("170 0 1 810 0 170");
                            SyncSocket.Send(msg);
                            int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                            if (bytesRec <= 0)
                            {
                                OnConnectionLost("Unable to return control over valves, connection lost.");
                            }
                            else
                            {
                                m_ControlOverValves = false;
                                success = true;
                                if (m_monitorValvesThread != null) { m_monitorValvesThread.Join(); }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CryoviewTools.LogMessage(LogLevel.Err, ex.Message);
                        OnExceptionRaised(ex.Message);
                    }
                }
            }
            else
            {
                OnConnectionLost("Unable to request/relinquish control over valves, no connection.");
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return success;
        }

        public bool SetValveStatus(Valve_Status valve2Target, Valve_Status valve2DT, Valve_Status valve2UBed)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (valve2Target == Valve_Status.MCMCONTROLLED ||
                valve2DT == Valve_Status.MCMCONTROLLED ||
                valve2UBed == Valve_Status.MCMCONTROLLED) { return false; }
            bool success = false;
            bool connected = SocketConnected();
            if (connected && ControlOverValves)
            {
                char[] setValveBinary = { '0', '0', '0', '0', '0', '0', '0', '0' };
                if(valve2UBed == Valve_Status.OPEN) { setValveBinary[1] = '1'; }
                if(ValveToDT == Valve_Status.OPEN) { setValveBinary[2] = '1'; }
                if(valve2Target == Valve_Status.OPEN) { setValveBinary[3] = '1'; }
                //get the current status of the valves
                char[] binary = GetCurrentValveStatus();
                if (binary.Length < 8)
                {
                    CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                    return success;
                }

                if (binary[1] == setValveBinary[1] && binary[2] == setValveBinary[2] && binary[3] == setValveBinary[3]) { success = true; }
                else
                {
                    int SetStatus = Convert.ToInt32(string.Concat(setValveBinary), 2);

                    //send the message to change the valve status
                    setValveStatus(SetStatus);
                    success = true;
                }
            }
            else if (!connected)
            {
                OnConnectionLost("Unable to open/close valve, connection lost.");
            }

            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return success;
        }
        #endregion algorithm code

        #region hardware code
        public void Connect()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");            
            if (IPAddr != "" && Port > 0) { connect(IPAddr, Port); }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void connect(string ip, ushort port)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            try
            {
                IPAddress _ip;
                if (IPAddress.TryParse(ip, out _ip) == false)
                {
                    IPHostEntry hst = Dns.GetHostEntry(ip);
                    ip = hst.AddressList[0].ToString();
                }

                SyncSocket = new Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                SyncSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                SyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, m_timeout);
                SyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, m_timeout);
                SyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
            }
            catch (Exception ex)
            {
                string msg ="Exception: " + ex.Message;
                if(ex.InnerException != null)
                {
                    msg = msg + " Inner Exception: " + ex.InnerException.Message;
                }
                OnExceptionRaised(msg);
                disconnect();
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        public void disconnect()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            
            if (SyncSocket != null)
            {
                try { SyncSocket.Shutdown(SocketShutdown.Both); }
                catch { }
                SyncSocket.Close();
                SyncSocket = null;
            }
            
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion hardware code

        #region utility functions
        private bool SocketConnected()
        {
            if(SyncSocket != null)
            {
                bool part1 = SyncSocket.Poll(1000, SelectMode.SelectRead);
                bool part2 = (SyncSocket.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }

            return false;
        }
        private char[] GetCurrentValveStatus()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            try
            {
                lock (m_objLock)
                {
                    byte[] msg = Encoding.ASCII.GetBytes("170 32768 1 659 170");
                    SyncSocket.Send(msg);
                    int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                    if (bytesRec <= 0)
                    {
                        OnExceptionRaised("Unable to get control status for valves, connection timeout.");
                    }
                    else
                    {
                        string[] reply = Encoding.ASCII.GetString(SyncSocketBuffer).Split(' ');
                        string[] tobinary = reply[2].Split('.');
                        char[] binary = toBinary(tobinary[0]);
                        CryoviewTools.LogMessage(LogLevel.Debug, "Valve status: " + binary[0].ToString() + binary[1].ToString() + binary[2].ToString()
                            + binary[3].ToString()
                            + binary[4].ToString()
                            + binary[5].ToString()
                            + binary[6].ToString()
                            + binary[7].ToString());
                        CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                        return binary;
                    }
                }
            }
            catch (Exception ex)
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.Message);
                OnExceptionRaised(ex.Message);
            }

            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return new char[1];
        }

        private void setValveStatus(int status)
        {
            try
            {
                lock (m_objLock)
                {
                    byte[] msg = Encoding.ASCII.GetBytes("170 0 1 811 " + status.ToString() + " 170");
                    SyncSocket.Send(msg);
                    int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                    if (bytesRec <= 0)
                    {
                        OnExceptionRaised("Unable to open/close valve, connection timeout.");
                    }
                }
            }
            catch(Exception ex)
            {
                CryoviewTools.LogMessage(LogLevel.Err, ex.Message);
                OnExceptionRaised(ex.Message);
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }

        private char[] checkControlStatus()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (SocketConnected())
            {
                try
                {
                    lock (m_objLock)
                    {
                        //Check if software still has control, the MCM has not detected a leak or taken control for any reason
                        byte[] msg = Encoding.ASCII.GetBytes("170 32768 1 660 170");
                        SyncSocket.Send(msg);
                        int bytesRec = SyncSocket.Receive(SyncSocketBuffer);
                        if (bytesRec <= 0)
                        {
                            OnConnectionLost("Unable to get control status for valves, connection lost.");
                        }
                        else
                        {
                            string[] reply = Encoding.ASCII.GetString(SyncSocketBuffer).Split(' ');
                            string[] tobinary = reply[2].Split('.');
                            char[] binary = toBinary(tobinary[0]);

                            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                            return binary;
                        }
                    }
                }
                catch(Exception ex)
                {
                    CryoviewTools.LogMessage(LogLevel.Err, ex.Message);
                    OnExceptionRaised(ex.Message);
                }
            }

            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            return new char[1];
        }

        private void MonitorValves()
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            while (ControlOverValves && !isDisposing)
            {
                char[] binary = checkControlStatus();
                if (binary.Length == 8 && binary[7] != '0')
                {
                    m_ValveToTarge = Valve_Status.MCMCONTROLLED;
                    m_ValveToDT = Valve_Status.MCMCONTROLLED;
                    m_ValveToUBed = Valve_Status.MCMCONTROLLED;
                    m_ControlOverValves = false;
                    if (binary[5] == '1') 
                    { 
                        m_IsLeakAtTarget = true; 
                        m_IsLeak = true; 
                        RequestValveControl(false); 
                    }
                    else if(binary[6] == '1' || binary[4] == '1') 
                    { 
                        m_IsLeak = true; 
                        RequestValveControl(false); 
                    } 
                    CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
                    Thread.Sleep(500);
                    return;
                }

                binary = GetCurrentValveStatus();
                if (binary.Length == 8)
                {
                    if (binary[3] == '0') { m_ValveToTarge = Valve_Status.CLOSED; }
                    else { m_ValveToTarge = Valve_Status.OPEN; }
                    if (binary[2] == '0') { m_ValveToDT = Valve_Status.CLOSED; }
                    else { m_ValveToDT = Valve_Status.OPEN; }
                    if (binary[1] == '0') { m_ValveToUBed = Valve_Status.CLOSED; }
                    else { m_ValveToUBed = Valve_Status.OPEN; }
                }

                Thread.Sleep(500); //was set to 10000, probably too long, setting to 1000 for testing
            }

            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
            Thread.Sleep(500);
        }

        private char[] toBinary(string val)
        {
            char[] binary = { '0', '0', '0', '0', '0', '0', '0', '0' };
            char[] nums = Convert.ToString(Convert.ToByte(val), 2).ToCharArray();
            for(int i = 0; i < nums.Length; i++)
            {
                binary[7 - i] = nums[nums.Length - (i + 1)];
            }

            return binary;
        }
        private void OnExceptionRaised(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (ExceptionRaised != null)
            {
                ExceptionRaised(msg);
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        private void OnConnectionLost(string msg)
        {
            CryoviewTools.LogMessage(LogLevel.Debug, "Entering ...");
            if (ConnectionLost != null)
            {
                ConnectionLost(msg);
            }
            CryoviewTools.LogMessage(LogLevel.Debug, "Exiting ...");
        }
        #endregion utility functions

        #region event sinks
        #endregion event sinks
    }
}
