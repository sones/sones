using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class UniqueID
    {
        private long _id = Int64.MinValue;

        public UniqueID(long myStartID = Int64.MinValue)
        {
            _id = myStartID;
        }

        public long GetNextID()
        {
            long result = Interlocked.Increment(ref _id);
            return result - 1;
        }

        public long ReserveIDs(long myIDCount)
        {
            long result = Interlocked.Add(ref _id, myIDCount);
            return result - myIDCount;
        }

    }
}
