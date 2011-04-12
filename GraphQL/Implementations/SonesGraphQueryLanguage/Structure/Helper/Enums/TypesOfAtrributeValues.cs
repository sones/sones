using System;

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
