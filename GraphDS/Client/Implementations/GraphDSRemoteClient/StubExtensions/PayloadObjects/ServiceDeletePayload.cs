using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceDeletePayload
    {
        internal ServiceDeletePayload(RequestDelete myRequestDelete)
        {
            this.ToBeDeletedAttributes = myRequestDelete.ToBeDeletedAttributes.ToList();
        }
    }
}
