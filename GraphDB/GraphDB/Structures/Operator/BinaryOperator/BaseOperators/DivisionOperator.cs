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

/* <id name="GraphDB - division operator" />
 * <copyright file="DivisionOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a division operator.</summary>
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
    /// This class implements a division operator.
    /// </summary>
    public class DivisionOperator : ABinaryBaseOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "/" }; } }
        public override String              ContraryOperationSymbol { get { return "*"; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.Division; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public DivisionOperator()
        {

        }

        #endregion

        #region (public) Methods

        public override Exceptional<AOperationDefinition> SimpleOperation(AOperationDefinition left, AOperationDefinition right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (!(left is ValueDefinition && right is ValueDefinition))
            {
                return new Exceptional<AOperationDefinition>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            return SimpleOperation((ValueDefinition)left, (ValueDefinition)right);
        }

        public Exceptional<AOperationDefinition> SimpleOperation(ValueDefinition left, ValueDefinition right)
        {
            #region Data

            ValueDefinition resultObject = null;

            ADBBaseObject leftObject = left.Value;
            ADBBaseObject rightObject = right.Value;

            #endregion

            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObject, ref rightObject).Value)
            {
                return new Exceptional<AOperationDefinition>(new Error_DataTypeDoesNotMatch(leftObject.ObjectName, rightObject.ObjectName));
            }

            ADBBaseObject resultValue = leftObject.Div(leftObject, rightObject);

            resultObject = new ValueDefinition(resultValue.Type, resultValue.Value);

            return new Exceptional<AOperationDefinition>(resultObject);
        }

        #endregion

        public override Exceptional<IExpressionGraph> TypeOperation(AExpressionDefinition myLeftValueObject, AExpressionDefinition myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, Boolean aggregateAllowed = true)
        {
            return new Exceptional<IExpressionGraph>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

    }
}
