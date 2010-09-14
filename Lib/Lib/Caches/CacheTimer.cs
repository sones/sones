/*
 * CacheTimer<T>
 * (c) Achim Friedland, 2010
 */

#region Using

using System;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib.Caches
{

    public struct CacheTimer<T>
    {

        #region Data

        private readonly T _Object;
        private UInt64 _Timestamp;

        #endregion

        #region Constructor(s)

        public CacheTimer(T myObject)
        {
            _Object = myObject;
            _Timestamp = TimestampNonce.Ticks;
        }

        #endregion

        #region Object

        public T Object
        {
            get
            {
                _Timestamp = TimestampNonce.Ticks;
                return _Object;
            }
        }

        #endregion

        #region Timestamp

        public UInt64 Timestamp
        {
            get
            {
                return _Timestamp;
            }
        }

        #endregion

    }

}
