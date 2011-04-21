using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Expression.Tree.Literals
{
    public interface ILiteralExpression
    {
        IComparable Value { get; }
    }
}
