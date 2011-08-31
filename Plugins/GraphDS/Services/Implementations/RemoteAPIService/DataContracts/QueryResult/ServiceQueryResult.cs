using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphQL.Result;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.QueryResult
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceQueryResult
    {
        public ServiceQueryResult(sones.GraphQL.Result.QueryResult myQueryResult)
        {
            this.Duration = myQueryResult.Duration;
            if (myQueryResult.Error != null)
                this.Error = myQueryResult.Error.Message;

            this.NameOfQueryLanguage = myQueryResult.NameOfQuerylanguage;
            this.NumberOfAffectedVertices = myQueryResult.NumberOfAffectedVertices;
            this.Query = myQueryResult.Query;
            this.TypeOfResult = myQueryResult.TypeOfResult.ToString();
            //todo add List<IVertexView> ->
        }

        [DataMember]
        public UInt64 Duration;

        [DataMember]
        public UInt64 NumberOfAffectedVertices;

        [DataMember]
        public String Error;

        [DataMember]
        public String TypeOfResult;

        [DataMember]
        public String Query;

        [DataMember]
        public String NameOfQueryLanguage;
    }
}
