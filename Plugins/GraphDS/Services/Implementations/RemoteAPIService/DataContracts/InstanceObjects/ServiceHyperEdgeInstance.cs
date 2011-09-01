using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceHyperEdgeInstance : ServiceEdgeInstance
    {
        /// <summary>
        /// Gets all contained edges
        /// </summary>
        /// <param name="myFilter">A function to filter those edges</param>
        /// <returns>An IEnumerable of edges</returns>
        [DataMember]
        public IEnumerable<ServiceSingleEdgeInstance> GetAllEdges();
    }
}
