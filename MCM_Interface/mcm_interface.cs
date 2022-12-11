using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCM_Interface
{
    public delegate void DelExceptionRaised(string msg);
    public delegate void DelConnectionLost(string msg);
    public delegate void DelFailedConnection();
    public enum Valve_Status { OPEN, CLOSED, MCMCONTROLLED }
    public enum Valve_ID { ToTarget, ToDT, ToUBed}

    public interface MCMInterface
    {
        event DelExceptionRaised ExceptionRaised;
        event DelConnectionLost ConnectionLost;
   
        void Dispose();
        bool Initialize();
        void Connect();
        void SetLSTemp(Single temp);
        void SetCFETemp(Single temp);
        Single GetLSTemp();
        Single GetCFETemp();
        bool RequestValveControl(bool control);
        bool SetValveStatus(Valve_Status valve2Target, Valve_Status valve2DT, Valve_Status valve2UBed);

        Single LSSetPoint { get; }
        Single CFESetPoint { get; }
        Valve_Status ValveToTarget { get; }
        Valve_Status ValveToDT { get; }
        Valve_Status ValveToUBed { get; }
        bool ControlOverValves { get; }
        bool IsLeak { get; }
        bool IsLeakAtTarget { get; }
    }
}
