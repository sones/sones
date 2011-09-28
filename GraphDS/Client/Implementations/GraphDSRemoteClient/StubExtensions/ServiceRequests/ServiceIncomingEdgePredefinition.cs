using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceIncomingEdgePredefinition
    {
        internal ServiceIncomingEdgePredefinition(IncomingEdgePredefinition myIncomingEdgePredefinition) : base(myIncomingEdgePredefinition)
        {
            this.OutgoingEdgeName = myIncomingEdgePredefinition.AttributeType.Substring(myIncomingEdgePredefinition.AttributeType.IndexOf(IncomingEdgePredefinition.TypeSeparator) + 1);
        }
    }
}
