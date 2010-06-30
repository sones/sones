
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Libraries.DataStructures.UUID;
using sones.Pandora.Lib;

namespace sones.GraphFS.Session
{

    public sealed class SessionUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 234; } }

        #endregion

        #region Constructors

        #region SessionUUID()

        public SessionUUID()
            : this(Guid.NewGuid().ToString())
        { }

        #endregion

        #region SessionUUID(myBoolean)

        public SessionUUID(Boolean myBoolean)
        {
            _UUID = new Byte[0];
        }

        #endregion

        #region SessionUUID(myUInt64)

        public SessionUUID(UInt64 myUInt64)
        {
            _UUID = BitConverter.GetBytes(myUInt64).TakeWhile(aByte => aByte != 0).ToArray<Byte>();
        }

        #endregion

        #region SessionUUID(myString)

        /// <summary>
        /// Create a new ObjectUUID parsing the <paramref name="myString"/> as hex string.
        /// Do NOT use this for own created ObjectUUIDs!! User ObjectUUID.FromString instead!
        /// </summary>
        /// <param name="myString"></param>
        public SessionUUID(String myString)
        {
            _UUID = Encoding.UTF8.GetBytes(myString);
        }

        #endregion

        #region SessionUUID(mySerializedData)

        public SessionUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion


        #region NewUUID

        public new static SessionUUID NewUUID
        {
            get
            {
                return new SessionUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion


        #region Like ICloneable Members

        public new SessionUUID Clone()
        {
            var newUUID = new SessionUUID(_UUID);
            return newUUID;
        }

        #endregion

        #region Statics

        public static SessionUUID FromHexString(String myHexString)
        {
            return new SessionUUID(ByteArrayHelper.FromHexString(myHexString));
        }

        /// <summary>
        /// Create a ObjectUUID using the <paramref name="myString"/>. Each char of this string is one byte.
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        public static SessionUUID FromString(String myString)
        {
            return new SessionUUID(Encoding.UTF8.GetBytes(myString));
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return _UUID.ToHexString(SeperatorTypes.COLON, true);
        }

        #endregion

    }

}

