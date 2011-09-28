using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceVertexTypePredefinition
    {
        internal ServiceVertexTypePredefinition(VertexTypePredefinition myVertexTypePredefinition)
        {
            this.VertexTypeName = myVertexTypePredefinition.TypeName;
            this.Uniques = myVertexTypePredefinition.Uniques.Select(x => new ServiceUniquePredefinition(x)).ToList();
            this.Indices = myVertexTypePredefinition.Indices.Select(x => new ServiceIndexPredefinition(x)).ToList();
            this.SuperVertexTypeName = myVertexTypePredefinition.SuperTypeName;
            this.Properties = myVertexTypePredefinition.Properties.Select(x => new ServicePropertyPredefinition(x)).ToList();
            this.OutgoingEdges = myVertexTypePredefinition.OutgoingEdges.Select(x => new ServiceOutgoingEdgePredefinition(x)).ToList();
            this.IncomingEdges = myVertexTypePredefinition.IncomingEdges.Select(x => new ServiceIncomingEdgePredefinition(x)).ToList();
            this.IsSealed = myVertexTypePredefinition.IsSealed;
            this.IsAbstract = myVertexTypePredefinition.IsAbstract;
            this.Comment = myVertexTypePredefinition.Comment;
        }
    }
}
