using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceVertexView
    {
        internal IVertexView ToVertexView()
        {
            Dictionary<String, Object> properties = new Dictionary<string,object>();
            Dictionary<String, IEdgeView> egdes = new Dictionary<string,IEdgeView>();
            foreach(var item in this.Properties)
                properties.Add(item.Item1, item.Item2);
            foreach(var item in this.SingleEdges)
                egdes.Add(item.Item1, item.Item2.ToSingleEdgeView());
            foreach(var item in this.HyperEdges)
                egdes.Add(item.Item1, item.Item2.ToHyperEdgeView());
            return new sones.GraphQL.Result.VertexView(properties, egdes);
        }
    }
}
