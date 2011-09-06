using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InsertPayload;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceEdgePredefinition
    {
        [DataMember]
        public String EdgeName;

        [DataMember]
        public List<ServiceEdgePredefinition> ContainedEdges;

        [DataMember]
        public String Comment;

        [DataMember]
        public IDictionary<Int64,Int64> VertexIDsByID;

        [DataMember]
        public List<StructuredProperty> StructuredProperties;

        [DataMember]
        public List<UnstructuredProperty> UnstructuredProperties;


        public EdgePredefinition ToEdgePredefinition()
        {
            EdgePredefinition EdgePredef = new EdgePredefinition(this.EdgeName);
            if (!String.IsNullOrEmpty(Comment))
                EdgePredef.Comment = this.Comment;

            foreach (var Edge in this.ContainedEdges)
            {
                EdgePredef.AddEdge(Edge.ToEdgePredefinition());
            }

            foreach (var Property in this.StructuredProperties)
            {
                EdgePredef.AddStructuredProperty(Property.PropertyName, Property.PropertyValue as IComparable);
            }

            foreach (var Property in this.UnstructuredProperties)
            {
                EdgePredef.AddUnstructuredProperty(Property.PropertyName, Property.PropertyValue);
            }

            foreach (var Vertex in this.VertexIDsByID)
            {
                EdgePredef.AddVertexID(Vertex.Key,Vertex.Value);
            }

            
            return EdgePredef;
        }
    }
}
