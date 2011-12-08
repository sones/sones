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
                    object value = ConvertHelper.ToDsObject(item.Value, myServiceToken);
                    if (value == null)
                    {
                        value = item.Value;
                    }
                    properties.Add(item.Key, value);
                }
            }

            if (ListProperties != null)
            {
                foreach (var item in ListProperties)
                {
                    properties.Add(item.Key, item.Value.Select(x => ConvertHelper.ToDsObject(x, myServiceToken) ?? x));
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
