using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInformation
{
    internal class MonoSystemInformation: SystemInformation
    {
        public override long GetFreeSpaceForPath(string myPath)
        {
            throw new NotImplementedException();
        }

        public override long GetAvailableMainMemory()
        {
            throw new NotImplementedException();
        }

        public override long GetTotalMainMemory()
        {
            throw new NotImplementedException();
        }
    }
}
