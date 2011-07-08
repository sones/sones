using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace sones.Library.SystemInformation.Intern
{
    internal sealed class MonoSystemInformation: SystemInformation
    {


        public override ulong GetFreeSpaceForPath(string myPath)
        {
            return ulong.MaxValue;
        }

        public override ulong GetAvailableMainMemory()
        {
            //an estimation
            return GetTotalMainMemory() - GetMainMemoryConsumption();
        }

        public override ulong GetTotalMainMemory()
        {
            var pc = new PerformanceCounter("Mono Memory", "Total Physical Memory");
            return (ulong)pc.RawValue;
        }

        public override ulong GetMainMemoryConsumption()
        {
            Process.GetCurrentProcess().Refresh();
            return (ulong)Process.GetCurrentProcess().WorkingSet64;
        }
    }
}
