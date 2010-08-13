using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.NewAPI
{

    public class Set<TVertexType> : HashSet<TVertexType> 
    {
    }

    public class Set<TVertexType, TEdgeType> : HashSet<TVertexType>
        where TVertexType : DBObject
        where TEdgeType   : DBEdge
    {
        //ToDo: Implement TEdgeType
    }

}
