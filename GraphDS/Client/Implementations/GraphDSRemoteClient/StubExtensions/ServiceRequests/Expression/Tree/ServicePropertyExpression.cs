using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServicePropertyExpression
    {
        internal ServicePropertyExpression(PropertyExpression myExpression)
        {
            this.Edition = myExpression.Edition;
            this.NameOfProperty = myExpression.NameOfProperty;
            this.NameOfVertexType = myExpression.NameOfVertexType;
        }
    }
}
