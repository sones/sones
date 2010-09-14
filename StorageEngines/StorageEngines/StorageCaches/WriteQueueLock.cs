/* GraphFS
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
