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

/* <id name="sones GraphDB - not equals operator" />
 * <copyright file="NotEqualsOperator.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class implements a not equals operator.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Exceptions;

using sones.GraphDB.QueryLanguage.Enums;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.PandoraTypes;

#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{
    /// <summary>
    /// This class implements a not equals operator.
    /// </summary>
    class NotEqualsOperator : ABinaryCompareOperator
    {
        #region General comparer infos

        public override String[]            Symbol                  { get { return new String[] { "!=", "<>" }; } }
        public override String              ContraryOperationSymbol { get { return "="; } }
        public override BinaryOperator      Label                   { get { return BinaryOperator.NotEqual; } }
        public override TypesOfOperators    Type                    { get { return TypesOfOperators.AffectsLowerLevels; } }

        #endregion

        #region Constructor

        public NotEqualsOperator()
        {

        }

        #endregion

        protected override Exceptional<Boolean> Compare(ADBBaseObject myLeft, ADBBaseObject myRight)
        {
            return new Exceptional<Boolean>(myLeft.CompareTo(myRight) != 0);
        }

    }
}
