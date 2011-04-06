using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;

namespace sones.Plugins.Index
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class ContainsKeyOperation<TKey> : APipelinableRequest
    {
        #region Data
        
        private xBplusTreeBytes _Indexer;
        private Boolean         _Result;
        private TKey            _Key;

        #endregion

        #region Constructors
        
        public ContainsKeyOperation(xBplusTreeBytes myIndexer, TKey myKey)
        {
            TypeOfRequest = RequestType.read;
            _Indexer = myIndexer;
            _Key = myKey;            
        }

        #endregion

        #region APipelinableRequest
        
        public override void Execute()
        {
            _Result = false;
            _Result = _Indexer.tree.ContainsKey(_Key.ToString());
        }

        public override object GetRequest()
        {
            return _Result;
        }

        #endregion
    }
}
