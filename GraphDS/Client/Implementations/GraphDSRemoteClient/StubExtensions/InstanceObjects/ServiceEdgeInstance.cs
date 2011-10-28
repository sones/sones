using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using sones.GraphDS.GraphDSRemoteClient.GraphElements;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceEdgeInstance
    {
        public ServiceEdgeInstance(IEdge myEdge)
        {
            this.TypeID = myEdge.EdgeTypeID;
            this.EdgePropertyID = ((ARemoteEdge)myEdge).EdgePropertyID;
            this.SourceVertex = new ServiceVertexInstance(myEdge.GetSourceVertex());
        }

        public ServiceEdgeInstance()
        {}
    }
}
