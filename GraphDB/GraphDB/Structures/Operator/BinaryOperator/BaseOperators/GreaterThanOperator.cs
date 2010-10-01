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

/* <id name="GraphDB - greater than operator" />
 * <copyright file="GreaterThanOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This class implements a greater than operator.</summary>
 */

#region Usings

using System;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Structures.Operators
{
    /// <summary>
    /// This class implements a greater than operator.
    /// </summary>
    public class GreaterThanOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { ">" }; } }
        public override String              ContraryOperationSymbol { get { return "<="; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.GreaterThan; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public GreaterThanOperator()
        {

        }

        #endregion

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<bool>(myLeft.CompareTo(myRight) > 0);
        }

    }
}
