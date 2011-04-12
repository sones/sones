#region Usings

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BplusDotNet;
using sones.Library.NewFastSerializer;
using sones.Plugins.Index.Helper;
using sones.Plugins.Index.Interfaces;

#endregion

namespace sones.Plugins.Index
{
    /// <summary>
    /// This class realize a persistent single value index.
    /// </summary>
    /// <typeparam name="TKey">The type of the index key.</typeparam>
    /// <typeparam name="TValue">The type of the index value.</typeparam>
    public sealed class BufferedBplusDotNetIndex<TKey, TValue> : ISingleValueIndex<TKey, TValue>         
        where TKey   : IComparable
    {

        #region Data

        /// <summary>
        /// The internal index data structure.
        /// </summary>
        private xBplusTreeBytes                                 _Indexer;

        /// <summary>
        /// An serialization writer to serialize the keys and values.
        /// </summary>
        private readonly SerializationWriter                             _Writer;

        /// <summary>
        /// The tree file name. 
        /// </summary>
        private readonly String                                          _TreeFileName;

        /// <summary>
        /// The block file name.
        /// </summary>
        private readonly String                                          _BlockFileName;

        /// <summary>
        /// The prefix length for the tree.
        /// </summary>
        private readonly Int32                                           _PrefixLen;        

        /// <summary>
        /// 
        /// </summary>
        private readonly BlockingCollection<APipelinableRequest>         _Requests;

        /// <summary>
        /// The execution task.
        /// </summary>
        private readonly Task                                            _ExecTask;

        /// <summary>
        /// The cancellation token, that stops the execution.
        /// </summary>
        private readonly CancellationTokenSource                         _CancelToken;

        /// <summary>
        /// The blocking collection, contains the requests, which are waiting for execution.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, APipelinableRequest> _Results;

        /// <summary>
        /// The executed write tasks.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, APipelinableRequest> _ExecutedWriteTasks;

        #endregion

        #region Constructors

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="myTreeFileName">The name for the tree file.</param>
        /// <param name="myBlockFileName">The name for the block file.</param>
        /// <param name="myPrefixLength">The prefix length for the tree.</param>
        public BufferedBplusDotNetIndex(String myTreeFileName, String myBlockFileName, Int32 myPrefixLength, CancellationTokenSource myCancelToken)
        {
            if (File.Exists(myTreeFileName) && File.Exists(myBlockFileName))
            {
                _Indexer = xBplusTreeBytes.ReOpen(myTreeFileName, myBlockFileName);
            }
            else
            {
                _Indexer = xBplusTreeBytes.Initialize(myTreeFileName, myBlockFileName, myPrefixLength);
            }

            _TreeFileName   = myTreeFileName;
            _BlockFileName  = myBlockFileName;
            _PrefixLen      = myPrefixLength;

            _Indexer.tree.NoCulture();

            _Writer             = new SerializationWriter();
            _Requests           = new BlockingCollection<APipelinableRequest>(10000);
            _Results            = new ConcurrentDictionary<Guid, APipelinableRequest>();
            _ExecutedWriteTasks = new ConcurrentDictionary<Guid, APipelinableRequest>();

            _CancelToken = myCancelToken;

            var f = new TaskFactory(CancellationToken.None, TaskCreationOptions.LongRunning,
                                    TaskContinuationOptions.None, TaskScheduler.Default);

            _ExecTask = new Task(ExecuteRequests);

            _ExecTask = f.StartNew(ExecuteRequests);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Executes the requests in the blocking collection.
        /// </summary>
        private void ExecuteRequests()
        {
            APipelinableRequest pipelineRequest = null;

            try
            {
                foreach (var item in _Requests.GetConsumingEnumerable())
                {
                    if (_CancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    pipelineRequest = item;

                    pipelineRequest.Execute();

                    if (pipelineRequest.TypeOfRequest == RequestType.read)
                    {
                        _Results.TryAdd(pipelineRequest.ID, pipelineRequest);
                    }
                    else
                    {
                        _ExecutedWriteTasks.TryAdd(pipelineRequest.ID, pipelineRequest);
                    }
                }
            }
            catch(Exception e)
            {
                _CancelToken.Cancel();

                throw e;
            }
        }

        #region Complete

        /// <summary>
        /// Completes a blocking collection
        /// </summary>
        /// <param name="myCollection">The collection that should be completed</param>
        private void Complete(BlockingCollection<APipelinableRequest> myCollection)
        {
            if (myCollection != null)
            {
                myCollection.CompleteAdding();
            }
        }

        #endregion

        /// <summary>
        /// Register requests on the blocking collection.
        /// </summary>
        /// <param name="myRequest">The request to register.</param>
        private void Register(APipelinableRequest myRequest)
        {
            _Requests.Add(myRequest);
        }

        /// <summary>
        /// Validates already executed request.
        /// </summary>
        /// <param name="myInterestingOperation">The id of the request.</param>
        private void ValidateWriteOperations(Guid myInterestingOperation)
        {
            APipelinableRequest interestingRequest;

            while (true)
            {
                if (_ExecutedWriteTasks.TryRemove(myInterestingOperation, out interestingRequest))
                {                    
                    break;
                }

                if (_CancelToken.IsCancellationRequested)
                {
                    break;
                }
            }

            if (interestingRequest != null)
            {
                if (interestingRequest.Exception != null)
                {
                    Shutdown();
                    throw interestingRequest.Exception;
                }                
            }
        }


        /// <summary>
        /// Gets the result
        /// 
        /// If there was an error during validation or execution, the corresponding exception is thrown
        /// </summary>
        /// <param name="myInterestingResult">The id of the pipelineable request</param>
        /// <returns>The result of the request</returns>
        private Object GetResult(Guid myInterestingResult)
        {
            APipelinableRequest interestingRequest;

            while (true)
            {
                if (_Results.TryRemove(myInterestingResult, out interestingRequest))
                {
                    break;
                }

                if (_CancelToken.IsCancellationRequested)
                {
                    break;
                }
            }

            if (interestingRequest != null)
            {
                if (interestingRequest.Exception != null)
                {
                    //throw the exception and let the user handle it

                    throw interestingRequest.Exception;
                }

                return interestingRequest.GetRequest();
            }

            return null;
        }


        #endregion

        #region ISingleValueIndex

        public TValue this[TKey myKey]
        {
            get
            {
                var operation = new GetOperation<TKey, TValue>(_Indexer, myKey);

                Register(operation);

                return (TValue)GetResult(operation.ID);
            }
            set
            {
                var operation = new SetOperation<TKey, TValue>(_Indexer, _Writer, myKey, value);
                
                Register(operation);

                ValidateWriteOperations(operation.ID);
            }
        }
                        
        public void Add(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            var operation = new AddOperation<TKey, TValue>(_Indexer, _Writer, myKey, myValue, myIndexAddStrategy);

            Register(operation);

            ValidateWriteOperations(operation.ID);
        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            var operation = new AddOperation<TKey, TValue>(_Indexer, _Writer, myKeyValuePair, myIndexAddStrategy);
            
            Register(operation);

            ValidateWriteOperations(operation.ID);
        }

        public void Add(IDictionary<TKey, TValue> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            var operation = new AddOperation<TKey, TValue>(_Indexer, _Writer, myDictionary, myIndexAddStrategy);
            
            Register(operation);

            ValidateWriteOperations(operation.ID);
        }

        public ISet<TValue> Values()
        {
            var operation = new ValuesOperation<TKey, TValue>(_Indexer);

            Register(operation);

            return (ISet<TValue>)GetResult(operation.ID);
        }

        public bool IsPersistent
        {
            get { return true; }
        }

        public string Name
        {
            get { return "BufferedBplusDotNetIndex"; }
        }

        public long KeyCount()
        {
            var operation = new KeyCountOperation(_Indexer);
            
            Register(operation);

            return (long)GetResult(operation.ID);
        }

        public long ValueCount()
        {
            var operation = new ValueCountOperation(_Indexer);

            Register(operation);

            return (long)GetResult(operation.ID);
        }

        public ISet<TKey> Keys()
        {
            var operation = new KeysOperation<TKey, TValue>(_Indexer);

            Register(operation);

            return (ISet<TKey>)GetResult(operation.ID);
        }

        public bool ContainsKey(TKey myKey)
        {
            var operation = new ContainsKeyOperation<TKey>(_Indexer, myKey);

            Register(operation);

            return (Boolean)GetResult(operation.ID);
        }

        public bool ContainsValue(TValue myValue)
        {
            var operation = new ContainsValueOperation<TKey, TValue>(_Indexer, myValue);

            Register(operation);

            return (Boolean)GetResult(operation.ID);
        }

        public bool Contains(TKey myKey, TValue myValue)
        {
            var operation = new ContainsOperation<TKey, TValue>(_Indexer, myKey, myValue);

            Register(operation);

            return (Boolean)GetResult(operation.ID);
        }

        public bool Remove(TKey myKey)
        {
            var operation = new RemoveOperation<TKey>(_Indexer, myKey);

            Register(operation);

            return (Boolean)GetResult(operation.ID);
        }

        public void ClearIndex()
        {
            var operation = new ClearIndexOperation(_Indexer, _BlockFileName, _TreeFileName, _PrefixLen);

            Register(operation);

            _Indexer = (xBplusTreeBytes)GetResult(operation.ID);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var operation = new GetEnumeratorOperation<TKey, TValue>(_Indexer);

            Register(operation);

            return (IEnumerator<KeyValuePair<TKey, TValue>>)GetResult(operation.ID);
        }        

        IEnumerator IEnumerable.GetEnumerator()
        {            
            throw new NotImplementedException();
        }

        #endregion

        public void Shutdown()
        {
            Complete(_Requests);
            Task.WaitAll(_ExecTask);
            _Indexer.Shutdown();
        }
    }
}
