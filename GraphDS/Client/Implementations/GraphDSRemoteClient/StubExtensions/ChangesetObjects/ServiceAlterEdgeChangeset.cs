using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceAlterEdgeChangeset
    {
        internal ServiceAlterEdgeChangeset(RequestAlterEdgeType myRequestAlterEdgeType)
        {
            this.NewTypeName = myRequestAlterEdgeType.AlteredTypeName;
            this.Comment = myRequestAlterEdgeType.AlteredComment;
            this.ToBeAddedProperties = myRequestAlterEdgeType.ToBeAddedProperties.Select(x => new ServicePropertyPredefinition(x)).ToList();
            this.ToBeRemovedProperties = myRequestAlterEdgeType.ToBeRemovedProperties.ToList();
            this.ToBeRenamedProperties = myRequestAlterEdgeType.ToBeRenamedProperties;
        }
    }
}
