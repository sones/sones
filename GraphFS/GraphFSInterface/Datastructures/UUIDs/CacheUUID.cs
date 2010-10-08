/*
 * CacheUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class CacheUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 235; } }

        #endregion

        #region Constructors

        #region CacheUUID()

        public CacheUUID()
            : base()
        {
        }

        #endregion

        #region CacheUUID(myUInt64)

        public CacheUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region CacheUUID(myString)

        public CacheUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region CacheUUID(mySerializedData)

        public CacheUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion


        #region NewUUID

        public new static CacheUUID NewUUID
        {
            get
            {
                return new CacheUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion


    }

}
