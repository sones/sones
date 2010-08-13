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
        None        = 0, // attribute selection
        Asterisk    = 1, // select all attributes and undefined attributes
        Rhomb       = 2, // select all user defined attributes and undefined attributes but no edges       
        Minus       = 3, // select only edges
        Ad          = 4, // select all attributes by type
        Lt          = 5, // select only edges without backwardedges
        Gt          = 6  // select only backwardedges
    }
}
