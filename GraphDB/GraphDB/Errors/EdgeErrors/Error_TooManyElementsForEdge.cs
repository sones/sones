using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.EdgeTypes;

namespace sones.GraphDB.Errors
{
    public class Error_TooManyElementsForEdge : GraphDBEdgeError
    {
        public UInt64 CurrentElements { get; private set; }
        public IEdgeType EdgeType { get; private set; }

        public Error_TooManyElementsForEdge(IEdgeType edgeType, UInt64 currentElements)
        {
            CurrentElements = currentElements;
            EdgeType = edgeType;
        }

        public override string ToString()
        {
            return String.Format("The edge [{0}] does not take {1} elements.", EdgeType.EdgeTypeName, CurrentElements);
        }
    }
}
