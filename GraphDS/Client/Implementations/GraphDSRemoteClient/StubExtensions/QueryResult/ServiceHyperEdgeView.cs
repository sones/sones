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
            if (Properties != null)
            {
                foreach (var item in Properties)
                {
                    var value = ConvertHelper.ToDsObject(item.Value, myServiceToken);
                    if (value != null)
                    {
                        properties.Add(item.Key, value);
                    }
                    else
                    {
                        properties.Add(item.Key, item.Value);
                    }
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
