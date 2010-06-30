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

/* <id name="sones GraphDB – TypeUUID" />
 * <copyright file="TypeUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class has been created in favour of getting compile errors when referencing a type.</summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphDB.TypeManagement
{
    /// <summary>
    /// This class has been created in favour of getting compile errors when referencing an attribute.
    /// </summary>
    
    public class TypeUUID : UUID
    {

        #region TypeCode 
        public override UInt32 TypeCode { get { return 222; } }
        #endregion

        #region Constructors

        #region TypeUUID()

        public TypeUUID()
            : this(Guid.NewGuid().ToString())
        {
        }

        #endregion

        #region TypeUUID(myUInt64)

        public TypeUUID(UInt64 myUInt64)
            : this(myUInt64.ToString())
        {
        }

        #endregion

        #region TypeUUID(myString)

        public TypeUUID(String myString)
        {
            _UUID = Encoding.UTF8.GetBytes(myString);
        }

        #endregion

        #region TypeUUID(mySerializedData)

        public TypeUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion
        
        #region TypeUUID(ref mySerializationReader)

        public TypeUUID(ref SerializationReader mySerializationReader)
            : base(ref mySerializationReader)
        {
        }

        #endregion

        #endregion


        #region NewUUID()

        public new static TypeUUID NewUUID()
        {
            return new TypeUUID(Guid.NewGuid().ToByteArray());
        }

        #endregion

        #region ToString()

        public override string ToString()
        {
            //return _UUID.ToHexString(SeperatorTypes.NONE, true);
            return Encoding.UTF8.GetString(_UUID);
        }

        #endregion

    }
}
