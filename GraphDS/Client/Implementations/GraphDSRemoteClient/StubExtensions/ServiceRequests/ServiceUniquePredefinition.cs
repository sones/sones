using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceUniquePredefinition
    {
        internal ServiceUniquePredefinition(UniquePredefinition myUniquePredefinition)
        {
            this.Properties = myUniquePredefinition.Properties.ToList();
        }
    }
}
