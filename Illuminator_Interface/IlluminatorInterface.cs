using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Illuminator_Interface
{
    public delegate void DelExceptionRaised(string msg);
    public interface IlluminatorInterface
    {
        event DelExceptionRaised ExceptionRaised;
        void Dispose();
        bool Initialize();

        Single CH1DiodeBrightness { get; set; }
        Single CH2DiodeBrightness { get; set; }
        Single CH1PulseDurration { get; set; }
        Single CH2PulseDurration { get; set; }
    }
}
