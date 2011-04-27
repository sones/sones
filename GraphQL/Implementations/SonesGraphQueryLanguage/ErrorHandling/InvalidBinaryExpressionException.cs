using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class InvalidBinaryExpressionException : AGraphQLException
    {
        public InvalidBinaryExpressionException(BinaryExpressionDefinition myBinaryExpression)
        {
            _msg = String.Format("The BinaryExpression is not valid: {0} {1} {2}", myBinaryExpression.Left.ToString(), myBinaryExpression.Operator.Symbol.First(), myBinaryExpression.Right.ToString());
        }
    }
}
