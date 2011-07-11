#if __MonoCS__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace sones.Library.SystemInformation.Intern
{
    /// <summary>
    /// This class provides specific plattform informations like total and available main memory.
    /// </summary>
    internal sealed class MonoSystemInformation : SystemInformation
    {
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
            //an estimation
            return GetTotalMainMemory() - GetMainMemoryConsumption();
        }

        /// <summary>
        /// Reads out the total main memory.
        /// </summary>
        public override ulong GetTotalMainMemory()
        {
            var pc = new PerformanceCounter("Mono Memory", "Total Physical Memory");
            return (ulong)pc.RawValue;
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