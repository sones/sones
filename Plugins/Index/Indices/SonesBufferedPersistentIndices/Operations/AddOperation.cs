using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;
using sones.Plugins.Index;
using sones.Plugins.Index.Helper;
using sones.Library.NewFastSerializer;
using sones.Plugins.Index.ErrorHandling;

namespace sones.Plugins.Index
{
    public class AddOperation<TKey, TValue> : APipelinableRequest
    {
        private xBplusTreeBytes             _Indexer;
        private SerializationWriter         _Writer;
        private Dictionary<TKey, TValue>    _Inserts;
        private IndexAddStrategy            _AddStrategy;


        public AddOperation(IndexAddStrategy myAddStrategy)
        {
            TypeOfRequest = RequestType.write;
            _AddStrategy = myAddStrategy;
        }
        
        public AddOperation(xBplusTreeBytes myIndexer, SerializationWriter myWriter, TKey myKey, TValue myValue, IndexAddStrategy myAddStrategy) 
            : this(myAddStrategy)
        {            
            _Indexer = myIndexer;
            _Writer = myWriter;
            _Inserts = new Dictionary<TKey, TValue>();            
            _Inserts.Add(myKey, myValue);
        }

        public AddOperation(xBplusTreeBytes myIndexer, SerializationWriter myWriter, KeyValuePair<TKey, TValue> myValues, IndexAddStrategy myAddStrategy)
            : this(myAddStrategy)
        {
            _Indexer = myIndexer;
            _Writer = myWriter;
            _Inserts = new Dictionary<TKey, TValue>();

            _Inserts.Add(myValues.Key, myValues.Value);
        }

        public AddOperation(xBplusTreeBytes myIndexer, SerializationWriter myWriter, IDictionary<TKey, TValue> myValues, IndexAddStrategy myAddStrategy)
            : this(myAddStrategy)
        {
            _Indexer = myIndexer;
            _Writer = myWriter;
            _Inserts = new Dictionary<TKey, TValue>(myValues);
        }

        /// <summary>
        /// Serialize an key and a value.
        /// </summary>
        /// <param name="myKey">The key.</param>
        /// <param name="myValue">The value.</param>
        /// <returns>The serialized keys and values.</returns>
        private byte[] SerializeObject(TKey myKey, TValue myValue)
        {            
            _Writer.ResetBuffer();
            _Writer.WriteObject(myKey);
            _Writer.WriteObject(myValue);
            _Writer.Flush();
            return _Writer.ToArray();
        }

        /// <summary>
        /// Adds values to the internal data structure.
        /// </summary>
        /// <param name="myKey">The index key.</param>
        /// <param name="myValues">The index value.</param>
        /// <param name="myIndexAddStrategy">The add strategy.</param>
        private void AddValues(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy)
        {
            switch (myIndexAddStrategy)
            {
                case IndexAddStrategy.MERGE:

                case IndexAddStrategy.REPLACE:
                    
                    _Indexer.Set(myKey.ToString(), SerializeObject(myKey, myValue));
                    _Indexer.Commit();

                    break;

                case IndexAddStrategy.UNIQUE:
                    
                    if (_Indexer.tree.ContainsKey(myKey.ToString()))
                    {
                        throw new UniqueIndexConstraintException(String.Format("Index key {0} already exist.", myKey.ToString()));
                    }
                    else
                    {                        
                        _Indexer.Set(myKey.ToString(), SerializeObject(myKey, myValue));
                        _Indexer.Commit();
                    }                    

                    break;
            }
        }        

        public override void Execute()
        {
            foreach (var item in _Inserts)
            {
                AddValues(item.Key, item.Value, _AddStrategy);
            }
        }

        public override APipelinableRequest GetRequest(Guid myInterestingResult)
        {
            return null;
        }
    }
}
