/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB - division operator" />
 * <copyright file="DivisionOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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
    /// This class implements a division operator.
    /// </summary>
    class DivisionOperator : ABinaryBaseOperator
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

        public override Exceptional<IOperationValue> SimpleOperation(IOperationValue left, IOperationValue right, TypesOfBinaryExpression myTypeOfBinaryExpression)
        {
            if (!(left is AtomValue && right is AtomValue))
            {
                return new Exceptional<IOperationValue>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }

            return SimpleOperation((AtomValue)left, (AtomValue)right);
        }

        public Exceptional<IOperationValue> SimpleOperation(AtomValue left, AtomValue right)
        {
            #region Data

            AtomValue resultObject = null;

            ADBBaseObject leftObject = left.Value;
            ADBBaseObject rightObject = right.Value;

            #endregion

            if (!GraphDBTypeMapper.ConvertToBestMatchingType(ref leftObject, ref rightObject).Value)
            {
                return new Exceptional<IOperationValue>(new Error_DataTypeDoesNotMatch(leftObject.ObjectName, rightObject.ObjectName));
            }

            ADBBaseObject resultValue = leftObject.Div(leftObject, rightObject);

            resultObject = new AtomValue(resultValue.Type, resultValue.Value);

            return new Exceptional<IOperationValue>(resultObject);
        }

        #endregion

        public override Exceptional<IExpressionGraph> TypeOperation(object myLeftValueObject, object myRightValueObject, DBContext dbContext, TypesOfBinaryExpression typeOfBinExpr, TypesOfAssociativity associativity, IExpressionGraph result, Boolean aggregateAllowed = true)
        {
            return new Exceptional<IExpressionGraph>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

    }
}
