/* <id name="GraphDB - or operator" />
 * <copyright file="OrOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a or operator.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.Structures.Enums;





using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.Errors;
using sones.GraphFS.Session;
using sones.GraphFS.Session;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.Structures.Operators
{
    /// <summary>
    /// This class implements a or operator.
    /// </summary>
    public class OrOperator : ABinaryLogicalOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "OR", "|" }; } }
        public override String              ContraryOperationSymbol { get { return "OR"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.Or; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public OrOperator()
        {

        }

        #endregion

        #region (public) Methods

        public override Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            return new Exceptional<AOperationDefinition>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        #endregion

        public override Exceptional<IExpressionGraph> TypeOperation(IExpressionGraph myLeftValueObject, IExpressionGraph myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, bool aggregateAllowed = true)
        {
            myLeftValueObject.UnionWith(myRightValueObject);

            return new Exceptional<IExpressionGraph>(myLeftValueObject);        
        }
    }
}
