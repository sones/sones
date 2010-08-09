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

/* <id name="sones GraphDB - or operator" />
 * <copyright file="OrOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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
using sones.Lib.Session;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.Structures.Operators
{
    /// <summary>
    /// This class implements a or operator.
    /// </summary>
    class OrOperator : ABinaryLogicalOperator
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
