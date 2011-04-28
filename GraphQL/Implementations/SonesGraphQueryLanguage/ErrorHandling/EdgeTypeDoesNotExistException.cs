using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class EdgeTypeDoesNotExistException : AGraphQLException
    {
        #region data

        public String Info { get; private set; }
        public String Edge { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new EdgeTypeDoesNotExistException exception
        /// </summary>
        public EdgeTypeDoesNotExistException(String myEdge, String myInfo)
        {
            Info = myInfo;
            Edge = myEdge;
            
            _msg = String.Format("Error the edge: [{0}] does not exist\n\n{1}", Edge, myInfo);
        }

        #endregion
    }
}
