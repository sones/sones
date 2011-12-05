using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceHyperEdgeView
    {
        internal IHyperEdgeView ToHyperEdgeView(IServiceToken myServiceToken)
        {
            Dictionary<String, Object> properties = new Dictionary<string, object>();
            List<ISingleEdgeView> edges = new List<ISingleEdgeView>();
            if (PropertyList != null)
            {
                foreach (var item in PropertyList)
                {
                    properties.Add(item.Item1, item.Item2);
                }
            }
            if (Edges != null)
            {
                foreach(var item in Edges)
                {
                    edges.Add(item.ToSingleEdgeView(myServiceToken));
                }
            }
            return new HyperEdgeView(properties, edges);
        }
    }
}
