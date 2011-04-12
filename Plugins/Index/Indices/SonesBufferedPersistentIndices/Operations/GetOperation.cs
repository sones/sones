using BplusDotNet;
using sones.Library.NewFastSerializer;

namespace sones.Plugins.Index
{
    public sealed class GetOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        private readonly xBplusTreeBytes    _Indexer;
        private TValue             _Value;
        private readonly TKey               _Key;

        #endregion


        #region Constructors
        
        public GetOperation(xBplusTreeBytes myIndexer, TKey myKey)
        {
            _Indexer = myIndexer;
            TypeOfRequest = RequestType.read;
            _Key = myKey;
        }

        #endregion

        #region APipelinableRequest
        

        public override void Execute()
        {
            byte[] value;

            if (_Indexer.ContainsKey(_Key.ToString()))
            {
                TKey key;
                TValue result;

                value = _Indexer[_Key.ToString()];

                DeserializeObject(value, out key, out result);

                _Value = result;
            }
            else
            {
                _Value = default(TValue);
            }
        }

        public override object GetRequest()
        {
            return _Value;
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
