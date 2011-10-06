using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceVertexType
    {
        internal ServiceVertexType(String myName) : base(myName)
        {
        }

        internal ServiceVertexType(IVertexType myVertexType) : base(myVertexType.Name)
        {
        }
    }
}
