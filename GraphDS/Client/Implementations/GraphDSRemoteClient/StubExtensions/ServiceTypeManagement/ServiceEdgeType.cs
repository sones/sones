using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceEdgeType
    {
        internal ServiceEdgeType(String myName) : base(myName)
        {

        }

        internal ServiceEdgeType(IEdgeType myEdgeType) : base(myEdgeType.Name)
        {

        }
    }
}
