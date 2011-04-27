using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using sones.GraphDB.Expression;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class UnaryExpressionDefinition : AExpressionDefinition
    {
        private AExpressionDefinition _Expression;
        private String _OperatorSymbol;

        public UnaryExpressionDefinition(string myOperatorSymbol, AExpressionDefinition myExpression)
        {
            _OperatorSymbol = myOperatorSymbol;
            _Expression = myExpression;
        }

        public BinaryExpressionDefinition GetBinaryExpression(GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {

            AExpressionDefinition right;
            var op = GetOperatorBySymbol(_OperatorSymbol);
            if (op == null)
            {
                throw new OperatorDoesNotExistException(_OperatorSymbol);
            }

            right = new ValueDefinition(1);

            var binExpr = new BinaryExpressionDefinition("*", _Expression, right);
            binExpr.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);

            return binExpr;
        }

    }
}
