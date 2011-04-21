using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Expression.Tree.Literals
{
    /// <summary>
    /// A wrapper interface that wraps datastructures that are do not implement IComparable by default
    /// </summary>
    public interface IComparableWrapper : IComparable
    {
    }
}
