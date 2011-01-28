/* GraphFS
 * (c) Stefan Licht, 2009
 * 
 * This is the WriteCache
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using sones.StorageEngines;

namespace sones.StorageEngines.Caches
{

    /// <summary>
    /// Prevent the ReadQueue to read Positions from the disks which are still in the WriteQueue and need to flush first
    /// </summary>
    public sealed class WriteCache
    {

        private Dictionary<UInt64, Byte[]> _WriteQueuePositions;
        private Hashtable _Storages;

        #region Singleton

        private static readonly WriteCache _Instance = new WriteCache();

        public static WriteCache Instance
        {
            get
            {
                return _Instance;
            }
        }

        // The laziness of type initializers is only guaranteed by .NET when the type isn't marked with a special flag called beforefieldinit
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static WriteCache()
        {
        }

        #endregion

        private WriteCache()
        {
            _WriteQueuePositions = new Dictionary<UInt64, Byte[]>();
            _Storages = new Hashtable();
        }


        /// <summary>
        /// The Writequeue add a new Position to the Lock
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        public void Add(QueueEntry QueueEntry)
        {
            // this will work only if we have just 1 Stream and 1 Extent!!!
            foreach (var ObjectExtent in QueueEntry.RWQueueStreams)
            {
                var WriteQueuePositions = new Dictionary<UInt64, Byte[]>();

                lock (_Storages)
                {
                    if (_Storages[ObjectExtent.StorageUUID] == null)
                        _Storages.Add(ObjectExtent.StorageUUID, WriteQueuePositions);
                    else
                        WriteQueuePositions = (Dictionary<UInt64, Byte[]>)_Storages[ObjectExtent.StorageUUID];
                    try
                    {
                        if (WriteQueuePositions.ContainsKey(ObjectExtent.PhysicalPosition))
                        {
//                                System.Diagnostics.Debug.WriteLine("[WriteCache]" + ObjectExtent.StorageID + "(" + WriteQueuePositions.Count + ") Overwrite " + ObjectExtent.PhysicalPosition + " Data: " + QueueEntry.Data.Length);
                            WriteQueuePositions[ObjectExtent.PhysicalPosition] = QueueEntry.Data;
                        }
                        else
                        {
//                                System.Diagnostics.Debug.WriteLine("[WriteCache]" + ObjectExtent.StorageID + "(" + WriteQueuePositions.Count + ") Add " + ObjectExtent.PhysicalPosition + " Data: " + QueueEntry.Data.Length);
                            WriteQueuePositions.Add(ObjectExtent.PhysicalPosition, QueueEntry.Data);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Check whether or not the position is not yet written to disk
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        /// <param name="StorageID">The StorageID of the storage</param>
        /// <returns>True, if this position is still not written to the disk</returns>
        public Boolean Contains(UInt64 Position, UInt64 StorageID)
        {
            
            var WriteQueuePositions = new Dictionary<UInt64, Byte[]>();

            lock (_Storages)
            {
                if (_Storages[StorageID] != null)
                {
                    WriteQueuePositions = (Dictionary<UInt64, Byte[]>)_Storages[StorageID];
                    return WriteQueuePositions.ContainsKey(Position);
                }
            }
            
            return false;

        }

        /// <summary>
        /// Check whether or not the position is not yet written to disk
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        /// <returns>True, if this position is still not written to the disk</returns>
        public Boolean Contains(UInt64 Position)
        {
            // Like QueueEntry - StorageID 1 seems to be the standard
            return Contains(Position, 1);
        }

        /// <summary>
        /// After complete writing of this Position to the storage, remove it from the Lock
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        /// <param name="StorageID">The StorageID of the storage</param>
        public void Remove(UInt64 Position, UInt64 StorageID)
        {

            var WriteQueuePositions = new Dictionary<UInt64, Byte[]>();
            
            lock (_Storages)
            {
                if (_Storages[StorageID] != null)
                {
                    WriteQueuePositions = (Dictionary<UInt64, Byte[]>)_Storages[StorageID];
//                    System.Diagnostics.Debug.WriteLine("[WriteCache]" + StorageID + "(" + WriteQueuePositions.Count + ") Remove " + Position);
                    WriteQueuePositions.Remove(Position);
                }
            }

        }

        /// <summary>
        /// After complete writing of this Position to the storage, remove it from the Lock
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        public void Remove(UInt64 Position)
        {
            // Like QueueEntry - StorageID 1 seems to be the standard
            Remove(Position, 1);
        }

        /// <summary>
        /// Get the cached bytearray
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        /// <param name="StorageID">The StorageID of the storage</param>
        public Byte[] Get(UInt64 Position, UInt64 StorageID)
        {

            var WriteQueuePositions = new Dictionary<UInt64, Byte[]>();
            
            lock (_Storages)
            {
                if (_Storages[StorageID] != null)
                {
                    WriteQueuePositions = (Dictionary<UInt64, Byte[]>)_Storages[StorageID];
                    if (WriteQueuePositions.ContainsKey(Position))
                    {
//                        System.Diagnostics.Debug.WriteLine("[WriteCache]"+StorageID+"(" + WriteQueuePositions.Count + ") Get " + Position);
                        return WriteQueuePositions[Position];
                    }
                }
            }
            
            return new Byte[0];

        }

        /// <summary>
        /// Get the cached bytearray
        /// </summary>
        /// <param name="Position">The Physical Position on the storage</param>
        public Byte[] Get(UInt64 Position)
        {
            // Like QueueEntry - StorageID 1 seems to be the standard
            return Get(Position, 1);
        }

        public UInt64 Count
        {
            get
            {
                UInt64 C = 0;
                foreach (UInt64 _StorageID in _Storages.Keys)
                {
                    C += (UInt64)((Dictionary<ulong, byte[]>)_Storages[_StorageID]).Count;
                }
                return C;
            }
        }

    }
}
