using System;

namespace ICryoview
{
    /// <summary>
    /// This interface provides the specs necessary for an application,
    /// which implements the Managed Extensbility Framework, to 
    /// load those modules that implement it dynamically at run-time.
    /// 
    /// The application specifies that it will import components that implement this interface.
    /// The component specifies that it will satisfy - export - this interface.
    /// </summary>
    public interface ICryoviewWindow
    {
        /// <summary>
        /// The way to access those components that implement this interface
        /// </summary>
        ICryoviewWindow Window {get;}

        /// <summary>
        /// Call on the module to close itself.
        /// </summary>
        void Close();

        /// <summary>
        /// Human-readable name
        /// </summary>
        String ServiceName { get; }
    }
}
