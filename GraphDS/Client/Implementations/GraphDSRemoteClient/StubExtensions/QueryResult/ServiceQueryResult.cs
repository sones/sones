using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceQueryResult
    {
        internal QueryResult ToQueryResult()
        {
            
            ResultType type;
            if(this.TypeOfResult == ServiceResultType.Successful)
                type = ResultType.Successful;
            else
                type = ResultType.Failed;
            return new QueryResult(this.Query, this.NameOfQueryLanguage, this.Duration, type, this.Vertices.Select(x => x.ToVertexView()));
        }
    }
}
