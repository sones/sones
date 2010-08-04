using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDS.API.CSharp.Reflection
{

    public class TraversalState
    {
        public DBObject StartNode               { get; private set; }
        public UInt64   Depth                   { get; private set; }
        public UInt64   NumberOfVisitedVertices { get; private set; }
        public UInt64   NumberOfVisitedEdges    { get; private set; }
        public UInt64   NumberOfFoundPaths      { get; private set; }
    }

}
