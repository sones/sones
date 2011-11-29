using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceUnknownAttributePredefinition
    {
        internal ServiceUnknownAttributePredefinition(UnknownAttributePredefinition myPredef)
            : base(myPredef)
        {
            DefaultValue = myPredef.DefaultValue;
            EdgeType = myPredef.EdgeType;
            InnerEdgeType = myPredef.InnerEdgeType;
            IsMandatory = myPredef.IsMandatory;
            IsUnique = myPredef.IsUnique;
            Multiplicity = myPredef.Multiplicity;
        }
    }
}
