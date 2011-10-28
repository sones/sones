using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using System.IO;
using System.ServiceModel;

namespace sones.GraphDS.Services.RemoteAPIService.MessageContracts
{
    [MessageContract]
    public class SetBinaryPropertyMessage
    {
        [MessageHeader(MustUnderstand=true)]
        public SecurityToken SecurityToken;

        [MessageHeader(MustUnderstand = true)]
        public Int64 TransToken;

        [MessageHeader(MustUnderstand = true)]
        public Int64 VertexTypeID;

        [MessageHeader(MustUnderstand = true)]
        public Int64 VertexID;

        [MessageHeader(MustUnderstand = true)]
        public String PropertyName;

        [MessageBodyMember]
        public Stream Stream;
    }
}
