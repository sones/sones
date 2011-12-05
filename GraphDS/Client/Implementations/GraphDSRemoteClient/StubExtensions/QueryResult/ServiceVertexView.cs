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
            if (Properties != null)
            {
                foreach (var item in Properties)
                {
                    object value = ConvertHelper.ToDsObject(item.Value, myServiceToken);
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

            if (SingleEdges != null)
            {
                foreach (var item in this.SingleEdges)
                {
                    egdes.Add(item.Key, item.Value.ToSingleEdgeView(myServiceToken));
                }
            }

            if (HyperEdges != null)
            {
                foreach (var item in HyperEdges)
                {
                    egdes.Add(item.Key, item.Value.ToHyperEdgeView(myServiceToken));
                }
            }
            
            return new sones.GraphQL.Result.VertexView(properties, egdes);
        }
    }
}
