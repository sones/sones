using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.Structure.Helper.Enums
{
    /// <summary>
    /// This enum describes the impact of operators on ExpressionGraph integration
    /// </summary>
    public enum TypesOfOperators
    {
        AffectsLocalLevelOnly, //affects the local level key only
        AffectsLowerLevels, //affects lower levels too
    }
}
