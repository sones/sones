using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceSingleEdgeView
    {
        internal ISingleEdgeView ToSingleEdgeView(IServiceToken myServiceToken)
        {
            Dictionary<String, Object> properties = new Dictionary<string,object>();
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
            
            return new SingleEdgeView(properties, TargetVertex.ToVertexView(myServiceToken));
        }
    }
}
