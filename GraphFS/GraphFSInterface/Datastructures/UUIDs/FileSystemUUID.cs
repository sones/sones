/*
 * FileSystemUUID
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{

    public sealed class FileSystemUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 233; } }

        #endregion

        #region Constructors

        #region FileSystemUUID()

        public FileSystemUUID()
            : base()
        {
        }

        #endregion

        #region FileSystemUUID(myUInt64)

        public FileSystemUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region FileSystemUUID(myString)

        public FileSystemUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region FileSystemUUID(mySerializedData)

        public FileSystemUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region FileSystemUUID(myUUID)

        /// <summary>
        /// Generates a UUID based on the content of myUUID
        /// </summary>
        /// <param name="myUUID">A UUID</param>
        public FileSystemUUID(UUID myUUID)
        {
            var _ByteArray = myUUID.GetByteArray();
            _UUID = new Byte[_ByteArray.LongLength];
            Array.Copy(_ByteArray, 0, _UUID, 0, _ByteArray.LongLength);
        }

        #endregion

        #endregion

        #region NewUUID

        public new static FileSystemUUID NewUUID
        {
            get
            {
                return new FileSystemUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

    }

}
