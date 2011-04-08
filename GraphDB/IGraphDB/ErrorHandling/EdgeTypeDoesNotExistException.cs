using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// An edge type does not exist
    /// </summary>
    public sealed class EdgeTypeDoesNotExistException : AGraphDBException
    {
        public String EdgeType { get; private set; }

        /// <summary>
        /// Creates a new EdgeTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myEdgeType"></param>
        public EdgeTypeDoesNotExistException(String myEdgeType)
        {
            EdgeType = myEdgeType;            
            _msg = String.Format("{0} : The edgetype \"{1}\" does not exist!", EdgeType);
        }       
 
    }
}
