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
            if (PropertyList != null)
            {
                foreach (var item in PropertyList)
                {
                    properties.Add(item.Item1, item.Item2);
                }
            }
            
            return new SingleEdgeView(properties, TargetVertex.ToVertexView(myServiceToken));
        }
    }
}
