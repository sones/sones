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
 * (c) Daniel Kirstenpfad, 2007-2008
 * Achim Friedland, 2008
 * 
 * This is the ReadQueueManager - every read action takes place in here
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
using sones.StorageEngines;

#endregion

namespace sones.StorageEngines.Caches
{

    /// <summary>
    /// This is the ReadQueueManager - every read action takes place in here
    /// </summary>
    public class ReadQueueManager
    {


        #region Data

        public  FileStream        GraphFSFileStream;
        private List<QueueEntry>  _ReadQueue;
        private Thread            _ReaderThread;
        private bool              ShutdownReaderThread          = false;
        private bool              PauseTheQueue                 = false;
        private WriteQueueLock    _WriteQueueLock                = null;

        #endregion

        #region Properties

        #region MaxNumberOfQueueEntries

        private UInt32 _MaximumNumberOfQueueEntries = 128;

        public UInt32 MaximumNumberOfQueueEntries
        {

            get
            {
                return _MaximumNumberOfQueueEntries;
            }

            set
            {
                _MaximumNumberOfQueueEntries = checked(value);
            }

        }

        #endregion

        #region NumberOfQueueEntries

        public UInt32 NumberOfQueueEntries
        {

            get
            {

                lock (_ReadQueue)
                {
                    return (UInt32)_ReadQueue.Count;
                }

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

        #region BytesRead

        private UInt64 _BytesRead = 0;

        public UInt64 BytesRead
        {

            get
            {
                return _BytesRead;
            }

        }

        #endregion

        #endregion


        #region Constructor

        #region ReadQueueManager(WriteQueueLock myWriteQueueLock)

        /// <summary>
        /// Instantiates the ReadQueueManager
        /// </summary>
        /// <param name="myWriteQueueLock">hold all position which are currently not written from WriteQueueManager</param>
        public ReadQueueManager(WriteQueueLock myWriteQueueLock)
        {

            _WriteQueueLock        = myWriteQueueLock;
            GraphFSFileStream    = null;
            ShutdownReaderThread   = false;
            _ReadQueue             = new List<QueueEntry>();
            _ReaderThread          = new Thread(new ThreadStart(ReadQueueThread));
            _ReaderThread.Name     = "ReadQueueManager.ReadQueueThread()";
            _ReaderThread.Priority = ThreadPriority.Highest;
            _ReaderThread.Start();

        }

        #endregion

        #region ReadQueueManager(myInputFileStream)

        /// <summary>
        /// Instantiates the ReadQueueManager and attaches the given FileStream
        /// </summary>
        /// <param name="myInputFileStream">the FileStream for reading and writing</param>
        /// <param name="myWriteQueueLock">hold all position which are currently not written from WriteQueueManager</param>
        public ReadQueueManager(FileStream myInputFileStream, WriteQueueLock myWriteQueueLock) : this(myWriteQueueLock)
        {

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

            if (!myInputFileStream.CanRead)
                throw new QueueManagerException("[ReadQueue] Can not read from the underlying filesystem!");

            if (GraphFSFileStream == null)
                GraphFSFileStream = myInputFileStream;
            else
                throw new QueueManagerException("[ReadQueue] This ReadQueueManager is already initialised!");

        }

        #endregion



        // The following method will be run within an own thread!
        #region ReadQueueThread

        /// <summary>
        /// This is the actual QueueManagerThread
        /// </summary>
        public void ReadQueueThread()
        {

            #region Data
            #endregion

            #region Sleep until FileStream was attached or thread was ended

            while (GraphFSFileStream == null && !ShutdownReaderThread)
            {

                try
                {

                    // Send this thread into sleep, it may reawake every 1ms
                    Thread.Sleep(1);

                }
                catch (ThreadInterruptedException e)
                {
                    Debug.WriteLine("[ReadQueueThread] Thread reawaken from start-up sleep (" + e + ")!");
                }

            }

            #endregion


            // Loop until thread was ended, but empty queue before
            while (!ShutdownReaderThread)
            {
                // 1 Milliseconds wait..
                Thread.Sleep(1);
            }

            ShutdownReaderThread  = false;
            QueueIsOffline        = true;

        }

        #endregion



        #region Read

        /// <summary>
        /// Reads a pile of data from the filesystem
        /// </summary>
        /// <param name="ReadEntry">a QueueEntry </param>
        /// <returns></returns>
        public byte[] Read(QueueEntry ReadEntry)
        {
            // check size
            if (ReadEntry.DataLength > Int32.MaxValue)
                throw new QueueManagerException("Cannot read QueueEntry - it's too big.");

            byte[] Output = new byte[ReadEntry.DataLength];

            // TODO: actually make use of the read queue...

            if (!GraphFSFileStream.CanRead)
                throw new QueueManagerException("Cannot read. Maybe nothing is mounted.");

            // CORRECTION: Check if the current StartPosition is still in the WriteQueue - Prevent Readqueue from reading not yet written positions
            /*
            while (_WriteQueueLock.HasLock(ReadEntry.RWQueueStreams[0].PhysicalPosition))
            {
                Debug.WriteLine("[ReadQueue] has lock at Position " + ReadEntry.RWQueueStreams[0].PhysicalPosition);
                Thread.Sleep(1); // or read next in queue
            }
            */
            lock (GraphFSFileStream)
            {
                GraphFSFileStream.Seek( (long) ReadEntry.RWQueueStreams[0].PhysicalPosition, 0);
                Int32 HowMany = GraphFSFileStream.Read(Output, 0, (Int32)ReadEntry.RWQueueStreams[0].Length);

                if (_BytesRead + (UInt64) HowMany < 0)
                    _BytesRead = 0;

                _BytesRead += (UInt64) HowMany;
            }
            //Debug.WriteLine("[ReadQueue] Reading " + Output.Length + " bytes from position " + ReadEntry.Streams[0].Extents[0].PhysicalPosition);
            //Debug.WriteLine("   \t_WriteQueuePositions" + ((List<UInt64>)UnitTestHelper.GetPrivateField("_WriteQueuePositions", _WriteQueueLock)).Count);
            
            return Output;
        }


        /// <summary>
        /// Reads a pile of data from the filesystem
        /// </summary>
        /// <param name="StartPosition">where to start reading</param>
        /// <param name="StreamLength">who many bytes to read</param>        
        /// <returns>the read data</returns>
        public byte[] Read(UInt64 StartPosition, UInt64 Length)
        {
            // check size
            if (Length > Int32.MaxValue)
                throw new QueueManagerException("[ReadQueue] You can not read that much data.");

            byte[] Output = new byte[Length];

            // TODO: actually make use of the read queue...

            if (!GraphFSFileStream.CanRead)
                throw new QueueManagerException("Cannot read. Maybe nothing is mounted.");

            // CORRECTION: Check if the current StartPosition is still in the WriteQueue - Prevent Readqueue from reading not yet written positions
            /*
            while (_WriteQueueLock.HasLock(StartPosition))
            {
                Debug.WriteLine("[ReadQueue] has lock at Position " + StartPosition);
                Thread.Sleep(1); // or read next in queue
            }
             * */

            lock (GraphFSFileStream)
            {
                GraphFSFileStream.Seek((long)StartPosition, 0);
                Int32 HowMany = GraphFSFileStream.Read(Output, 0, (Int32)Length);

                if (_BytesRead + (UInt64) HowMany < 0)
                    _BytesRead = 0;

                _BytesRead += (UInt64) HowMany;
            }

            
            //Debug.WriteLine("[ReadQueue] Reading " + Length + " bytes from position " + StartPosition);
            
            return Output;
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
            _ReaderThread.Interrupt();
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

            ShutdownReaderThread  = true;
//            _ReaderThread.Interrupt();

        }

        #endregion


    }

}
