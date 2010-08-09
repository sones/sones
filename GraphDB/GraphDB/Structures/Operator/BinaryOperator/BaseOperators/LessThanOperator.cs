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

/* <id name="sones GraphDB - less than operator" />
 * <copyright file="LessThanOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a less than operator.</summary>
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
    /// This class implements a less than operator.
    /// </summary>
    class LessThanOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "<" }; } }
        public override String              ContraryOperationSymbol { get { return ">="; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.LessThan; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLocalLevelOnly; } }

        #endregion

        #region Constructor

        public LessThanOperator()
        {

        }

        #endregion

        protected override Exceptional<bool> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) < 0);
        }

    }
}
