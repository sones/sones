using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// An IncomingEdge type does not exist
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
            _msg = String.Format("The edgetype \"{0}\" does not exist!", EdgeType);
        }       
 
    }
}
