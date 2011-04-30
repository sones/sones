using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB.Expression;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public static class ABinaryLogicalOperator
    {
        public static IExpressionGraph TypeOperation(IExpressionGraph myLeftValueObject, IExpressionGraph myRightValueObject, BinaryOperator myOperator)
        {
            switch (myOperator)
            {
                case BinaryOperator.AND:
                    myLeftValueObject.IntersectWith(myRightValueObject);

                    break;
                case BinaryOperator.OR:
                    
                    myLeftValueObject.UnionWith(myRightValueObject);

                    break;
                case BinaryOperator.Equals:
                case BinaryOperator.GreaterOrEqualsThan:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.InRange:
                case BinaryOperator.LessOrEqualsThan:
                case BinaryOperator.LessThan:
                case BinaryOperator.NotEquals:
                default:
                    throw new ArgumentOutOfRangeException("myOperator");
            }

            return myLeftValueObject;
        }
    }
}
