using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.NewAPI
{

    public class TraversalState
    {
        public DBVertex StartNode               { get; private set; }
        public UInt64   Depth                   { get; private set; }
        public UInt64   NumberOfVisitedVertices { get; private set; }
        public UInt64   NumberOfVisitedEdges    { get; private set; }
        public UInt64   NumberOfFoundPaths      { get; private set; }
    }

}
