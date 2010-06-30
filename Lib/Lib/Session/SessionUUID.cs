/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.DataStructures.UUID;
using sones.Lib;

namespace sones.Lib.Session
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

