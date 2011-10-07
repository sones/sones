using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace sones.GraphDS.GraphDSRemoteClient
{
    internal interface IServiceToken
    {
        SecurityToken SecurityToken { get; }
        Int64 TransactionToken { get; }
        VertexTypeService VertexTypeService { get; }
        VertexInstanceService VertexService { get; }
        EdgeTypeService EdgeTypeService { get; }
        EdgeInstanceService EdgeService { get; }
        GraphDSService GraphDSService { get; }
        StreamedService StreamedService { get; }
    }
}
