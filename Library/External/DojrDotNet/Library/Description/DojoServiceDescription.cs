using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
//+
namespace Jampad.Dojo.Rpc.Description
{
    [DataContract]
    public class DojoServiceDescription
    {
        //- @ServiceType -//
        [DataMember(Name = "serviceType")]
        public String ServiceType { get; set; }

        //- @ServiceURL -//
        [DataMember(Name = "serviceURL")]
        public String ServiceURL { get; set; }

        //- @Methods -//
        [DataMember(Name = "methods")]
        public List<DojoServiceDescriptionMethod> MethodList { get; set; }

        //+
        //- @Ctor -//
        public DojoServiceDescription()
        {
            this.ServiceType = "JSON-RPC";
            this.MethodList = new List<DojoServiceDescriptionMethod>();
        }
    }
}