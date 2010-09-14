/*
 * IVersionedIndexObject
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The additional interface for all versioned IndexObjects
    /// </summary>
    public interface IVersionedIndexObject<TKey, TValue> : IIndexObject<TKey, TValue>, IVersionedIndexInterface<TKey, TValue>
        where TKey : IComparable
    {

        IVersionedIndexObject<TKey, TValue> GetNewInstance2();

    }

}
