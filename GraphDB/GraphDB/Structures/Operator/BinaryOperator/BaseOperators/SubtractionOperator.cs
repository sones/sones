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

/* <id name="GraphDB - subtraction operator" />
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

using sones.GraphDB.Structures.Enums;


using sones.GraphDB.TypeManagement.BasicTypes;
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
    /// This class implements a subtraction operator.
    /// </summary>
    public class SubtractionOperator : ABinaryBaseOperator
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

        public override Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (left is ValueDefinition && right is ValueDefinition)
                return SimpleOperation((ValueDefinition)left, (ValueDefinition)right);
            /* 3 - [3,5,7] ???
            if (left is AtomValue && right is TupleValue)
                return SimpleOperation((AtomValue)left, (TupleValue)right);
            */
            if (left is TupleDefinition && right is ValueDefinition)
                return SimpleOperation((ValueDefinition)right, (TupleDefinition)left);
            if (left is TupleDefinition && right is TupleDefinition)
                return SimpleOperation((TupleDefinition)right, (TupleDefinition)left);

            return new Exceptional<AOperationDefinition>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public Exceptional<AOperationDefinition> SimpleOperation(TupleDefinition left, TupleDefinition right)
        {

            right.Remove(left);

            return new Exceptional<AOperationDefinition>(right);
        }

        public Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, TupleDefinition right)
        {

            right.Remove(left);

            return new Exceptional<AOperationDefinition>(right);
        }

        public Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, ValueDefinition right)
        {

            ADBBaseObject leftObject = left.Value;
            ADBBaseObject rightObject = right.Value;


            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObject, ref rightObject).Value)
            {
                return new Exceptional<AOperationDefinition>(new Error_DataTypeDoesNotMatch(leftObject.Type.ToString(), rightObject.Type.ToString()));
            }

            if (leftObject.CompareTo(rightObject) < 0 && leftObject is DBUInt64 && DBInt64.IsValid(leftObject.Value))
            {
                leftObject = new DBInt64(leftObject.Value);

                if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObject, ref rightObject).Value)
                {
                    return new Exceptional<AOperationDefinition>(new Error_DataTypeDoesNotMatch(leftObject.Type.ToString(), rightObject.Type.ToString()));
                }
            }

            ADBBaseObject resultValue = leftObject.Sub(leftObject, rightObject);

            var resultObject = new ValueDefinition(resultValue.Type, resultValue.Value);

            return new Exceptional<AOperationDefinition>(resultObject);
        }

        public override Exceptional<IExpressionGraph> TypeOperation(AExpressionDefinition myLeftValueObject, AExpressionDefinition myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, Boolean aggregateAllowed = true)
        {
            return new Exceptional<IExpressionGraph>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        #endregion
    }
}
