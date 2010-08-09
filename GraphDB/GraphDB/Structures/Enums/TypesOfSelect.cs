/* <id name="PandoraDB – TypesOfSelect Enum" />
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
    /// describe the type of selection * or # or -
    /// </summary>
    public enum TypesOfSelect
    {
        None,
        Asterisk,
        Rhomb,
        Minus
    }
}
