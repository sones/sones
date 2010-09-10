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

/* PandoraLib - PandoraThreadPool
 * (c) Stefan Licht, 2009
 * 
 * This class is a simple dynamic ThreadPool.
 * There are some workerThreads in it, which will work on tasks added with 
 * the QueueWorkItem() method.
 * The minimum number of workers is defined by the number of processors. The maximum 
 * could be defined with the Constructor parameter or is the ProcessorCount, as well.
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

namespace sones.Lib.Threading
{

    public delegate void WorkerThreadExceptionHandler(Object mySender, Exception myException);

    /// <summary>
    /// This is a simple dynamic ThreadPool.
    /// </summary>
    public class PandoraThreadPool : IDisposable
    {

        public event WorkerThreadExceptionHandler OnWorkerThreadException;

        #region Struct ThreadPoolEntry

        /// <summary>
        /// This is a pool entry. A worker will pick this entry and will invoke the delegate with the given params
        /// </summary>
        public struct ThreadPoolEntry
        {
            private Delegate _Delegate;
            private Object[] _Arguments;

            public ThreadPoolEntry(Delegate myDelegate, params Object[] myArguments)
            {
                if (myDelegate == null) throw new ArgumentNullException("myDelegate");

                _Delegate = myDelegate;
                _Arguments = myArguments;
            }

            internal void Invoke()
            {
                _Delegate.DynamicInvoke(_Arguments);
            }

        }

        #endregion

        #region Fields

        private Int32                   _MaxNumberOfParallelThreads;
        private Int32                   _MinNumberOfParallelThreads;
        private List<Thread>            _ParallelWorkerThreads;
        private Queue<ThreadPoolEntry>  _WorkitemsQueue;
        private Boolean                 _Disposed = false;
        private Boolean                 _AddingAllowed = true;
        
        private Int32                   _BusyWorkers = 0;
        private Int32                   _FreeWorkers = 0;
        private Int32                   _Workitems = 0;
        private Int32                   _WorkerThreads = 0;

        private Boolean                 _SuspendNextFreeWorker = false;
        private Object                  _SuspendNextFreeWorkerLock = new Object();
        private String                  _PoolName;

        #endregion

        #region Properties

        public Boolean IsBusy
        {
            get
            {
                return _BusyWorkers > 0;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// This will set the number of worker threads to Environment.ProcessorCount
        /// </summary>
        public PandoraThreadPool(String myPoolName)
            : this(myPoolName, Environment.ProcessorCount)
        { }
        
        /// <summary>
        /// This will define the maximum number of worker threads. But at least Environment.ProcessorCount.
        /// </summary>
        /// <param name="myMaxNumberOfParallelThreads">The maximum number of worker threads</param>
        public PandoraThreadPool(String myPoolName, Int32 myMaxNumberOfParallelThreads)
            : this(myPoolName, Environment.ProcessorCount, myMaxNumberOfParallelThreads)
        {
        }

        /// <summary>
        /// This will define the maximum number of worker threads. But at least Environment.ProcessorCount.
        /// </summary>
        /// <param name="myMaxNumberOfParallelThreads">The maximum number of worker threads</param>
        public PandoraThreadPool(String myPoolName, Int32 myMinNumberOfParallelThreads, Int32 myMaxNumberOfParallelThreads)
        {
            if (myMinNumberOfParallelThreads > myMaxNumberOfParallelThreads)
            {
                throw new ArgumentException("The minimum number of threads is greater than the maximum number of threads.");
            }

            _PoolName = myPoolName;

            // the min should not be bigger than myMaxNumberOfParallelThreads
            //_MinNumberOfParallelThreads = Math.Min(Environment.ProcessorCount, myMaxNumberOfParallelThreads);
            _MinNumberOfParallelThreads = myMinNumberOfParallelThreads;
            _MaxNumberOfParallelThreads = myMaxNumberOfParallelThreads;

            _ParallelWorkerThreads = new List<Thread>();
            _WorkitemsQueue = new Queue<ThreadPoolEntry>();

            for (Int32 i = 0; i < _MinNumberOfParallelThreads; i++)
            {
                AddWorkerThread();
            }
        }

        #endregion

        #region QueueWorkItem

        /// <summary>
        /// Queues a new workitem.
        /// </summary>
        /// <param name="myThreadPoolEntry">The work item, defined by the delegate and params</param>
        /// <returns>True, if the item was succesfully queued and False if it could be queued. This might be happen if the queue is full.</returns>
        public bool QueueWorkItem(ThreadPoolEntry myThreadPoolEntry)
        {
            if (!_AddingAllowed) throw new InvalidOperationException("Cannot queue item for this pool.");
            
            if (_Disposed) throw new InvalidOperationException("Cannot queue item for a disposed pool.");

            //System.Diagnostics.Debug.WriteLine("[PandoraThreadPool] BusyWorkers: " + _BusyWorkers + " in Queue: "
              //  + _ThreadEntries.Count + " FreeWorkers: " + _FreeWorkers + " CurThreads: " + _ParallelThreadWorkers.Count + " T:" + DateTime.Now.Ticks);

            if (_WorkerThreads < _MaxNumberOfParallelThreads && _FreeWorkers == 0 && _WorkerThreads < _Workitems)
            {
                AddWorkerThread();
                //System.Diagnostics.Debug.WriteLine("[PandoraThreadPool] Spawn New Thread: BusyWorkers: " + _BusyWorkers + " in Queue: " + _ThreadEntries.Count + " FreeWorkers: " + _FreeWorkers);
            }
            // all workers are free and nothing to do, shutdown one worker
            else if (_FreeWorkers == _WorkerThreads && _WorkerThreads > _MinNumberOfParallelThreads)
            {
                lock (_SuspendNextFreeWorkerLock)
                {
                    _SuspendNextFreeWorker = true;
                }
            }

            lock (_WorkitemsQueue)
            {
                try
                {
                    _WorkitemsQueue.Enqueue(myThreadPoolEntry);
                    Interlocked.Increment(ref _Workitems);
                }
                catch
                {
                    // The queue might be full
                    return false;
                }

                // there is at least 1 worker waiting for items
                if (_FreeWorkers > 0)
                    Monitor.Pulse(_WorkitemsQueue);
            }

            return true;

        }

        #endregion

        #region Worker methods

        private void AddWorkerThread()
        {
            Thread workerThread = new Thread(new ThreadStart(Worker));
            workerThread.IsBackground = true;
            workerThread.Name = _PoolName + " Worker " + _WorkerThreads;
            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start();
            _ParallelWorkerThreads.Add(workerThread);
            Interlocked.Increment(ref _WorkerThreads);
        }

        /// <summary>
        /// Is invoked multiple times
        /// </summary>
        private void Worker()
        {
            ThreadPoolEntry? threadPoolEntry = null;

            while (!_Disposed)
            {

                if (_SuspendNextFreeWorker)
                {
                    lock (_SuspendNextFreeWorkerLock)
                    {
                        if (_SuspendNextFreeWorker)
                        {
                            _SuspendNextFreeWorker = false;
                            Interlocked.Decrement(ref _WorkerThreads);
                            break;
                        }
                    }
                }

                Interlocked.Increment(ref _FreeWorkers);

                lock (_WorkitemsQueue)
                {

                    while (_WorkitemsQueue.Count == 0 && !_Disposed)
                    {
                        Monitor.Wait(_WorkitemsQueue, 50);
                    }

                    if (_Disposed) break;

                    threadPoolEntry = _WorkitemsQueue.Dequeue();
                    Interlocked.Decrement(ref _Workitems);
                }

                Interlocked.Decrement(ref _FreeWorkers);
                Interlocked.Increment(ref _BusyWorkers);

                try
                {
                    threadPoolEntry.Value.Invoke();
                }
                catch (Exception ex)
                {
                    if (OnWorkerThreadException != null)
                        if (ex.InnerException != null)
                            OnWorkerThreadException(this, ex.InnerException);
                        else
                            OnWorkerThreadException(this, ex);
                }

                Interlocked.Decrement(ref _BusyWorkers);

            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

            if (_Disposed) 
                throw new InvalidOperationException("Already Disposed!");

            _AddingAllowed = false;

            GC.SuppressFinalize(this);

            lock (_WorkitemsQueue)
            {
                while (!_Disposed)
                {
                    if (_WorkitemsQueue.Count == 0)
                        _Disposed = true;
                    else
                        Monitor.Wait(_WorkitemsQueue, 50); // wait for changed on _ThreadTargets
                }
            }

            if (_BusyWorkers > 0)
            {
                foreach (Thread thread in _ParallelWorkerThreads)
                {
                    thread.Join();
                }
            }
        }

        #endregion
    }
}
