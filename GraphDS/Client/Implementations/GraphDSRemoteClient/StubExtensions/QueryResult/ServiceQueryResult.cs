using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;
using sones.GraphDS.GraphDSRemoteClient.ErrorHandling;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceQueryResult
    {
        internal QueryResult ToQueryResult(IServiceToken myServiceToken)
        {
            ResultType type;
            if (this.TypeOfResult == ServiceResultType.Successful)
            {
                type = ResultType.Successful;
            }
            else
            {
                type = ResultType.Failed;
            }
            if (!String.IsNullOrEmpty(this.Error))
            {
                return new QueryResult(this.Query, this.NameOfQueryLanguage, this.Duration, type, this.Vertices.Select(x => x.ToVertexView(myServiceToken)), new RemoteException(this.Error));
            }
            else
            {
                return new QueryResult(this.Query, this.NameOfQueryLanguage, this.Duration, type, this.Vertices.Select(x => x.ToVertexView(myServiceToken)));
            }
        }
    }
}
