using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace sones.GraphDS.GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceUnaryExpression
    {
        internal ServiceUnaryExpression(UnaryExpression myExpression)
        {
            this.Expression = ConvertHelper.ToServiceExpression(myExpression.Expression);
            this.Operator = (ServiceUnaryLogicOperator)myExpression.Operator;
        }
    }
}
