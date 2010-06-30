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


/* PandoraFS - StorageUUID
 * Achim Friedland, 2009
 *  
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.StorageEngines
{

    
    public sealed class StorageUUID : UUID, ICloneable
    {

        #region TypeCode
        
        public override UInt32 TypeCode { get { return 227; } }
        
        #endregion

        #region Constructors

        #region StorageUUID()

        public StorageUUID()
            : base()
        {
        }

        #endregion

        #region StorageUUID(myUInt64)

        public StorageUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region StorageUUID(myString)

        public StorageUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region StorageUUID(mySerializedData)

        public StorageUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region StorageUUID(ref mySerializationReader)

        public StorageUUID(ref SerializationReader mySerializationReader)
            : base(ref mySerializationReader)
        {
            Deserialize(ref mySerializationReader);
        }

        #endregion

        #region StorageUUID(myStorageUUID)

        public StorageUUID(StorageUUID myStorageUUID)
            : this(myStorageUUID._UUID)
        {
        }

        #endregion

        #endregion


        #region Like ICloneable Members

        public new object Clone()
        {
            var newUUID = new StorageUUID(_UUID);
            return newUUID;
        }

        #endregion



        #region NewUUID

        public new static StorageUUID NewUUID
        {
            get
            {
                return new StorageUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion


        #region Implicit conversation from UInt64

        public static implicit operator StorageUUID(UInt64 myParameter)
        {
            return new StorageUUID(myParameter);
        }

        #endregion


        #region Statics

        public static StorageUUID FromHexString(String myHexString)
        {
            return new StorageUUID(ByteArrayHelper.FromHexString(myHexString));
        }

        #endregion
                
    }

}
