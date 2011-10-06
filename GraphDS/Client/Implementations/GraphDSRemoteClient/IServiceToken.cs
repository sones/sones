using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient
{
    internal interface IServiceToken
    {
        SecurityToken SecurityToken { get; }
        Int64 TransactionToken { get; }
        VertexTypeService VertexTypeService { get; }
        VertexInstanceService VertexService { get; }
        EdgeTypeService EdgeTypeService { get; }
        EdgeInstanceService EdgeService { get; }
        GraphDS GraphDSService { get; }
        StreamedService StreamedService { get; }
    }
}
