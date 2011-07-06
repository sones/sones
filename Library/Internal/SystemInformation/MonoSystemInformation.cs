using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;

namespace SystemInformation
{
    internal class MonoSystemInformation: SystemInformation
    {

        private ComputerInfo _info;

        internal protected MonoSystemInformation()
        {
            _info = new ComputerInfo();
        }

        public override ulong GetFreeSpaceForPath(string myPath)
        {
            throw new NotImplementedException();
        }

        public override ulong GetAvailableMainMemory()
        {
            return _info.AvailablePhysicalMemory;
        }

        public override ulong GetTotalMainMemory()
        {
            return _info.TotalPhysicalMemory;
        }

        public override ulong GetMainMemoryConsumption()
        {
            Process.GetCurrentProcess().Refresh();
            return (ulong)Process.GetCurrentProcess().WorkingSet64;
        }
    }
}
