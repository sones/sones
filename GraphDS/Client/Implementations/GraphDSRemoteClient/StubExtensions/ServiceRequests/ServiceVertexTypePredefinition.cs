using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceVertexTypePredefinition
    {
        internal ServiceVertexTypePredefinition(VertexTypePredefinition myVertexTypePredefinition)
        {
            this.VertexTypeName = myVertexTypePredefinition.TypeName;
            this.SuperVertexTypeName = myVertexTypePredefinition.SuperTypeName;
            this.IsSealed = myVertexTypePredefinition.IsSealed;
            this.IsAbstract = myVertexTypePredefinition.IsAbstract;
            this.Comment = myVertexTypePredefinition.Comment;

            this.Uniques = (myVertexTypePredefinition.Uniques == null)
                ? null : myVertexTypePredefinition.Uniques.Select(x => new ServiceUniquePredefinition(x)).ToList();

            this.Indices = (myVertexTypePredefinition.Indices == null)
                ? null : myVertexTypePredefinition.Indices.Select(x => new ServiceIndexPredefinition(x)).ToList();
            
            this.Properties = (myVertexTypePredefinition.Properties == null)
                ? null : myVertexTypePredefinition.Properties.Select(x => new ServicePropertyPredefinition(x)).ToList();

            this.BinaryProperties = (myVertexTypePredefinition.BinaryProperties == null)
                ? null : myVertexTypePredefinition.BinaryProperties.Select(x => new ServiceBinaryPropertyPredefinition(x)).ToList();

            this.OutgoingEdges = (myVertexTypePredefinition.OutgoingEdges == null)
                ? null : myVertexTypePredefinition.OutgoingEdges.Select(x => new ServiceOutgoingEdgePredefinition(x)).ToList();

            this.IncomingEdges = (myVertexTypePredefinition.IncomingEdges == null)
                ? null : myVertexTypePredefinition.IncomingEdges.Select(x => new ServiceIncomingEdgePredefinition(x)).ToList();
        }
    }
}
