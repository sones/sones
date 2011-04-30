using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public abstract class AExpressionDefinition
    {
        protected BinaryOperator GetOperatorBySymbol(string myOperatorSymbol)
        {
            switch (myOperatorSymbol.ToUpper())
            {
                case "=":
                    return BinaryOperator.Equals;

                case ">":
                    return BinaryOperator.GreaterThan;

                case "<":
                    return BinaryOperator.LessThan;

                case ">=":
                    return BinaryOperator.GreaterOrEqualsThan;

                case "<=":
                    return BinaryOperator.LessOrEqualsThan;

                case "!=":
                    return BinaryOperator.NotEquals;

                case "AND":
                    return BinaryOperator.AND;

                case "OR":
                    return BinaryOperator.OR;

                case "INRANGE":
                    return BinaryOperator.InRange;

                default:

                    throw new NotImplementedQLException(String.Format("The operator {0} is not yet implemented.", myOperatorSymbol));
            }
        }
    }
}
