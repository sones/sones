/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* PandoraFS
 * (c) Achim Friedland, 2008-2010
 * 
 * This is the WriteQueueManager - every write action takes place in here
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using sones.Lib;
using sones.Notifications;
using sones.StorageEngines;

#endregion

namespace sones.StorageEngines.Caches
{

    public class FlushSucceededEventArgs
    {

        public FlushSucceededEventArgs(UInt64 myPhysicalPostion, UInt64 myLength)
        {
            PhysicalPostion = myPhysicalPostion;
            Length = myLength;
        }

        public UInt64 PhysicalPostion;
        public UInt64 Length;
    }

    public delegate void FlushSucceededHandler(Object sender, FlushSucceededEventArgs args);

    /// <summary>
    /// This is the WriteQueueManager - every write action takes place in here
    /// </summary>
    public class WriteQueueManager
    {

        public event FlushSucceededHandler OnFlushSucceeded;


        #region Data

        FileStream                PandoraFSFileStream;
        private List<QueueEntry>  WriteQueue;
        private Thread            _WriterThread;

        // Volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        private volatile Boolean  ShutdownWriterThread      = false;
        private Boolean           BlockQueueInsertion       = false;
        private volatile Boolean  PauseTheQueue             = false;
        private volatile Boolean  WriteQueueThreadSleeping;
        private volatile Boolean  QueueFull;
        private          Object   WriteQueueHasFreeSpace    = "";
        private          Object   WriteQueueHasNewElements  = "";
        private          Object   WriteQueueThreadEndOfPause   = "";
        private          WriteQueueLock    _WriteQueueLock  = null;

        public NotificationDispatcher NotificationDispatcher { get; set; }

        #endregion

        #region Properties

        #region MaxNumberOfQueueEntries

        private UInt32 _MaxNumberOfQueueEntries = 120;

        public UInt32 MaxNumberOfQueueEntries
        {

            get
            {
                return _MaxNumberOfQueueEntries;
            }

            set
            {
                _MaxNumberOfQueueEntries = checked(value);
            }

        }

        #endregion

        #region NumberOfQueueEntries

        public UInt32 NumberOfQueueEntries
        {

            get
            {

                lock (WriteQueue)
                {
                    return (UInt32) WriteQueue.Count;
                }

            }

        }

        #endregion

        #region MaxNumberOfWriteAttempts

        private UInt32 _MaxNumberOfWriteAttempts = 10;

        public UInt32 MaxNumberOfWriteAttempts
        {

            get
            {
                return _MaxNumberOfWriteAttempts;
            }

            set
            {
                _MaxNumberOfWriteAttempts = checked(value);
            }

        }

        #endregion

        #region isPaused

        public Boolean isPaused
        {

            get
            {
                return PauseTheQueue;
            }

        }

        #endregion

        #region isShuttingDown

        public Boolean isShuttingDown
        {

            get
            {
                return (isShuttingDown);
            }

        }

        #endregion

        #region isOffline

        private bool QueueIsOffline = false;

        public Boolean isOffline
        {

            get
            {
                return (QueueIsOffline);
            }

        }

        #endregion

        #region BytesWritten

        private UInt64 _BytesWritten = 0;

        public UInt64 BytesWritten
        {

            get
            {
                return _BytesWritten;
            }

        }

        #endregion

        #endregion


        #region Constructor

        #region WriteQueueManager(WriteQueueLock myWriteQueueLock)

        /// <summary>
        /// Instantiates the WriteQueueManager
        /// </summary>
        /// <param name="myWriteQueueLock">hold all position which are currently not written from WriteQueueManager</param>
        public WriteQueueManager(WriteQueueLock myWriteQueueLock)
        {

            _WriteQueueLock         = myWriteQueueLock;
            PandoraFSFileStream     = null;
            ShutdownWriterThread    = false;
            WriteQueue              = new List<QueueEntry>();
            _WriterThread           = new Thread(new ThreadStart(WriteQueueThread));
            _WriterThread.Name      = "WriteQueueManager.WriteQueueThread()";
            _WriterThread.Priority  = ThreadPriority.Highest;
            _WriterThread.Start();

        }

