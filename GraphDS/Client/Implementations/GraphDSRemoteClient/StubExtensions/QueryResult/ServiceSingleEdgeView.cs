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
            
            return new SingleEdgeView(properties, TargetVertex.ToVertexView(myServiceToken));
        }
    }
}
