using System;
using BplusDotNet;

namespace sones.Plugins.Index
{
    public sealed class KeyCountOperation : APipelinableRequest
    {
        #region Data
        
        private readonly xBplusTreeBytes _Indexer;
        private Int64           _Cnt;

        #endregion

        #region Constructors
        
        public KeyCountOperation(xBplusTreeBytes myIndexer)
        {
            TypeOfRequest = RequestType.read;
            _Indexer = myIndexer;
        }

        #endregion

        #region APipelinableRequest
        

        public override void Execute()
        {
            String nextKey = String.Empty;
            
            nextKey = _Indexer.tree.FirstKey();
            _Cnt = 0;

            while (!String.IsNullOrEmpty(nextKey))
            {
                nextKey = _Indexer.tree.NextKey(nextKey);
                _Cnt++;
            }            
        }

        public override Object GetRequest()
        {
            return (Object)_Cnt;
        }

        #endregion
    }
}
