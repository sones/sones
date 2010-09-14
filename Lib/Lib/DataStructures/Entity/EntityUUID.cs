using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.UUID;

namespace sones.GraphFS.DataStructures
{

    public sealed class EntityUUID : UUID
    {

        #region TypeCode
        public override UInt32 TypeCode { get { return 223; } }
        #endregion

        #region Constructors

        #region EntityUUID()

        public EntityUUID()
            : base()
        {
        }

        #endregion

        #region EntityUUID(myUInt64)

        public EntityUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region EntityUUID(myString)

        public EntityUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region EntityUUID(mySerializedData)

        public EntityUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion


        #region NewUUID

        public static EntityUUID NewUUID
        {
            get
            {
                return new EntityUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

    }

}
