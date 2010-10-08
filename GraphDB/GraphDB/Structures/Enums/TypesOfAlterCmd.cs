/* <id name="GraphDB – TypesOfAlterCmd Enum" />
 * <copyright file="TypesOfAlterCmd.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDB.Structures.Enums
{
    public enum TypesOfAlterCmd
    {
        Add,
        Drop,
        RenameAttribute,
        RenameType,
        RenameBackwardedge,
        Unqiue,
        DropUnqiue,
        Mandatory,
        DropMandatory,
        ChangeComment
    }
}
