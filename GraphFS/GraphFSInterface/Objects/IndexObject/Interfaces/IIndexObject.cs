/*
 * IIndexObject
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;

using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface for all GraphFS IndexObjects.
    /// </summary>
    public interface IIndexObject<TKey, TValue> : IIndexInterface<TKey, TValue>, IObjectLocation
        where TKey : IComparable
    {

        #region GetNewInstance()

        IIndexObject<TKey, TValue> GetNewInstance();

        #endregion

        #region Members of AGraphHeader

        Boolean       isNew                     { get; set; }
        INode         INodeReference            { get; }
        ObjectLocator ObjectLocatorReference    { get; set; }
        ObjectUUID    ObjectUUID                { get; }

        #endregion

        #region Members of IFastSerialize

        Boolean       isDirty                   { get; set; }

        #endregion

    }

}
