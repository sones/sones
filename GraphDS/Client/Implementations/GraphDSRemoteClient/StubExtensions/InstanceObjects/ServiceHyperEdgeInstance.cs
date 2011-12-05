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
            var edges = myEdge.GetAllEdges();
            SingleEdges = (edges == null || edges.Count() == 0) ? null : edges.Select(x => new ServiceSingleEdgeInstance(x)).ToList();
        }
    }
}
