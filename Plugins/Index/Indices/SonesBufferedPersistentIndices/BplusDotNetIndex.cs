#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using sones.Plugins.Index.Interfaces;
using sones.Plugins.Index.Helper;
using BplusDotNet;
using sones.Library.Serializer;
using sones.Library.NewFastSerializer;
using sones.Plugins.Index.ErrorHandling;

#endregion

namespace sones.Plugins.Index
{
    /// <summary>
    /// This class realize a persistent single value index.
    /// </summary>
    /// <typeparam name="TKey">The type of the index key.</typeparam>
    /// <typeparam name="TValue">The type of the index value.</typeparam>
    public sealed class BplusDotNetIndex<TKey, TValue> : ISingleValueIndex<TKey, TValue>         
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
        private SerializationWriter                             _Writer;

        /// <summary>
        /// The tree file name. 
        /// </summary>
        private String                                          _TreeFileName;

        /// <summary>
        /// The block file name.
        /// </summary>
        private String                                          _BlockFileName;

        /// <summary>
        /// The prefix length for the tree.
        /// </summary>
        private Int32                                           _PrefixLen;        

        private BlockingCollection<APipelinableRequest>         _Requests;

        private Task                                            _ExecTask;

        private CancellationTokenSource                         _CancelToken;

        private ConcurrentDictionary<Guid, APipelinableRequest> _Results;

        #endregion

        #region Constructors

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="myTreeFileName">The name for the tree file.</param>
        /// <param name="myBlockFileName">The name for the block file.</param>
        /// <param name="myPrefixLength">The prefix length for the tree.</param>
        public BplusDotNetIndex(String myTreeFileName, String myBlockFileName, Int32 myPrefixLength, CancellationTokenSource myCancelToken)
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

            _Writer   = new SerializationWriter();
            _Requests = new BlockingCollection<APipelinableRequest>(10000);
            _Results  = new ConcurrentDictionary<Guid, APipelinableRequest>();

            _CancelToken = myCancelToken;

            var f = new TaskFactory(CancellationToken.None, TaskCreationOptions.LongRunning,
                                    TaskContinuationOptions.None, TaskScheduler.Default);

            _ExecTask = new Task(ExecuteRequests);

