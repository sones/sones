using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;
using sones.Library.NewFastSerializer;

namespace sones.Plugins.Index
{
    public sealed class ContainsValueOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        private xBplusTreeBytes  _Indexer;
        private Boolean          _Result;
        private TValue           _Value;

        #endregion

        #region Constructors
        
        public ContainsValueOperation(xBplusTreeBytes myIndexer, TValue myValue)
        {
            TypeOfRequest = RequestType.read;
            _Indexer = myIndexer;
            _Value = myValue;            
        }

        #endregion

        #region APipelinableRequest
        
        public override void Execute()
        {
            String nextKey = String.Empty;
            var result = new HashSet<TValue>();
            byte[] value;            

            TKey key;
            TValue desObject;
            
            nextKey = _Indexer.tree.FirstKey();
            _Result = false;

            while (!String.IsNullOrEmpty(nextKey))
            {
                value = _Indexer[nextKey];

                DeserializeObject(value, out key, out desObject);

                if (desObject.Equals(_Value))
                {
                    _Result = true;
                }

                nextKey = _Indexer.tree.NextKey(nextKey);
            }            
        }

        public override object GetRequest()
        {
            return _Result;
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
