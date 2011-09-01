using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    [KnownType(typeof(ServiceEdgeInstance))]
    [KnownType(typeof(ServiceVertexInstance))]
    public class AGraphElement
    {
        public AGraphElement(Int64 myTypeID)
        {
            this.TypeID = myTypeID;
        }

        [DataMember]
        public Int64 TypeID; 
    }
}
