using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.Structure.Helper.Enums
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
