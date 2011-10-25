using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
//+
namespace Jampad.Dojo.Rpc.Description
{
    [DataContract]
    public class DojoServiceDescriptionMethodParameter
    {
        //- @Name -//
        [DataMember(Name = "name")]
        public String Name;

        //+
        //- @Ctor -//
        public DojoServiceDescriptionMethodParameter(String name)
        {
            this.Name = name;
        }
    }
}