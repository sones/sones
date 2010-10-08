/*
 * IVersionedDictionaryObject
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Text;
using sones.Lib.DataStructures;
using System.Collections.Generic;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures.Dictionaries;
using sones.Lib;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface of a VersionedDictionaryObject to store a mapping TKey => DictionaryValueHistory&lt;TValue&gt;.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>
    public interface IVersionedDictionaryObject<TKey, TValue> : IDictionaryObject<TKey, TValue>, IVersionedDictionaryInterface<TKey, TValue>
        where TKey : IComparable
        where TValue : IEstimable
    {
    }

}