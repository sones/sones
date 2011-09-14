/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Threading;

namespace sones.GraphDB.Manager.TypeManagement
{
    public class UniqueID
    {
        private long _id                = Int64.MinValue;
        private readonly Object _lock   = new Object();

        public UniqueID(long myStartID = Int64.MinValue)
        {
            _id = myStartID;
        }

        public long GetNextID()
        {
            var result = Interlocked.Increment(ref _id);
            return result - 1;
        }

        public long ReserveIDs(long myIDCount)
        {
            var result = Interlocked.Add(ref _id, myIDCount);
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
                    _id = myID + 1;
            }
        }

    }
}
