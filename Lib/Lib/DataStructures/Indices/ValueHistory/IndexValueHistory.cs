/*
 * IndexValueHistory
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    /// <summary>
    /// This datastructure implements a versioned and serialized value
    /// to be used within index datastructures
    /// </summary>
    /// <typeparam name="TValue">The type of the stored value</typeparam>
    public class IndexValueHistory<TValue>
    {

        #region Properties

        public  UInt64            Timestamp   { get; set; }
        public  HashSet<TValue>   AddSet      { get; set; }
        public  HashSet<TValue>   RemSet      { get; set; }

        #endregion


        #region Constructors

        #region IndexValueHistory()

        /// <summary>
        /// Creates a new IndexValueHistory using default values
        /// </summary>
        public IndexValueHistory()
        {
            Timestamp           = default(Int64);
            AddSet              = new HashSet<TValue>();
            RemSet              = new HashSet<TValue>();
        }

        #endregion

        #region IndexValueHistory(myAddSet, myRemSet)

        /// <summary>
        /// Creates a new IndexValueHistory, setting the internal value to the content of myValue
        /// </summary>
        public IndexValueHistory(IEnumerable<TValue> myAddSet, IEnumerable<TValue> myRemSet)
            : this(TimestampNonce.Ticks, myAddSet, myRemSet)
        {
        }

        #endregion

        #region IndexValueHistory(myTimestamp, myAddSet, myRemSet)

        /// <summary>
        /// Creates a new IndexValueHistory, setting the internal value to the content of myValue
        /// </summary>
        public IndexValueHistory(UInt64 myTimestamp, IEnumerable<TValue> myAddSet, IEnumerable<TValue> myRemSet)
        {

            Timestamp = myTimestamp;

            if (myAddSet == null)
                AddSet = new HashSet<TValue>();
            else
                AddSet = new HashSet<TValue>(myAddSet);

            if (myRemSet == null)
                RemSet = new HashSet<TValue>();
            else
                RemSet = new HashSet<TValue>(myRemSet);

        }

        #endregion

        #endregion


        #region GetHashCode()

        public override int GetHashCode()
        {
            return Timestamp.GetHashCode() ^ AddSet.GetHashCode() ^ RemSet.GetHashCode();
        }

        #endregion

    }

}
