using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace sones.GraphDB.Manager.TypeManagement
{
    public class UniqueID
    {
        private long _id = Int64.MinValue;
        private Object _lock = new Object();

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

        public void SetID(long myNewID)
        {
            lock (_lock) 
            {
                _id = myNewID;
            }
        }

        public void SetToMaxID(long myID)
        {
            if (myID > _id)
            lock (_lock)
            {
                //Repeat the question in locked area, because it might have changed.
                if (myID > _id)
                    _id = myID;
            }
        }

    }
}
