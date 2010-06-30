/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* PandoraFS
 * (c) Stefan Licht, 2009
 * 
 * This is the WriteQueueLock - prevent the ReadQueue from reading dirty blocks - these are blocks which are still in the WriteQueue and not yet written to the storage
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace sones.StorageEngines.Caches
{

    /// <summary>
    /// Prevent the ReadQueue to read Positions from the disks which are still in the WriteQueue and need to flush first
    /// </summary>
    public class WriteQueueLock
    {
        private List<UInt64> _WriteQueuePositions = new List<UInt64>();
        private ReaderWriterLockSlim ReaderWriterLockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// The Writequeue add a new Position to the Lock
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        public void AddLock(UInt64 Position)
        {
            ReaderWriterLockSlim.EnterWriteLock();
                _WriteQueuePositions.Add(Position);
            ReaderWriterLockSlim.ExitWriteLock();
        }

        /// <summary>
        /// Check whether or not the position is not yet written to disk
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        /// <returns>True, if this position is still not written to the disk</returns>
        public Boolean HasLock(UInt64 Position)
        {
            Boolean retVal = false;
            
            ReaderWriterLockSlim.EnterReadLock();
                retVal = _WriteQueuePositions.Contains(Position);
            ReaderWriterLockSlim.ExitReadLock();

            return retVal;
        }

        /// <summary>
        /// After complete writing of this Position to the storage, remove it from the Lock
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        public void RemoveLock(UInt64 Position)
        {
            ReaderWriterLockSlim.EnterWriteLock();
                _WriteQueuePositions.Remove(Position);
            ReaderWriterLockSlim.ExitWriteLock();

        }

    }
}
