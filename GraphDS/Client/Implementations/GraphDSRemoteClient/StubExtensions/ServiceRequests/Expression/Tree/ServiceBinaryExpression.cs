using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;

namespace GraphDSRemoteClient.sonesGraphDSRemoteAPI
{
    public partial class ServiceBinaryExpression
    {
        internal ServiceBinaryExpression(BinaryExpression myExpression)
        {
            this.Left = ConvertHelper.ToServiceExpression(myExpression.Left);
            this.Right = ConvertHelper.ToServiceExpression(myExpression.Right);
            this.Operator = (ServiceBinaryOperator)myExpression.Operator;
        }
    }
}
