using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceDeletePayload
    {
        internal ServiceDeletePayload(RequestDelete myRequestDelete)
        {
            this.ToBeDeletedAttributes = (myRequestDelete.ToBeDeletedAttributes == null)
                ? null : myRequestDelete.ToBeDeletedAttributes.ToList();
        }
    }
}
