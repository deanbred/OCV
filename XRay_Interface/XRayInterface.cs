using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRay_Interface
{
    public delegate void DelExceptionRaised(string msg);
    public interface XRayInterface
    {
        event DelExceptionRaised ExceptionRaised;
        void Dispose();
        bool Initialize();
        void AcquireImage();

        int Gain { get; set; }
        int DigitalGain { get; set; }
        int IntegrationTime { get; set; }
        int Imgs2Average { get; set; }
    }
}
