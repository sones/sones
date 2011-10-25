using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//+
namespace Jampad.Dojo.Rpc.Description
{
    [DataContract]
    public class DojoServiceDescriptionMethod
    {
        //- @Name -//
        [DataMember(Name = "name")]
        public String Name;

        //- @Parameters -//
        [DataMember(Name = "parameters")]
        public List<DojoServiceDescriptionMethodParameter> ParameterList;

        //+
        //- @Ctor -//
        public DojoServiceDescriptionMethod(String name)
        {
            this.Name = name;
            this.ParameterList = new List<DojoServiceDescriptionMethodParameter>();
        }
    }
}