/* <id name="GraphDB – TypesOfStatements Enum" />
 * <copyright file="TypesOfStatements.cs"
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
    public enum TypesOfStatements
    {
        Readonly,
        ReadWrite,
        Setting
    }
}
