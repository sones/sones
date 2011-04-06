using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;

namespace sones.Plugins.Index
{
    public sealed class RemoveOperation<TKey> : APipelinableRequest
    {
        #region Data
        
        private xBplusTreeBytes _Indexer;
        private Boolean         _Result;
        private TKey            _Key;

        #endregion

        #region Constructors
        
        public RemoveOperation(xBplusTreeBytes myIndexer, TKey myKey)
        {
            _Result         = true;
            TypeOfRequest   = RequestType.read;
            _Indexer        = myIndexer;
            _Key            = myKey;
        }

        #endregion

        #region APipelinableRequest
        
        public override void Execute()
        {
            _Indexer.tree.RemoveKey(_Key.ToString());
        }

        public override object GetRequest()
        {
            return _Result;
        }

        #endregion
    }
}
