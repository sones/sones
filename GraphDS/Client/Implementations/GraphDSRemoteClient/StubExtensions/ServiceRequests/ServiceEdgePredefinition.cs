using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceEdgePredefinition
    {
        internal ServiceEdgePredefinition(EdgePredefinition myEdgePredefinition)
        {
            this.EdgeName = myEdgePredefinition.EdgeName;
            this.ContainedEdges = myEdgePredefinition.ContainedEdges.Select(x => new ServiceEdgePredefinition(x)).ToList();
            this.Comment = myEdgePredefinition.Comment;
            this.VertexIDsByID = myEdgePredefinition.VertexIDsByVertexTypeID.Select(x => new Tuple<Int64, List<Int64>>(x.Key, x.Value.ToList())).ToList();
            this.StructuredProperties = myEdgePredefinition.StructuredProperties.Select(x => new StructuredProperty(x.Key, x.Value)).ToList();
            this.UnstructuredProperties = myEdgePredefinition.UnstructuredProperties.Select(x => new UnstructuredProperty(x.Key, x.Value)).ToList();
        }
    }
}
