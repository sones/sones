/* <id name="GraphDB – TypesOfAtrributeValues Enum" />
 * <copyright file="TypesOfAtrributeValues.cs"
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

    [Obsolete("Use BasicType")]
    public enum TypesOfAtrributeValues
    {
        Unknown,
        NumberLiteral,
        StringLiteral,
        ListOfDBObjects,
        NonTerminal,
    }
}
