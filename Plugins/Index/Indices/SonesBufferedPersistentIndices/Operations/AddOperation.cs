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
    /// <summary>
    /// This class realize an add operation on the buffered persistent index.
    /// </summary>
    /// <typeparam name="TKey">The key.</typeparam>
    /// <typeparam name="TValue">The value.</typeparam>
    public sealed class AddOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        /// <summary>
        /// The internal index structure.
        /// </summary>
        private xBplusTreeBytes             _Indexer;

        /// <summary>
        /// The internal serialization writer.
        /// </summary>
        private SerializationWriter         _Writer;

        /// <summary>
        /// The key and values, which are to insert.
        /// </summary>
        private Dictionary<TKey, TValue>    _Inserts;

        /// <summary>
        /// The index add strategy.
        /// </summary>
        private IndexAddStrategy            _AddStrategy;

        #endregion

        #region Constructors
        
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="myAddStrategy"></param>
        private AddOperation(IndexAddStrategy myAddStrategy)
        {
            TypeOfRequest = RequestType.write;
            _AddStrategy = myAddStrategy;
        }
        
        /// <summary>
        /// The constructor for an key and value pair insert operation.
        /// </summary>
        /// <param name="myIndexer">The internal index structure.</param>
        /// <param name="myWriter">The internal serialization writer.</param>
        /// <param name="myKey">The key to insert.</param>
        /// <param name="myValue">The value to insert.</param>
        /// <param name="myAddStrategy">The index insert strategy.</param>
        public AddOperation(xBplusTreeBytes myIndexer, SerializationWriter myWriter, TKey myKey, TValue myValue, IndexAddStrategy myAddStrategy) 
            : this(myAddStrategy)
        {            
            _Indexer = myIndexer;
            _Writer = myWriter;
            _Inserts = new Dictionary<TKey, TValue>();            
            _Inserts.Add(myKey, myValue);
        }

        /// <summary>
        /// The constructor for an key value pair insert operation.
        /// </summary>
        /// <param name="myIndexer">The internal index structure.</param>
        /// <param name="myWriter">The internal serialization writer.</param>
        /// <param name="myKey">The key to insert.</param>
        /// <param name="myValue">The value to insert.</param>
        /// <param name="myAddStrategy">The index insert strategy.</param>
        public AddOperation(xBplusTreeBytes myIndexer, SerializationWriter myWriter, KeyValuePair<TKey, TValue> myValues, IndexAddStrategy myAddStrategy)
            : this(myAddStrategy)
        {
            _Indexer = myIndexer;
            _Writer = myWriter;
            _Inserts = new Dictionary<TKey, TValue>();

            _Inserts.Add(myValues.Key, myValues.Value);
        }

        /// <summary>
        ///  The constructor for an dictionary insert operation.
        /// </summary>
        /// <param name="myIndexer">The internal index structure.</param>
        /// <param name="myWriter">The internal serialization writer.</param>
        /// <param name="myKey">The key to insert.</param>
        /// <param name="myValue">The value to insert.</param>
        /// <param name="myAddStrategy">The index insert strategy.</param>
        public AddOperation(xBplusTreeBytes myIndexer, SerializationWriter myWriter, IDictionary<TKey, TValue> myValues, IndexAddStrategy myAddStrategy)
            : this(myAddStrategy)
        {
            _Indexer = myIndexer;
            _Writer = myWriter;
            _Inserts = new Dictionary<TKey, TValue>(myValues);
        }

        #endregion
        

        #region Private Helpers

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
                        Exception = new UniqueIndexConstraintException(String.Format("Index key {0} already exist.", myKey.ToString()));
                    }
                    else
                    {                        
                        _Indexer.Set(myKey.ToString(), SerializeObject(myKey, myValue));
                        _Indexer.Commit();
                    }                    

                    break;
            }
        }

        #endregion

        #region APipelinableRequest

        public override void Execute()
        {
            foreach (var item in _Inserts)
            {
                AddValues(item.Key, item.Value, _AddStrategy);
            }
        }

        public override Object GetRequest()
        {
            return null;
        }

        #endregion
    }
}
