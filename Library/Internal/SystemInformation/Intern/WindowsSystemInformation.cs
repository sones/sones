#if !__MonoCS__

using System.Diagnostics;
using Microsoft.VisualBasic.Devices;

namespace sones.Library.SystemInformation.Intern
{
    /// <summary>
    /// This class provides specific plattform informations like total and available main memory.
    /// </summary>
    internal sealed class WindowsSystemInformation : SystemInformation
    {
        private ComputerInfo _info;

        /// <summary>
        /// Constructor
        /// </summary>
        internal protected WindowsSystemInformation()
        {
            _info = new ComputerInfo();
        }

        /// <summary>
        /// Reads out the free space for the given path.
        /// </summary>
        public override ulong GetFreeSpaceForPath(string myPath)
        {
            return ulong.MaxValue;
        }

        /// <summary>
        /// Reads out the available main memory.
        /// </summary>
        public override ulong GetAvailableMainMemory()
        {
            return _info.AvailablePhysicalMemory;
        }

        /// <summary>
        /// Reads out the total main memory.
        /// </summary>
        public override ulong GetTotalMainMemory()
        {
            return _info.TotalPhysicalMemory;
        }

        /// <summary>
        /// Reads out the consumed main memory.
        /// </summary>
        public override ulong GetMainMemoryConsumption()
        {
            Process.GetCurrentProcess().Refresh();
            return (ulong)Process.GetCurrentProcess().WorkingSet64;
        }
    }
}

#endif
