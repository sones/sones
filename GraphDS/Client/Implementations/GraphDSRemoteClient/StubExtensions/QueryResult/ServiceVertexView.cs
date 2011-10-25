using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceVertexView
    {
        internal IVertexView ToVertexView(IServiceToken myServiceToken)
        {
            Dictionary<String, Object> properties = new Dictionary<string,object>();
            Dictionary<String, IEdgeView> egdes = new Dictionary<string,IEdgeView>();
            foreach (var item in this.Properties)
            {
                object value = ConvertHelper.ToDsObject(item.Item2, myServiceToken);
                if (value != null)
                    properties.Add(item.Item1, value);
                else
                    properties.Add(item.Item1, item.Item2);
            }

            foreach(var item in this.SingleEdges)
                egdes.Add(item.Item1, item.Item2.ToSingleEdgeView(myServiceToken));
            foreach(var item in this.HyperEdges)
                egdes.Add(item.Item1, item.Item2.ToHyperEdgeView(myServiceToken));
            return new sones.GraphQL.Result.VertexView(properties, egdes);
        }
    }
}
