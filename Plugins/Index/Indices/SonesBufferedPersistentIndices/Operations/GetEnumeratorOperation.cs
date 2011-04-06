using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;
using sones.Library.NewFastSerializer;

namespace sones.Plugins.Index
{
    public sealed class GetEnumeratorOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        private xBplusTreeBytes                         _Indexer;
        private List<KeyValuePair<TKey, TValue>>        _Result;

        #endregion

        #region Constructors
        
        public GetEnumeratorOperation(xBplusTreeBytes myIndexer)
        {
            _Indexer        = myIndexer;
            TypeOfRequest   = RequestType.read;
        }

        #endregion

        #region APipelinableRequest
        

        public override void Execute()
        {
            String nextKey = String.Empty;
            byte[] value;

            TKey key;
            TValue desObject;
            
            nextKey = _Indexer.tree.FirstKey();
            _Result = new List<KeyValuePair<TKey, TValue>>();

            while (!String.IsNullOrEmpty(nextKey))
            {
                value = _Indexer[nextKey];

                DeserializeObject(value, out key, out desObject);
                _Result.Add(new KeyValuePair<TKey, TValue>(key, desObject));
                nextKey = _Indexer.tree.NextKey(nextKey);
            }
            
        }

        public override object GetRequest()
        {
            return _Result.GetEnumerator();
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
