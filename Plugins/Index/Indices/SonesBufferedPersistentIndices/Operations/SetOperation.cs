using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;
using sones.Library.NewFastSerializer;

namespace sones.Plugins.Index
{
    public sealed class SetOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        private xBplusTreeBytes         _Indexer;
        private SerializationWriter     _Writer;
        private TValue                  _Value;
        private TKey                    _Key;

        #endregion

        #region Constructors
        
        public SetOperation(xBplusTreeBytes myIndexer, SerializationWriter myWriter, TKey myKey, TValue myValue)
        {
            _Indexer        = myIndexer;
            _Writer         = myWriter;
            TypeOfRequest   = RequestType.write;
            _Key            = myKey;
            _Value          = myValue;
        }

        #endregion

        #region APipelinableRequest
        
        public override void Execute()
        {
            _Indexer[_Key.ToString()] = SerializeObject(_Key, _Value);
        }

        public override object GetRequest()
        {
            return null;
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

        #endregion
    }
}
