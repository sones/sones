using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BplusDotNet;
using System.IO;

namespace sones.Plugins.Index
{
    /// <summary>
    /// This class realize an clear index operation on the buffered persistent index.
    /// </summary>
    public sealed class ClearIndexOperation : APipelinableRequest
    {

        #region Data

        /// <summary>
        /// The internal index structure.
        /// </summary>
        private xBplusTreeBytes _Indexer;

        /// <summary>
        /// The block file name, that is used on the bplus tree.
        /// </summary>
        private String          _BlockFileName;

        /// <summary>
        /// The tree file name, that is used on the bplus tree.
        /// </summary>
        private String          _TreeFileName;

        /// <summary>
        /// The prefix length, that is used on the bplus tree.
        /// </summary>
        private Int32           _PrefixLen;

        #endregion

        #region Constructors
        
        /// <summary>
        /// The class constructor to register the operation on the buffered persistent index. 
        /// </summary>
        /// <param name="myIndexer">The internal index structure.</param>
        /// <param name="myBlockFile">The block file name, that is used on the bplus tree.</param>
        /// <param name="myTreeFile">The tree file name, that is used on the bplus tree.</param>
        /// <param name="myPrefixLen">The prefix length, that is used on the bplus tree.</param>
        public ClearIndexOperation(xBplusTreeBytes myIndexer, String myBlockFile, String myTreeFile, Int32 myPrefixLen)
        {
            _Indexer        = myIndexer;
            TypeOfRequest = RequestType.read;
            _BlockFileName  = myBlockFile;
            _TreeFileName   = myTreeFile;
            _PrefixLen      = myPrefixLen;
        }

        #endregion

        #region APipelinableRequest
        
        public override void Execute()
        {
            if (File.Exists(_BlockFileName) && File.Exists(_TreeFileName))
            {                
                _Indexer.Shutdown();

                File.Delete(_BlockFileName);
                File.Delete(_TreeFileName);

                _Indexer = xBplusTreeBytes.Initialize(_BlockFileName, _TreeFileName, _PrefixLen);
                
            }                      
        }

        public override object GetRequest()
        {
            return _Indexer;
        }

        #endregion
    }
}
