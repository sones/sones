/*
 * IDictionaryObject
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;

using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Dictionaries;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface of a DictionaryObject to store a mapping TKey => TValue.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>

    public interface IDictionaryObject<TKey, TValue> : IDictionaryInterface<TKey, TValue>, IObjectLocation
        where TKey : IComparable
    {

        #region Members of AGraphHeader

        Boolean       isNew                   { get; set; }
        INode         INodeReference          { get; }
        ObjectLocator ObjectLocatorReference  { get; set; }
        ObjectUUID    ObjectUUID              { get; }

        #endregion

        #region Members of IFastSerialize

        Boolean       isDirty                 { get; set; }

        #endregion

    }

}