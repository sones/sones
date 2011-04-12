using System;
using BplusDotNet;

namespace sones.Plugins.Index
{
    public sealed class RemoveOperation<TKey> : APipelinableRequest
    {
        #region Data
        
        private readonly xBplusTreeBytes _Indexer;
        private readonly Boolean         _Result;
        private readonly TKey            _Key;

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
