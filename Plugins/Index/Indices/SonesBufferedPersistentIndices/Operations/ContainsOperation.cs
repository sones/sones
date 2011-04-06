using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;

namespace sones.Plugins.Index
{
    public sealed class ContainsOperation<TKey, TValue> : APipelinableRequest
    {
        #region Data
        
        private xBplusTreeBytes _Indexer;
        private Boolean         _Result;
        private TValue          _Value;
        private TKey            _Key;

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
