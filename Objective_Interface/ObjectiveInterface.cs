using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objective_Interface
{
    public delegate void DelExceptionRaised(string msg);
    public interface ObjectiveInterface
    {
        event DelExceptionRaised ExceptionRaised;
        void Dispose();
        bool Initialize();

        int ZoomLimit { get; }
        int FocusLimit { get; }
        int ZoomCurrentPos { get; }
        int FocusCurrentPos { get; }
        int ZoomTargetPos { set; }
        int FocusTargetPos { set; }
    }
}
