/*
 * IVersionedRangeIndexInterface
 * (c) Martin Junghanns, 2009 - 2010
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.Lib.DataStructures.Indices
{
    interface IVersionedRangeIndexInterface<TKey, TValue>
    {
        IEnumerable<TValue> GreaterThen(TKey myKey, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> GreaterThen(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> LowerThen(TKey myKey, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> LowerThen(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Int64 myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);
    }
}
