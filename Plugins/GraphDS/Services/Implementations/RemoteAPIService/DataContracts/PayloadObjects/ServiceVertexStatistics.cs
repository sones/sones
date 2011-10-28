using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.PayloadObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceVertexStatistics
    {
        public ServiceVertexStatistics(IVertexStatistics myStatictic)
        {
            InDegree = myStatictic.InDegree;
            OutDegree = myStatictic.OutDegree;
            Visits = myStatictic.Visits;
        }

        [DataMember]
        public ulong InDegree;

        [DataMember]
        public ulong OutDegree;

        [DataMember]
        public long Visits;
    }
}
