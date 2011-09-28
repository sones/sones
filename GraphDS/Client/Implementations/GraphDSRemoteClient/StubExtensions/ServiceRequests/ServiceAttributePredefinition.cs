using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceAttributePredefinition
    {
        internal ServiceAttributePredefinition(AAttributePredefinition myAttributePredefinition)
        {
            this.AttributeName = myAttributePredefinition.AttributeName;
            this.AttributeType = myAttributePredefinition.AttributeType;
            this.Comment = myAttributePredefinition.Comment;
        }
    }
}
