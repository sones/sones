using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.Structure.Helper.Enums
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
