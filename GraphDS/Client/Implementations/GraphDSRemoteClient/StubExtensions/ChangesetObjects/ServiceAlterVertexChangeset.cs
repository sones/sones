using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceAlterVertexChangeset
    {
        internal ServiceAlterVertexChangeset(RequestAlterVertexType myRequestAlterVertexType)
        {
            this.NewTypeName = myRequestAlterVertexType.AlteredTypeName;
            this.Comment = myRequestAlterVertexType.AlteredComment;
            this.ToBeAddedProperties = myRequestAlterVertexType.ToBeAddedProperties.Select(x => new ServicePropertyPredefinition(x)).ToList();
            this.ToBeAddedIncomingEdges = myRequestAlterVertexType.ToBeAddedIncomingEdges.Select(x => new ServiceIncomingEdgePredefinition(x)).ToList();
            this.ToBeAddedOutgoingEdges = myRequestAlterVertexType.ToBeAddedOutgoingEdges.Select(x => new ServiceOutgoingEdgePredefinition(x)).ToList();
            this.ToBeAddedIndices = myRequestAlterVertexType.ToBeAddedIndices.Select(x => new ServiceIndexPredefinition(x)).ToList();
            this.ToBeAddedUniques = myRequestAlterVertexType.ToBeAddedUniques.Select(x => new ServiceUniquePredefinition(x)).ToList();
            this.ToBeAddedMandatories = myRequestAlterVertexType.ToBeAddedMandatories.Select(x => new ServiceMandatoryPredefinition(x)).ToList();
            this.ToBeRemovedProperties = myRequestAlterVertexType.ToBeRemovedProperties.ToList();
            this.ToBeRemovedIncomingEdges = myRequestAlterVertexType.ToBeRemovedIncomingEdges.ToList();
            this.ToBeRemovedOutgoingEdges = myRequestAlterVertexType.ToBeRemovedOutgoingEdges.ToList();
            this.ToBeRemovedIndices = myRequestAlterVertexType.ToBeRemovedIndices;
            this.ToBeRemovedUniques = myRequestAlterVertexType.ToBeRemovedUniques.ToList();
            this.ToBeRemovedMandatories = myRequestAlterVertexType.ToBeRemovedMandatories.ToList();
            this.ToBeRenamedProperties = myRequestAlterVertexType.ToBeRenamedProperties;
        }
    }
}
