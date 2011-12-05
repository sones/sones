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
            this.VertexTypeName = myRequestInsertVertex.VertexTypeName;
            this.UUID = myRequestInsertVertex.VertexUUID;
            this.Comment = myRequestInsertVertex.Comment;
            this.Edition = myRequestInsertVertex.Edition;
            
            this.StructuredProperties = (myRequestInsertVertex.StructuredProperties == null)
                ? null : myRequestInsertVertex.StructuredProperties.Select(x => new StructuredProperty(x.Key, x.Value)).ToList();

            this.UnstructuredProperties = (myRequestInsertVertex.UnstructuredProperties == null)
                ? null : myRequestInsertVertex.UnstructuredProperties.Select(x => new UnstructuredProperty(x.Key, x.Value)).ToList();

            this.Edges = (myRequestInsertVertex.OutgoingEdges == null)
                ? null : myRequestInsertVertex.OutgoingEdges.Select(x => new ServiceEdgePredefinition(x)).ToList();
        }
    }
}
