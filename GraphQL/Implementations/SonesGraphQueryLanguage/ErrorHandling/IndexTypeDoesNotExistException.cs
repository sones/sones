using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class IndexTypeDoesNotExistException : AGraphQLException
    {
        #region data

        public String Info { get; private set; }
        public String Index { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new EdgeTypeDoesNotExistException exception
        /// </summary>
        public IndexTypeDoesNotExistException(String myIndex, String myInfo)
        {
            Info = myInfo;
            Index = myIndex;
            
            _msg = String.Format("Error the IndexType: [{0}] does not exist\n\n{1}", Index, myInfo);
        }

        #endregion
    }
}
