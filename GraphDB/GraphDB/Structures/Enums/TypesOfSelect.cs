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

/* <id name="GraphDB – TypesOfSelect Enum" />
 * <copyright file="TypesOfSelect.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.Structures.Enums
{
    /// <summary>
    /// describe the type of selection * or # or - or @
    /// </summary>
    [Flags]
    public enum TypesOfSelect
    {

        /// <summary>
        /// attribute selection
        /// </summary>
        None        = 0,

        /// <summary>
        /// select all attributes and undefined attributes
        /// </summary>
        Asterisk    = 1,

        /// <summary>
        /// select all user defined attributes and undefined attributes but no edges       
        /// </summary>
        Rhomb       = 2,

        /// <summary>
        /// select only edges
        /// </summary>
        Minus       = 3,

        /// <summary>
        /// select all attributes by type
        /// </summary>
        Ad          = 4,

        /// <summary>
        /// select only edges without backwardedges
        /// </summary>
        Lt          = 5,

        /// <summary>
        /// select only backwardedges
        /// </summary>
        Gt          = 6 
    }
}
