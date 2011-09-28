using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceMandatoryPredefinition
    {
        internal ServiceMandatoryPredefinition(MandatoryPredefinition myMandatoryPredefinition)
        {
            this.PropertyName = myMandatoryPredefinition.MandatoryAttribute;
            this.DefaultValue = myMandatoryPredefinition.DefaultValue;
        }
    }
}
