using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceOutgoingEdgePredefinition
    {
        internal ServiceOutgoingEdgePredefinition(OutgoingEdgePredefinition myOutgoingEdgePredefinition) : base(myOutgoingEdgePredefinition)
        {
            this.EdgeType = myOutgoingEdgePredefinition.EdgeType;
            this.InnerEdgeType = myOutgoingEdgePredefinition.InnerEdgeType;
            switch(myOutgoingEdgePredefinition.Multiplicity)
            {
                case EdgeMultiplicity.SingleEdge:
                    this.Multiplicity = ServiceEdgeMultiplicity.SingleEdge;
                    break;
                case EdgeMultiplicity.MultiEdge:
                    this.Multiplicity = ServiceEdgeMultiplicity.MultiEdge;
                    break;
                case EdgeMultiplicity.HyperEdge:
                    this.Multiplicity = ServiceEdgeMultiplicity.HyperEdge;
                    break;
            }
        }
    }
}
