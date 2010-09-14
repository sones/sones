/*
 * ABinaryLogicalOperator
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.ExpressionGraph;

#endregion

namespace sones.GraphDB.Structures.Operators
{
    public abstract class ABinaryLogicalOperator : ABinaryBaseOperator
    {

        public override Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression typeOfBinExpr)
        {
            throw new NotImplementedException();
        }

        public override Exceptional<IExpressionGraph> TypeOperation(AExpressionDefinition myLeftValueObject, AExpressionDefinition myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, ExpressionGraph.IExpressionGraph result, bool aggregateAllowed = true)
        {
            throw new NotImplementedException();
        }

        public abstract Exceptional<IExpressionGraph> TypeOperation(IExpressionGraph myLeftValueObject, IExpressionGraph myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, ExpressionGraph.IExpressionGraph result, bool aggregateAllowed = true);

    }
}
