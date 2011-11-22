using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceSingleEdgeInstance
    {
        public ServiceSingleEdgeInstance(ISingleEdge myEdge) : base(myEdge)
        {
            TargetVertex = new ServiceVertexInstance(myEdge.GetTargetVertex());
        }
    }
}
