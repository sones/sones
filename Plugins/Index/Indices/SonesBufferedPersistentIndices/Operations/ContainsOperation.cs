using System;
using BplusDotNet;

namespace sones.Plugins.Index
{
    public sealed class ContainsOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        private readonly xBplusTreeBytes _Indexer;
        private Boolean         _Result;
        private readonly TValue          _Value;
        private readonly TKey            _Key;

        #endregion

        #region Constructors
        
        public ContainsOperation(xBplusTreeBytes myIndexer, TKey myKey, TValue myValue)
        {
            TypeOfRequest = RequestType.read;
            _Indexer = myIndexer;

            _Key = myKey;
            _Value = myValue;            
        }

        #endregion

        #region APipelinableRequest
        
        public override void Execute()
        {
            _Result = false;

            if (_Indexer.tree.ContainsKey(_Key.ToString()))
            {
                var operation = new GetOperation<TKey, TValue>(_Indexer, _Key);

                operation.Execute();

                if (operation.GetRequest().Equals(_Value))
                {
                    _Result = true;
                }
            }            
        }

        public override object GetRequest()
        {
            return _Result;
        }

        #endregion
    }
}
