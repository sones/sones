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


/*
 * ObjectUUID
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;

using sones.Lib;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphFS.DataStructures
{


    public sealed class ObjectUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 231; } }

        #endregion

        #region Constructors

        #region ObjectUUID()

        public ObjectUUID()
            : this(Guid.NewGuid().ToString())
        { }

        #endregion

        #region ObjectUUID(myBoolean)

        public ObjectUUID(Boolean myBoolean)
        {
            _UUID = new Byte[0];
        }

        #endregion

        #region ObjectUUID(myUInt64)

        public ObjectUUID(UInt64 myUInt64)
            : this(myUInt64.ToString())
        {
//            _UUID = BitConverter.GetBytes(myUInt64);
        }

        #endregion

        #region ObjectUUID(myString)

        /// <summary>
        /// Create a new ObjectUUID parsing the <paramref name="myString"/> as hex string.
        /// Do NOT use this for own created ObjectUUIDs!! User ObjectUUID.FromString instead!
        /// </summary>
        /// <param name="myString"></param>
        public ObjectUUID(String myString)
        {
            _UUID = Encoding.UTF8.GetBytes(myString);
        }

        #endregion

        #region ObjectUUID(mySerializedData)

        public ObjectUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #endregion


        #region NewUUID

        public new static ObjectUUID NewUUID
        {
            get
            {
                return new ObjectUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion


        #region Like ICloneable Members

        public new ObjectUUID Clone()
        {
            ObjectUUID newUUID = new ObjectUUID(_UUID);
            return newUUID;
        }

        #endregion

        #region Statics

        public static ObjectUUID FromHexString(String myHexString)
        {
            return new ObjectUUID(ByteArrayHelper.FromHexString(myHexString));
        }

        /// <summary>
        /// Create a ObjectUUID using the <paramref name="myString"/>. Each char of this string is one byte.
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        public static ObjectUUID FromString(String myString)
        {
            return new ObjectUUID(Encoding.UTF8.GetBytes(myString));
        }

        #endregion

        #region Implizit operators
        /*
        public static implicit operator ObjectUUID(String myObjectUUIDAsString)
        {
            return new ObjectUUID(myObjectUUIDAsString);
        }
        */
        #endregion

        public override string ToString()
        {
            //return _UUID.ToHexString(SeperatorTypes.NONE, true);
            return Encoding.UTF8.GetString(_UUID);
        }

    }

}
