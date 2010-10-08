/*
 * TransactionUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class TransactionUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 251; } }

        #endregion

        #region Constructors

        #region TransactionUUID()

        public TransactionUUID()
            : base()
        {
        }

        #endregion

        #region TransactionUUID(myUInt64)

        public TransactionUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region TransactionUUID(myString)

        public TransactionUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region TransactionUUID(mySerializedData)

        public TransactionUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion

        #region NewUUID

        public new static TransactionUUID NewUUID
        {
            get
            {
                return new TransactionUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

    }

}
