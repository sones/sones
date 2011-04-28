using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class VertexTypeDoesNotExistException : AGraphQLException
    {
         #region data

        public String Info { get; private set; }
        public String Vertex { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new EdgeTypeDoesNotExistException exception
        /// </summary>
        public VertexTypeDoesNotExistException(String myVertex, String myInfo)
        {
            Info = myInfo;
            Vertex = myVertex;
            
            _msg = String.Format("Error the vertexType: [{0}] does not exist\n\n{1}", Vertex, myInfo);
        }

        #endregion
    }
}
