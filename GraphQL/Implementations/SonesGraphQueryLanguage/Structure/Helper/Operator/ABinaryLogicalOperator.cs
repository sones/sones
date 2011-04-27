using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public abstract class ABinaryLogicalOperator : ABinaryBaseOperator
    {

        public override AOperationDefinition SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression typeOfBinExpr)
        {
            throw new NotImplementedException();
        }

        public override IExpressionGraph TypeOperation(AExpressionDefinition myLeftValueObject, AExpressionDefinition myRightValueObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, bool aggregateAllowed = true)
        {
            throw new NotImplementedException();
        }

        public abstract IExpressionGraph TypeOperation(IExpressionGraph myLeftValueObject, IExpressionGraph myRightValueObject, IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, bool aggregateAllowed = true);

    }
}