        #endregion

        #region WriteQueueManager(myInputFileStream, WriteQueueLock myWriteQueueLock)

        /// <summary>
        /// Instantiates the WriteQueueManager and attaches the given FileStream
        /// </summary>
        /// <param name="myInputFileStream">the FileStream for reading and writing</param>
        /// <param name="myWriteQueueLock">hold all position which are currently not written from WriteQueueManager</param>
        public WriteQueueManager(FileStream myInputFileStream, WriteQueueLock myWriteQueueLock)
            : this(myWriteQueueLock)
        {
            // First do the normal stuff!
            SetFileStream(myInputFileStream);
        }

        #endregion

        #endregion

        #region SetFileStream

        /// <summary>
        /// Sets the Filestream of this QueueManager. This can only be _done once per instance.
        /// </summary>
        /// <param name="myInputFileStream">the already opened and read/writeable Filestream</param>
        public void SetFileStream(FileStream myInputFileStream)
        {

            if (!myInputFileStream.CanWrite)
                throw new QueueManagerException("[WriteQueue] Can not write to the underlying filesystem!");

            if (PandoraFSFileStream == null)
                PandoraFSFileStream = myInputFileStream;

            else
                throw new QueueManagerException("[WriteQueue] This WriteQueueManager is already initialised!");

        }

