using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceVertexInstance
    {
        internal ServiceVertexInstance(IVertex myVertex)
        {
            this.Edition = myVertex.EditionName;
            this.TypeID = myVertex.VertexTypeID;
            this.VertexID = myVertex.VertexID;
        }
    }
}
