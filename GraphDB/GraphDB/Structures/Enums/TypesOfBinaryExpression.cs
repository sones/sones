/* <id name="GraphDB – TypesOfBinaryExpression Enum" />
 * <copyright file="TypesOfBinaryExpression.cs"
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
    public enum TypesOfBinaryExpression
    {
        Atom,
        LeftComplex,
        RightComplex,
        Complex,
        Unknown,
    }
}