            _ExecTask = f.StartNew(ExecuteRequests);
            
        }

        #endregion

        #region Private Helpers

        private void ExecuteRequests()
        {
            APipelinableRequest pipelineRequest;

            try
            {
                foreach (var item in _Requests.GetConsumingEnumerable())
                {
                    item.Execute();

                    if (_CancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    pipelineRequest = item;

                    ExecuteRequest(ref pipelineRequest);

                    if (pipelineRequest.TypeOfRequest == RequestType.read)
                    {
                        _Results.TryAdd(pipelineRequest.ID, pipelineRequest);
                    }
                }
            }
            catch(Exception e)
            {
                _CancelToken.Cancel();

                if (!(e is OperationCanceledException))
                    throw;
            }
        }        

        private void ExecuteRequest(ref APipelinableRequest myRequest)
        {
            try
            {
                myRequest.Execute();
            }
            catch (Exception e)
            {
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

        private void Register(APipelinableRequest myRequest)
        {
            /*if (myRequest.Writeable)
	        {
		        _failoverFile.AppendLine(myRequest.Serialize())
	        }*/

            _Requests.Add(myRequest);
        }

        /// <summary>
        /// Deserialize an key and a value.
        /// </summary>
        /// <param name="myBytes">The serialized key and value.</param>
        /// <param name="myKey">The key.</param>
        /// <param name="myValue">The value.</param>
        private void DeserializeObject(byte[] myBytes, out TKey myKey, out TValue myValue)
        {            
            SerializationReader reader;           
            
            reader = new SerializationReader(myBytes);            
                        
            myKey = (TKey)reader.ReadObject();
            myValue = (TValue)reader.ReadObject();

            reader.Close();
            reader.Dispose();
            reader = null;
        }

        #endregion

        #region ISingleValueIndex

        public TValue this[TKey myKey]
        {
            get
            {
                byte[] value;

                lock (_Indexer)
                {
                    if (_Indexer.ContainsKey(myKey.ToString()))
                    {
                        value = _Indexer[myKey.ToString()];
                    }
                    else
                    {
                        return default(TValue);
                    }
                }
                
                TKey key;
                TValue result;

                DeserializeObject(value, out key, out result);

                return result;
            }
            set
            {                
                /*lock(_Indexer)
                {
                    _Indexer[myKey.ToString()] = SerializeObject(myKey, value);
                }*/
            }
        }
                        
        public void Add(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            Register(new AddOperation<TKey, TValue>(_Indexer, _Writer, myKey, myValue, myIndexAddStrategy));

            //AddValues(myKey, myValue, myIndexAddStrategy);
        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            Register(new AddOperation<TKey, TValue>(_Indexer, _Writer, myKeyValuePair, myIndexAddStrategy));
            //AddValues(myKeyValuePair.Key, myKeyValuePair.Value, myIndexAddStrategy);
        }

        public void Add(IDictionary<TKey, TValue> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            /*foreach (var item in myDictionary)
            {
                AddValues(item.Key, item.Value, myIndexAddStrategy);
            }*/

            Register(new AddOperation<TKey, TValue>(_Indexer, _Writer, myDictionary, myIndexAddStrategy));
        }

        public ISet<TValue> Values()
        {
            String nextKey = String.Empty;
            var result = new HashSet<TValue>();
            byte[] value;
            
            TKey key;
            TValue desObject;

            lock (_Indexer)
            {
                nextKey = _Indexer.tree.FirstKey();

                while (!String.IsNullOrEmpty(nextKey))
                {
                    value = _Indexer[nextKey];
                    DeserializeObject(value, out key, out desObject);
                    result.Add(desObject);
                    nextKey = _Indexer.tree.NextKey(nextKey);
                }
            }

            return result;
        }

        public bool IsPersistent
        {
            get { return true; }
        }

        public string Name
        {
            get { return "BplusDotNetIndex"; }
        }

        public long KeyCount()
        {
            String nextKey = String.Empty;
            long cnt = 0;

            lock (_Indexer)
            {
                nextKey = _Indexer.tree.FirstKey();

                while (!String.IsNullOrEmpty(nextKey))
                {   
                    nextKey = _Indexer.tree.NextKey(nextKey);
                    cnt++;
                }
            }

            return cnt;            
        }

        public long ValueCount()
        {
            String nextKey = String.Empty;
            long cnt = 0;

            lock (_Indexer)
            {
                nextKey = _Indexer.tree.FirstKey();

                while (!String.IsNullOrEmpty(nextKey))
                {   
                    nextKey = _Indexer.tree.NextKey(nextKey);
                    cnt++;
                }
            }

            return cnt;
        }

        public ISet<TKey> Keys()
        {
            String nextKey = String.Empty;
            var result = new HashSet<TKey>();
            byte[] value;

            TKey key;
            TValue desObject;

            lock (_Indexer)
            {
                nextKey = _Indexer.tree.FirstKey();

                while (!String.IsNullOrEmpty(nextKey))
                {
                    value = _Indexer[nextKey];
                    DeserializeObject(value, out key, out desObject);
                    result.Add(key);

                    nextKey = _Indexer.tree.NextKey(nextKey);

                }
            }
            
            return result;
        }

        public bool ContainsKey(TKey myKey)
        {
            lock (_Indexer)
            {
                return _Indexer.tree.ContainsKey(myKey.ToString());
            }
        }

        public bool ContainsValue(TValue myValue)
        {
            String nextKey = String.Empty;
            var result = new HashSet<TValue>();
            byte[] value;

            TKey key;
            TValue desObject;

            lock (_Indexer)
            {
                nextKey = _Indexer.tree.FirstKey();

                while (!String.IsNullOrEmpty(nextKey))
                {                   
                    value = _Indexer[nextKey];

                    DeserializeObject(value, out key, out desObject);

                    if (desObject.Equals(myValue))
                    {
                        return true;
                    }
                    
                    nextKey = _Indexer.tree.NextKey(nextKey);
                }
            }

            return false;
        }

        public bool Contains(TKey myKey, TValue myValue)
        {
            lock (_Indexer)
            {
                if (_Indexer.tree.ContainsKey(myKey.ToString()))
                {
                    if (this[myKey].Equals(myValue))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Remove(TKey myKey)
        {
            lock(_Indexer)
            {
                _Indexer.tree.RemoveKey(myKey.ToString());
            }

            return true;
        }

        public void ClearIndex()
        {
            if (File.Exists(_BlockFileName) && File.Exists(_TreeFileName))
            {
                lock (_Indexer)
                {
                    _Indexer.Shutdown();

                    File.Delete(_BlockFileName);
                    File.Delete(_TreeFileName);
                
                    _Indexer = xBplusTreeBytes.Initialize(_BlockFileName, _TreeFileName, _PrefixLen);
                }  
            }                      
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var result = new List<KeyValuePair<TKey, TValue>>();
            String nextKey = String.Empty;            
            byte[] value;

            TKey key;
            TValue desObject;

            lock (_Indexer)
            {
                nextKey = _Indexer.tree.FirstKey();

                while(!String.IsNullOrEmpty(nextKey))
                {
                    value = _Indexer[nextKey];

                    DeserializeObject(value, out key, out desObject);
                    result.Add(new KeyValuePair<TKey, TValue>(key, desObject));
                    nextKey = _Indexer.tree.NextKey(nextKey);
                } 
            }

            return result.GetEnumerator();
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

            lock (_Indexer)
            {
                _Indexer.Shutdown();
            }
        }
    }
}
