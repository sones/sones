/* <id name="GraphDB - abstract binary operator types" />
 * <copyright file="BinaryOperatorType.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary>This class holds all possible types of operators</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.GraphDB.Structures.Operators
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