        #endregion

/*
        // The following method will be run within an own thread!
        #region WriteQueueThread

        /// <summary>
        /// This is the WriteQueueThread, here all writing takes place...
        /// </summary>
        public void WriteQueueThread()
        {

            #region Data

            QueueEntry  actualQueueElement;
            Int32       Divisor, Major, Minor;

            #endregion

            #region Sleep until FileStream was attached or thread was ended

            while (PandoraFSFileStream == null && !ShutdownWriterThread)
            {

                try
                {

                    // Send this thread into sleep, it may reawake every 1ms
                    Thread.Sleep(1);

                }
                catch (ThreadInterruptedException e)
                {
                    Debug.WriteLine("[WriteQueueThread] Thread reawaken from start-up sleep (" + e + ")!");
                }

            }

            #endregion


            // Loop until thread was ended, but empty queue before
            while (!ShutdownWriterThread || WriteQueue.Count > 0)
            {

                #region Get first element of the WriteQueue

                actualQueueElement = null;

                lock (WriteQueue)
                {
                    if (WriteQueue.Count > 0)
                    {
                        actualQueueElement = WriteQueue[0];
                        WriteQueue.RemoveAt(0);
                    }

                }

                #endregion

                #region WriteQueueEntry

                if (actualQueueElement != null)
                {

//                    Console.WriteLine("Number of entries: " + WriteQueue.Count);

                    lock (PandoraFSFileStream)
                    {

                        foreach (ObjectStream myObjectStream in actualQueueElement.Streams)
                        {

                            foreach (ObjectExtent myExtent in myObjectStream.Extents)
                            {

                                // The actual .NET framework can only write 2^32 bytes at once!
                                // So check if the extent logical position plus its length is
                                // smaller than the getLatestRevisionTime value of a 32 bit integer:
                                // If yes, write at once..
                                if (myExtent.LogicalPosition + myExtent.Length <= Int32.MaxValue)
                                {
                                    PandoraFSFileStream.Seek( (Int64) myExtent.PhysicalPosition, 0);
                                    PandoraFSFileStream.Write(actualQueueElement.Data, (Int32) myExtent.LogicalPosition, (Int32) Math.Min(myExtent.Length, (UInt64) actualQueueElement.Data.Length - myExtent.LogicalPosition));
                                    _BytesWritten += myExtent.Length;
//                                    Debug.WriteLine("[WriteQueue] Writing " + myExtent.Length + " bytes at position " + myExtent.PhysicalPosition);
                                }

                                // If no, write in smaller pieces!
                                else
                                {

                                    Divisor = Int32.MaxValue / 8;
                                    Major   = (Int32) myExtent.Length / Divisor;
                                    Minor   = (Int32) myExtent.Length % Divisor;

                                    Byte[] tmpData = new Byte[Divisor];

                                    for (Int32 i = 0; i < Major; i++)
                                    {
                                        actualQueueElement.Data.CopyTo(tmpData, i * Divisor);
                                        PandoraFSFileStream.Seek( (Int64) myExtent.PhysicalPosition + i * Divisor, 0);
                                        PandoraFSFileStream.Write(tmpData, 0, Divisor);
                                        _BytesWritten += (UInt64) Divisor;
                                    }

                                    // Write the rest of the data
                                    actualQueueElement.Data.CopyTo(tmpData, Major * Divisor);
                                    PandoraFSFileStream.Seek( (Int64) myExtent.PhysicalPosition + Major * Divisor, 0);
                                    PandoraFSFileStream.Write(tmpData, 0, Minor);
                                    _BytesWritten += (UInt64) Minor;

                                }

                                // CORRECTION: Remove this Entry from Writequeue - Prevent Readqueue from reading not yet written positions
//                                WriteQueueLock.RemoveLock(myExtent.PhysicalPosition);

                            }

                        }

                    }

                    // Check if the FileStream has to be flushed now
                    if (actualQueueElement.FlushAfterWrite)
                        PhysicalFlush();

                }

                #endregion

                #region Pause the thread if this was requested

                Debug.WriteLineIf(PauseTheQueue, "[WriteQueueThread] Thread paused!");

                while (PauseTheQueue)
                {

                    try
                    {

                        // Send this thread into sleep, it may reawake every 5sec, by an
                        // interrupt signal send by the Write()-method or unpause
                        Thread.Sleep(5000);
                        Debug.WriteLine("[WriteQueueThread] Thread insomnia! Still pause?");

                    }
                    catch (ThreadInterruptedException e)
                    {
                        Debug.WriteLine("[WriteQueueThread] Thread reawaken from pause by interrupt (" + e + ")!");
                    }

                }

                #endregion

                #region If queue is empty and nothing to do: Goto sleep

                #region Check QueueSize

                lock (WriteQueue)
                {

                    if (WriteQueue.Count > 0)
                        NothingToDo = false;

                    else
                    {

                        NothingToDo = true;

                        // Make sure everything is written to disc...
                        PhysicalFlush();

                        Debug.WriteLine("[WriteQueueThread] Nothing to do, will sleep now!");

                    }

                }

                #endregion

                #region Loop of insomnia

                while (NothingToDo)
                {

                    try
                    {

                        // Send this thread into sleep, it may reawake every 1ms, by an
                        // interrupt signal send by the Write()-method or unpause
                        Thread.Sleep(1);
//                        Debug.WriteLine("[WriteQueueThread] Thread insomnia! Still nothing to do?");

                    }
                    catch (ThreadInterruptedException e)
                    {
                        Debug.WriteLine("[WriteQueueThread] Thread reawaken from idleness by interrupt (" + e + ")!");
                    }

                }

                #endregion

                #endregion

            }

            ShutdownWriterThread  = false;
            QueueIsOffline        = true;

        }

        #endregion
*/









        // The following method will be run within an own thread!
        #region WriteQueueThread

