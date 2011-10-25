using System;
using System.Runtime.Serialization;
//+
namespace Jampad.Dojo.Rpc
{
    [DataContract]
    public class DojoMessage
    {
        //- @Params -//
        [DataMember(Name = "params")]
        public String[] Params;

        //- @Method -//
        [DataMember(Name = "method")]
        public String Method;

        //- @Id -//
        [DataMember(Name = "id")]
        public Int32 Id = 0;
    }
}