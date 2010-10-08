using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.DataStructures.UUID;

namespace sones.GraphFS.InternalObjects
{

    public sealed class RightUUID : UUID
    {

        #region TypeCode 
        public override UInt32 TypeCode { get { return 225; } }
        #endregion

        #region Constructors

        #region RightUUID()

        public RightUUID()
            : base()
        {
        }

        #endregion

        #region RightUUID(myUInt64)

        public RightUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region RightUUID(myString)

        public RightUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region RightUUID(mySerializedData)

        public RightUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion
    }

}
