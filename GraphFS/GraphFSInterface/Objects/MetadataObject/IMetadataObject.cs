/* 
 * IMetadataObject
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;

using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface for all Graph directory objects and virtual
    /// directory objects.
    /// </summary>

    public interface IMetadataObject<TValue> : IObjectLocation
    {

        #region Members of AGraphHeader

        Boolean       isNew                   { get; set; }
        INode         INodeReference          { get; }
        ObjectLocator ObjectLocatorReference  { get; set; }
        ObjectUUID    ObjectUUID              { get; }

        #endregion


        #region Members of IFastSerialize

        Boolean isDirty { get; set; }

        #endregion


        #region Members of IMetadataObject

        #region Add

        void Add(String myKey, TValue myValue);

        void Add(params Func<String, TValue>[] myFunc);

        void Add(String myKey, IEnumerable<TValue> myValues);

        void Add(KeyValuePair<String, TValue> myKeyValuePair);

        void Add(KeyValuePair<String, IEnumerable<TValue>> myKeyValuesPair);

        void Add(Dictionary<String, TValue> myDictionary);

        void Add(Dictionary<String, IEnumerable<TValue>> myDictionary);

        #endregion

        #region Set

        void Set(String myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy);

        void Set(String myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy);

        void Set(IEnumerable<KeyValuePair<String, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy);

        void Set(KeyValuePair<String, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy);

        void Set(KeyValuePair<String, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy);

        void Set(Dictionary<String, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy);

        void Set(Dictionary<String, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy);

        #endregion

        #region Contains

        Trinary ContainsKey(String myKey);

        Trinary ContainsValue(TValue myValue);

        Trinary Contains(String myKey, TValue myValue);

        Trinary Contains(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);

        #endregion

        #region Get/Keys/Values/Enumerator

        HashSet<TValue>                 this[String key] { get; set; }
        Boolean                         TryGetValue(String key, out HashSet<TValue> value);

        IEnumerable<String>             Keys();
        IEnumerable<String>             Keys(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);
        UInt64                          KeyCount();
        UInt64                          KeyCount(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);

        IEnumerable<HashSet<TValue>>    Values();
        IEnumerable<HashSet<TValue>>    Values(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);
        UInt64                          ValueCount();
        UInt64                          ValueCount(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);

        IDictionary<String, HashSet<TValue>> GetIDictionary();
        IDictionary<String, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);

        IEnumerator<KeyValuePair<String, HashSet<TValue>>> GetEnumerator();
        IEnumerator<KeyValuePair<String, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);

        #endregion

        #region Remove/Clear

        Boolean Remove(String myKey);

        Boolean Remove(String myKey, TValue myValue);

        Boolean Remove(Func<KeyValuePair<String, IEnumerable<TValue>>, Boolean> myFunc);

        void Clear();

        #endregion

        #region NotificationHandling

        /// <summary>
        /// Returns the NotificationHandling bitfield that indicates which
        /// notifications should be triggered.
        /// </summary>
        NHIMetadataObject NotificationHandling { get; }

        /// <summary>
        /// This method adds the given NotificationHandling flags.
        /// </summary>
        /// <param name="myNotificationHandling">The NotificationHandlings to be added.</param>
        void SubscribeNotification(NHIMetadataObject myNotificationHandling);

        /// <summary>
        /// This method removes the given NotificationHandling flags.
        /// </summary>
        /// <param name="myNotificationHandling">The NotificationHandlings to be removed.</param>
        void UnsubscribeNotification(NHIMetadataObject myNotificationHandling);

        #endregion

        #endregion

    }

}