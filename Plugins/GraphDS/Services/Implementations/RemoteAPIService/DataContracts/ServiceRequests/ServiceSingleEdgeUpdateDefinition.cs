using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceSingleEdgeUpdateDefinition : ServiceGraphElementUpdateDefinition
    {
        [DataMember]
        public ServiceVertexInformation SourceVertex;
        [DataMember]
        public ServiceVertexInformation TargetVertex;

        [DataMember]
        public Int64 EdgeTypeID;

        [DataContract(Namespace = sonesRPCServer.Namespace)]
        public class ServiceVertexInformation
        {
            [DataMember]
            public String VertexEditionName;
            [DataMember]
            public Int64 VertexID;
            [DataMember]
            public Int64 VertexRevisionID;
            [DataMember]
            public Int64 VertexTypeID;

            public ServiceVertexInformation(Int64 myVertexTypeID, Int64 myVertexID, Int64 myVertexRevisionID = 0L, String myVertexEditionName = null)
            {
                VertexTypeID = myVertexTypeID;
                VertexID = myVertexID;
                VertexRevisionID = myVertexRevisionID;
                VertexEditionName = myVertexEditionName ?? String.Empty;
            }

            public VertexInformation ToVertexInformation()
            {
                return new VertexInformation(VertexTypeID, VertexID, VertexRevisionID, VertexEditionName);
            }
        }

        public SingleEdgeUpdateDefinition ToSingleEdgeUpdateDefinition()
        {
            return new SingleEdgeUpdateDefinition(
                SourceVertex.ToVertexInformation(),
                TargetVertex.ToVertexInformation(),
                EdgeTypeID,
                CommentUpdate,
                new StructuredPropertiesUpdate(UpdatedStructuredProperties, DeletedStructuredProperties),
                new UnstructuredPropertiesUpdate(UpdatedUnstructuredProperties, DeletedUnstructuredProperties));
        }
    }
}