        /// <summary>
        /// This is the WriteQueueThread, here all writing takes place...
        /// </summary>
        public void WriteQueueThread()
        {

            #region Data

            QueueEntry  actualQueueElement;
            Int32       Divisor, Major, Minor;

            #endregion

            #region Sleep until FileStream was attached or thread was ended

            while (PandoraFSFileStream == null && !ShutdownWriterThread)
            {

                try
                {

                    // Send this thread into sleep, it may reawake every 10ms
                    Thread.Sleep(10);

                }
                catch (ThreadInterruptedException e)
                {
//                    Debug.WriteLine("[WriteQueueThread] Thread reawaken from start-up sleep (" + e + ")!");
                }

            }

            #endregion


            // Break this loop only if Shutdown==true and WriteQueue==empty
            while (!(ShutdownWriterThread && WriteQueueThreadSleeping) || WriteQueue.Count > 0)
            {

                #region Pause the queue processing!

                if (PauseTheQueue)
                {

                    Debug.WriteLine("[WriteQueueThread] Thread paused!");

                    lock (WriteQueueThreadEndOfPause)
                    {
                        // Wartet auf Unpause()
                        Monitor.Wait(WriteQueueThreadEndOfPause);
                    }

                    Debug.WriteLine("[WriteQueueThread] Thread (re-)awaken from pause!");

                }

                #endregion

                #region Sleep if there is nothing to do... or get next QueueEntry!

                actualQueueElement        = null;
                WriteQueueThreadSleeping  = false;

                lock (WriteQueue)
                {

                    if (QueueFull && WriteQueue.Count < _MaxNumberOfQueueEntries)
                    {
                        lock (WriteQueueHasFreeSpace)
                        {
                            // Signalisiert, dass wieder Platz im Puffer ist
                            Monitor.PulseAll(WriteQueueHasFreeSpace);
                        }
                    }

                    // Falls keine Elemente zum Verbrauchen
                    if (WriteQueue.Count == 0)
                    {
                        WriteQueueThreadSleeping = true;
                    }

                    // else get first element of the WriteQueue!
                    else
                    {
                        actualQueueElement = WriteQueue[0];
                        WriteQueue.RemoveAt(0);
                    }

                }

                #endregion

                #region Goto Sleep!

                if (WriteQueueThreadSleeping)
                {

                    // Make sure everything is written to disc...
                    //PhysicalFlush();
//                    Debug.WriteLine("[WriteQueueThread] Thread went to sleep!");

                    lock (WriteQueueHasNewElements)
                    {
                        // Wartet bis wieder ein Element vorhanden ist... aber maximal 10ms
                        Monitor.Wait(WriteQueueHasNewElements, 10);
                    }

//                    Debug.WriteLine("[WriteQueueThread] Thread (re-)awake from sleep!");

                }

                #endregion

                #region Write QueueEntry

                if (actualQueueElement != null)
                {

//                    Debug.WriteLine("[WriteQueueThread] Number of entries: " + WriteQueue.Count);

                    lock (PandoraFSFileStream)
                    {

                      
                        #region Write Extents

                        foreach (var _ObjectExtent in actualQueueElement.RWQueueStreams)
                        {

                            // The actual .NET framework can only write 2^32 bytes at once!
                            // So check if the extent logical position plus its length is
                            // smaller than the getLatestRevisionTime value of a 32 bit integer:
                            // If yes, write at once..
                            if (_ObjectExtent.LogicalPosition + _ObjectExtent.Length <= Int32.MaxValue)
                            {
                                PandoraFSFileStream.Seek((Int64)_ObjectExtent.PhysicalPosition, 0);
                                PandoraFSFileStream.Write(actualQueueElement.Data, (Int32)_ObjectExtent.LogicalPosition, (Int32)Math.Min(_ObjectExtent.Length, (UInt64)actualQueueElement.Data.Length - _ObjectExtent.LogicalPosition));
                                _BytesWritten += _ObjectExtent.Length;
//                                    Debug.WriteLine("[WriteQueue] Writing " + myExtent.Length + " bytes at position " + myExtent.PhysicalPosition);
                            }

                            // If no, write in smaller pieces!
                            else
                            {

                                Divisor = Int32.MaxValue / 8;
                                Major = (Int32)_ObjectExtent.Length / Divisor;
                                Minor = (Int32)_ObjectExtent.Length % Divisor;

                                var tmpData = new Byte[Divisor];

                                for (Int32 i = 0; i < Major; i++)
                                {
                                    actualQueueElement.Data.CopyTo(tmpData, i * Divisor);
                                    PandoraFSFileStream.Seek((Int64)_ObjectExtent.PhysicalPosition + i * Divisor, 0);
                                    PandoraFSFileStream.Write(tmpData, 0, Divisor);
                                    _BytesWritten += (UInt64)Divisor;
                                }

                                // Write the rest of the data
                                actualQueueElement.Data.CopyTo(tmpData, Major * Divisor);
                                PandoraFSFileStream.Seek((Int64)_ObjectExtent.PhysicalPosition + Major * Divisor, 0);
                                PandoraFSFileStream.Write(tmpData, 0, Minor);
                                _BytesWritten += (UInt64)Minor;

                            }

                            // CORRECTION: Remove this Entry from Writequeue - Prevent Readqueue from reading not yet written positions
                            //_WriteQueueLock.RemoveLock(_ObjectExtent.PhysicalPosition);
                            if (OnFlushSucceeded != null)
                            {
                                var args = new FlushSucceededEventArgs(_ObjectExtent.PhysicalPosition, _BytesWritten);
                                OnFlushSucceeded(this, args);
                            }

                        }

                        #endregion

                        

                    }

                    // Check if the FileStream has to be flushed now
                    if (actualQueueElement.FlushAfterWrite)
                        PhysicalFlush();

                }

                #endregion


            }

            ShutdownWriterThread  = false;
            QueueIsOffline        = true;
            PhysicalFlush();

            //Debug.WriteLine("WriteQueue.Count: " + WriteQueue.Count);
            //Debug.WriteLine("   \t_WriteQueuePositions" + ((List<UInt64>)UnitTestHelper.GetPrivateField("_WriteQueuePositions", _WriteQueueLock)).Count);

        }

