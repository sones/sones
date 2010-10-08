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
    public class IndexValueHistory<TValue> : IEstimable
        where TValue : IEstimable
    {

        #region Properties

        public  UInt64            Timestamp   { get; set; }
        public  HashSet<TValue>   AddSet      { get; set; }
        public  HashSet<TValue>   RemSet      { get; set; }

        private UInt64            _estimatedSize = 0;

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

            //ClassDefaultSize + EstimatedSize + TimeStamp + AddSet + RemoveSet
            _estimatedSize = GetBaseSize();
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
            #region estimated size

            _estimatedSize = GetBaseSize();

            #endregion

            Timestamp = myTimestamp;

            #region AddSet

            if (AddSet == null)
            {
                AddSet = new HashSet<TValue>();
            }

            if (myAddSet != null)
            {
                foreach (var aAddElement in myAddSet)
                {
                    AddSet.Add(aAddElement);

                    #region estimated size

                    if (aAddElement != null)
                    {
                        _estimatedSize += aAddElement.GetEstimatedSize();
                    }

                    #endregion
                }
            }

            #endregion

            #region RemSet

            if (RemSet == null)
            {
                RemSet = new HashSet<TValue>();
            }

            if (myRemSet != null)
            {
                foreach (var aRemElement in myRemSet)
                {
                    RemSet.Add(aRemElement);

                    #region estimated size

                    if (aRemElement != null)
                    {
                        _estimatedSize += aRemElement.GetEstimatedSize();
                    }

                    #endregion
                }
            }

            #endregion
        }

        #endregion

        #endregion


        #region GetHashCode()

        public override int GetHashCode()
        {
            return Timestamp.GetHashCode() ^ AddSet.GetHashCode() ^ RemSet.GetHashCode();
        }

        #endregion

        #region IEstimable members

        public ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        private ulong GetBaseSize()
        {
            return EstimatedSizeConstants.ClassDefaultSize + EstimatedSizeConstants.UInt64 + EstimatedSizeConstants.UInt64 + EstimatedSizeConstants.HashSet + EstimatedSizeConstants.HashSet;
        }

        #endregion
    }

}
