using System;
using BplusDotNet;

namespace sones.Plugins.Index
{
    public sealed class ValueCountOperation : APipelinableRequest
    {
        #region Data
        
        private readonly xBplusTreeBytes _Indexer;
        private Int64 _Cnt;

        #endregion

        #region Constructors
        

        public ValueCountOperation(xBplusTreeBytes myIndexer)
        {
            TypeOfRequest = RequestType.read;
            _Indexer = myIndexer;            
        }

        #endregion

        #region APipelinableRequest
        
        public override void Execute()
        {
            String nextKey = String.Empty;
            _Cnt = 0;
            
            nextKey = _Indexer.tree.FirstKey();

            while (!String.IsNullOrEmpty(nextKey))
            {
                nextKey = _Indexer.tree.NextKey(nextKey);
                _Cnt++;
            }            
        }

        public override object GetRequest()
        {
            return (Object)_Cnt;
        }

        #endregion
    }
}