        #endregion



        #region Write

        /// <summary>
        /// Adds a Write request to the WriteQueue; if more than _MaxNumberOfQueueEntries are in the Queue
        /// we wait until there's less.
        /// </summary>
        /// <param name="newEntry">the WriteQueueEntry</param>
        public void Write(QueueEntry newEntry)
        {

            Boolean    WriteSucceeded  = false;
            Int16      WriteAttempt    = 0;

            // Wait, while the queue is flushing its entries
            while (BlockQueueInsertion)
                Thread.Sleep(1);

            if (ShutdownWriterThread)
                throw new QueueManagerException("[WriteQueue] Write failed, because the WriteQueueThread was shut down!");

            if (QueueIsOffline)
                throw new QueueManagerException("[WriteQueue] Write failed, because the WriteQueueThread is offline!");

            while (!WriteSucceeded && WriteAttempt <= _MaxNumberOfWriteAttempts)
            {               

                QueueFull = false;

                // Lock until WriteQueue get's free to use...
                lock (WriteQueue)
                {
                    if (WriteQueue.Count >= _MaxNumberOfQueueEntries) // Falls Puffer voll
                    {
                        QueueFull = true;
                        //NStorageEngine_WriteQueueFull.Arguments args = new NStorageEngine_WriteQueueFull.Arguments();
                        //args.NumberOfQueueEntries = NumberOfQueueEntries;

                        //NotificationDispatcher.SendNotification(typeof(NStorageEngine_WriteQueueFull), args);
                    }
                }

                if (QueueFull)
                    lock (WriteQueueHasFreeSpace)
                    {
                        // Wartet bis wieder Platz im Puffer vorhanden ist
                        Monitor.Wait(WriteQueueHasFreeSpace);
                        QueueFull = false;
                    }

                lock (WriteQueue)
                {

                    WriteAttempt++;

                    if (WriteQueue.Count < _MaxNumberOfQueueEntries)
                    {

                        WriteQueue.Add(newEntry);
                        
                        // CORRECTION: Add a new Entry to Writequeue - Prevent Readqueue from reading not yet written positions
                        /*
                        foreach (var _ObjectExtent in newEntry.RWQueueStreams)
                            _WriteQueueLock.AddLock(_ObjectExtent.PhysicalPosition);
                        */
                          
//                        Debug.WriteLine("[WriteQueue] Adding new entry!");

                        //Debug.WriteLine("[WriteQueueThread] Added to the queue!");

                        // Wake the WriteQueueThread if it was fallen asleep
                        WriteSucceeded  = true;

                        if (WriteQueueThreadSleeping)
                            lock (WriteQueueHasNewElements)
                            {
                                // Signalisiert, dass wieder ein Element vorhanden ist
                                Monitor.PulseAll(WriteQueueHasNewElements);
                            }

                    }

                }

            }

            // ...fail afterwards!
            // This should actually never happen!!!
            if (!WriteSucceeded)
                throw new QueueManagerCouldNotWriteQueueException("[WriteQueue] Could not write to the queue!");

        }

