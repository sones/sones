/*
 * ForestUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class ForestUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 229; } }

        #endregion

        #region Constructors

        #region ForestUUID()

        public ForestUUID()
            : base()
        {
        }

        #endregion

        #region ForestUUID(myUInt64)

        public ForestUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region ForestUUID(myString)

        public ForestUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region ForestUUID(mySerializedData)

        public ForestUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region ForestUUID(myUUID)

        /// <summary>
        /// Generates a UUID based on the content of myUUID
        /// </summary>
        /// <param name="myUUID">A UUID</param>
        public ForestUUID(UUID myUUID)
            : base(myUUID)
        {   
        }

        #endregion

        #endregion

        #region NewUUID

        public new static ForestUUID NewUUID
        {
            get
            {
                return new ForestUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

    }

}
