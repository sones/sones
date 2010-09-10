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

/* <id name="PandoraDB – TypesOfOperatorResult Enum" />
 * <copyright file="TypesOfOperatorResult.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.QueryLanguage.Enums
{
    public enum TypesOfOperatorResult : short
    {
        Unknown,
        Int64,
        Int32,
        UInt64,
        Double,
        DateTime,
        Boolean,
        String,

        /// <summary>
        /// This might be a Irony NonTerminal as well
        /// </summary>
        NotABasicType,

        /// <summary>
        /// An GraphDB edge containing some objects. 
        /// </summary>
        SetOfDBObjects,

        Reference,
        BackwardEdge,
        Expression,

    }
}
