using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceOutgoingEdgePredefinition
    {
        internal ServiceOutgoingEdgePredefinition(OutgoingEdgePredefinition myOutgoingEdgePredefinition) : base(myOutgoingEdgePredefinition)
        {
            this.EdgeType = myOutgoingEdgePredefinition.EdgeType;
            this.InnerEdgeType = myOutgoingEdgePredefinition.InnerEdgeType;
            this.Multiplicity = ConvertHelper.ToServiceEdgeMultiplicity(myOutgoingEdgePredefinition.Multiplicity);
        }
    }
}
