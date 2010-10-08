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
