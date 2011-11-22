using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceHyperEdgeInstance
    {
        public ServiceHyperEdgeInstance(IHyperEdge myEdge) : base(myEdge)
        {
            SingleEdges = myEdge.GetAllEdges().Select(x => new ServiceSingleEdgeInstance(x)).ToList();
        }
    }
}
