using System;
using System.Collections.Generic;

namespace sones.Plugins.Index.Interfaces
{
    #region IIndexVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IIndex plugin versions. 
    /// Defines the min and max version for all IIndex implementations which will be activated used this IIndex.
    /// </summary>
    public static class IIndexVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

    /// <summary>
    /// Root interface for all kinds of indices
    /// </summary>
    /// <typeparam name="TKey">The type of the comparable index key</typeparam>
    /// <typeparam name="TValue">The type of the index value</typeparam>
    public interface IIndex<TKey, TValue>
        where TKey : IComparable
    {
        #region General Properties

        /// <summary>
        /// Is this a persistent index?
        /// </summary>
        Boolean IsPersistent { get; }

        /// <summary>
        /// Returns the name of the index
        /// </summary>
        String Name { get; }

        #endregion

        #region Counts

        /// <summary>
        /// Returns the number of keys
        /// </summary>
        Int64 KeyCount();

        /// <summary>
        /// Returns the number of values
        /// </summary>
        /// <returns></returns>
        Int64 ValueCount();

        #endregion

        #region Keys

        /// <summary>
        /// Returns all keys of the index
        /// </summary>
        ISet<TKey> Keys();

        #endregion

        #region ContainsKey / -Value / -KeyValue

        /// <summary>
        /// Checks if a key exists in the index
        /// </summary>
        /// <param name="myKey">the key to be checked</param>
        /// <returns>true if the key exists, else false</returns>
        bool ContainsKey(TKey myKey);

        /// <summary>
        /// Checks if a value exists in the index
        /// </summary>
        /// <param name="myValue">the value to be checked</param>
        /// <returns>true if the value exists, else false</returns>
        bool ContainsValue(TValue myValue);

        /// <summary>
        /// Checks if a key and an associated value exists in the index
        /// </summary>
        /// <param name="myKey">the key</param>
        /// <param name="myValue">the associated value</param>
        /// <returns>true if the key and the associated balue exist, else false</returns>
        bool Contains(TKey myKey, TValue myValue);

        #endregion

        #region Remove

        /// <summary>
        /// Removes a given key and its associated value from the index
        /// </summary>
        /// <param name="myKey">the key to be removed</param>
        /// <returns></returns>
        bool Remove(TKey myKey);

        /// <summary>
        /// Clears the index
        /// </summary>
        void ClearIndex();

        #endregion
    }
}