        #endregion

        #region DirectWrite

        /// <summary>
        /// This method writes directly a QueueEntry as soon as possible on disc.
        /// It does this by putting the QueueEntry infront of all other QueueEntries
        /// within the queue.
        /// In most cases this is not what you want: USE WITH CARE!
        /// </summary>
        /// <param name="newEntry">the QueueEntry that needs to be written as soon as possible</param>
        public void DirectWrite(QueueEntry newEntry)
        {

            // Wait, while the queue is flushing its entries
            while (BlockQueueInsertion)
                Thread.Sleep(1);

            if (ShutdownWriterThread)
                throw new QueueManagerException("[WriteQueue] Write failed, because the WriteQueueThread was shut down!");

            if (QueueIsOffline)
                throw new QueueManagerException("[WriteQueue] Write failed, because the WriteQueueThread is offline!");


            newEntry.FlushAfterWrite = true;

            lock (WriteQueue)
            {
                WriteQueue.Insert(0, newEntry);
            }

        }

        #endregion


        #region Queue management methods

        #region PauseQueue()

        /// <summary>
        /// Pause the queue, use UnpauseQueue() for reactivation.
        /// </summary>
        public void PauseQueue()
        {
            PauseTheQueue = true;
        }

        #endregion

        #region UnpauseQueue()

        /// <summary>
        /// Reactivate a paused the queue.
        /// </summary>
        public void UnpauseQueue()
        {
            
            PauseTheQueue = false;

            lock (WriteQueueThreadEndOfPause)
            {
                // Signalisiert, dass die Pause rum ist!
                Monitor.PulseAll(WriteQueueThreadEndOfPause);
            }

        }

        #endregion

        #region FlushQueue()
        /// <summary>
        /// This method waits until every QueueEntry is written on disc
        /// and calls PhysicalFlush() afterwards
        /// </summary>
        public void FlushQueue()
        {

            // Block new Write()-requests until flushing is completed
            BlockQueueInsertion = true;

            while (BlockQueueInsertion)
            {

                lock (WriteQueue)
                {

                    if (WriteQueue.Count == 0)
                        BlockQueueInsertion = false;

                    lock (WriteQueueHasNewElements)
                    {
                        // Signalisiert, dass wieder ein Element vorhanden ist
                        Monitor.PulseAll(WriteQueueHasNewElements);
                    }

                }

            }

            PhysicalFlush();

        }
        #endregion

        #region PhysicalFlush()
        /// <summary>
        /// This private method calls the FileStream.Flush() method of the
        /// actual FileStream we depend on.
        /// </summary>
        private void PhysicalFlush()
        {

            lock (PandoraFSFileStream) 
            {
                PandoraFSFileStream.Flush();
            }

        }
        #endregion

        #endregion


        #region ShutdownQueueManagerThread()

        /// <summary>
        /// Ends the QueueManager - that means that the QueueManagerThreads is
        /// signalised to shut down. The WriteQueueThread then does not accept
        /// any further write requests, writes all QueueEntries on disc and exits.
        /// </summary>
        public void ShutdownQueueManagerThread()
        {

            ShutdownWriterThread  = true;

            lock (WriteQueueHasNewElements)
            {
                // Signalisiert, dass der WriteQueueThread aufwachen soll!
                Monitor.PulseAll(WriteQueueHasNewElements);
            }

        }

        #endregion


    }

}
