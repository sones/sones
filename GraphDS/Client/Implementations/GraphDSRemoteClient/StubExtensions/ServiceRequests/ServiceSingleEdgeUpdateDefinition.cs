using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceSingleEdgeUpdateDefinition
    {
        internal ServiceSingleEdgeUpdateDefinition(SingleEdgeUpdateDefinition myUpdateDefinition)
        {
            this.CommentUpdate = myUpdateDefinition.CommentUpdate;
            this.EdgeTypeID = myUpdateDefinition.EdgeTypeID;
            this.UpdatedStructuredProperties = myUpdateDefinition.UpdatedStructuredProperties.Updated.ToDictionary(k => k.Key, v => (object)v.Value);
            this.DeletedStructuredProperties = myUpdateDefinition.UpdatedStructuredProperties.Deleted.ToList();
            this.UpdatedUnstructuredProperties = myUpdateDefinition.UpdatedUnstructuredProperties.Updated.ToDictionary(k => k.Key, v => (object)v.Value);
            this.DeletedUnstructuredProperties = myUpdateDefinition.UpdatedUnstructuredProperties.Deleted.ToList();
            this.SourceVertex = new ServiceVertexInformation(myUpdateDefinition.SourceVertex);
            this.TargetVertex = new ServiceVertexInformation(myUpdateDefinition.TargetVertex);
        }

        public partial class ServiceVertexInformation
        {
            internal ServiceVertexInformation(VertexInformation myVertexInformation)
            {
                VertexTypeIDField = myVertexInformation.VertexTypeID;
                VertexIDField = myVertexInformation.VertexID;
                VertexRevisionIDField = myVertexInformation.VertexRevisionID;
                VertexEditionNameField = myVertexInformation.VertexEditionName ?? String.Empty;
            }
        }
    }
}
