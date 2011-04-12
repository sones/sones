using System;
using System.Collections.Generic;
using BplusDotNet;
using sones.Library.NewFastSerializer;

namespace sones.Plugins.Index
{
    public sealed class KeysOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        private readonly xBplusTreeBytes _Indexer;
        private HashSet<TKey>   _Result;

        #endregion

        #region Constructors
        
        public KeysOperation(xBplusTreeBytes myIndexer)
        {
            TypeOfRequest = RequestType.read;
            _Indexer = myIndexer;
        }

        #endregion

        #region APipelinableRequest
        

        public override void Execute()
        {
            String nextKey = String.Empty;
            _Result = new HashSet<TKey>();
            byte[] value;

            TKey key;
            TValue desObject;

            nextKey = _Indexer.tree.FirstKey();

            while (!String.IsNullOrEmpty(nextKey))
            {
                value = _Indexer[nextKey];
                DeserializeObject(value, out key, out desObject);
                _Result.Add(key);

                nextKey = _Indexer.tree.NextKey(nextKey);
            }
                        
        }

        public override object GetRequest()
        {
            return (Object)_Result;
        }

        #endregion

        #region Private Helpers
        
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
    }
}
