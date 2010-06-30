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

/* <id name="sones GraphDB - abstract binary operator types" />
 * <copyright file="BinaryOperatorType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary>This class holds all possible types of operators</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.GraphDB.QueryLanguage.Operators
{

    /// <summary>
    /// An enum type to list all possible types of operators
    /// </summary>
    public enum BinaryOperator
    {
        Equal,
        NotEqual,
        Inequal,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Or,
        And,
        In,
        NotIn,
        LessThan,
        LessEquals,
        GreaterThan,
        GreaterEquals,
        InRange
    }

}
