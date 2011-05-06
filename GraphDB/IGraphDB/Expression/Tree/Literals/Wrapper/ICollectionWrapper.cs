using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Expression.Tree.Literals
{
    /// <summary>
    /// A wrapper interface that wraps collections
    /// This data structure is needed, because generic collections do not implement IComparable
    /// </summary>
    public interface ICollectionWrapper : IComparableWrapper, IEnumerable<IComparable>
    {
        void Add(IComparable myComparable);

        void Add(IEnumerable<IComparable> myComparables);

        void Remove(IComparable myComparable);

        void Remove(IEnumerable<IComparable> myComparables);


    }
}
