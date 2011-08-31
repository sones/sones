using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceTransactionToken
    {
        public ServiceTransactionToken(Int64 myTransaktionToken)
        {
            TransactionID = myTransaktionToken;
        }

        [DataMember]
        public Int64 TransactionID;
    }
}
