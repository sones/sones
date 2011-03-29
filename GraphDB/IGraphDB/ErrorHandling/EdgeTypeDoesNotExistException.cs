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
        }

        public override string ToString()
        {
            return String.Format("The edgetype \"{0}\" does not exist!", EdgeType);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.EdgeTypeDoesNotExist; }
        }
    }
}
