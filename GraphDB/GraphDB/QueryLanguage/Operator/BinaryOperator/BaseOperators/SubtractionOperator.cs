/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id name="PandoraDB - subtraction operator" />
 * <copyright file="SubtractionOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a subtraction operator.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.QueryLanguage.Enums;

using sones.GraphDB.QueryLanguage.Operator;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.Errors;
using sones.GraphFS.Session;
using sones.Lib.Session;

#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    /// <summary>
    /// This class implements a subtraction operator.
    /// </summary>
    class SubtractionOperator : ABinaryBaseOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "-" }; } }
        public override String              ContraryOperationSymbol { get { return "+"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.Subtraction; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public SubtractionOperator()
        {

        }

        #endregion

        #region (public) Methods

        public override Exceptional<IOperationValue> SimpleOperation(IOperationValue left, IOperationValue right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (left is AtomValue && right is AtomValue)
                return SimpleOperation((AtomValue)left, (AtomValue)right);
            /* 3 - [3,5,7] ???
            if (left is AtomValue && right is TupleValue)
                return SimpleOperation((AtomValue)left, (TupleValue)right);
            */
            if (left is TupleValue && right is AtomValue)
                return SimpleOperation((AtomValue)right, (TupleValue)left);
            if (left is TupleValue && right is TupleValue)
                return SimpleOperation((TupleValue)right, (TupleValue)left);

            return new Exceptional<IOperationValue>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public Exceptional<IOperationValue> SimpleOperation(TupleValue left, TupleValue right)
        {

            right.Remove(left);

            return new Exceptional<IOperationValue>(right);
        }

        public Exceptional<IOperationValue> SimpleOperation(AtomValue left, TupleValue right)
        {

            right.Remove(left.Value);

            return new Exceptional<IOperationValue>(right);
        }

        public Exceptional<IOperationValue> SimpleOperation(AtomValue left, AtomValue right)
        {

            ADBBaseObject leftObject = left.Value;
            ADBBaseObject rightObject = right.Value;

            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObject, ref rightObject).Value)
            {
                return new Exceptional<IOperationValue>(new Error_DataTypeDoesNotMatch(leftObject.Type.ToString(), rightObject.Type.ToString()));
            }

            ADBBaseObject resultValue = leftObject.Sub(leftObject, rightObject);

            var resultObject = new AtomValue(resultValue.Type, resultValue.Value);

            return new Exceptional<IOperationValue>(resultObject);
        }

        public override Exceptional<IExpressionGraph> TypeOperation(object myLeftValueObject, object myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, Boolean aggregateAllowed = true)
        {
            return new Exceptional<IExpressionGraph>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        #endregion
    }
}
