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
        public static IExpressionGraph TypeOperation(IExpressionGraph myLeftValueObject, IExpressionGraph myRightValueObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, BinaryOperator myOperator, bool aggregateAllowed = true)
        {
            return null;
        }
    }
}
