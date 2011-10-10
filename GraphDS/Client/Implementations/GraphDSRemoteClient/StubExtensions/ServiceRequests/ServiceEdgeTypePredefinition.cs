using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceEdgeTypePredefinition
    {
        internal ServiceEdgeTypePredefinition(EdgeTypePredefinition myEdgeTypePredefinition)
        {
            this.EdgeTypeName = myEdgeTypePredefinition.TypeName;
            this.SuperEdgeTypeName = myEdgeTypePredefinition.SuperTypeName;
            this.IsSealed = myEdgeTypePredefinition.IsSealed;
            this.Comment = myEdgeTypePredefinition.Comment;

            this.Properties = (myEdgeTypePredefinition.Properties == null)
                ? null : myEdgeTypePredefinition.Properties.Select(x => new ServicePropertyPredefinition(x)).ToList();
        }
    }
}
