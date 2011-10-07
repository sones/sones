using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceInsertPayload
    {
        internal ServiceInsertPayload(RequestInsertVertex myRequestInsertVertex)
        {
            this.StructuredProperties = myRequestInsertVertex.StructuredProperties.Select(x => new StructuredProperty(x.Key, x.Value)).ToList();
            this.UnstructuredProperties = myRequestInsertVertex.UnstructuredProperties.Select(x => new UnstructuredProperty(x.Key, x.Value)).ToList();
            this.UUID = myRequestInsertVertex.VertexUUID.Value;
            this.Comment = myRequestInsertVertex.Comment;
            this.Edition = myRequestInsertVertex.Edition;
            this.Edges = myRequestInsertVertex.OutgoingEdges.Select(x => new ServiceEdgePredefinition(x)).ToList();
        }
    }
}
