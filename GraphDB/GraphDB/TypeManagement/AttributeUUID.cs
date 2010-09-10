/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="PandoraDB – AttributeUUID" />
 * <copyright file="AttributeUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class has been created in favour of getting compile errors when referencing an attribute.</summary>
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
    
    public class AttributeUUID : UUID
    {

        #region TypeCode
        public override UInt32 TypeCode { get { return 221; } }
        #endregion

        #region Constructors

        #region AttributeUUID()

        public AttributeUUID()
            : base()
        {
        }

        #endregion

        #region AttributeUUID(myUInt64)

        public AttributeUUID(UInt64 myUInt64)
            : base(myUInt64)
        {
        }

        #endregion

        #region AttributeUUID(myString)

        public AttributeUUID(String myString)
            : base(myString)
        {
        }

        #endregion

        #region AttributeUUID(mySerializedData)

        public AttributeUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region AttributeUUID(ref SerializationReader mySerializationReader)

        public AttributeUUID(ref SerializationReader mySerializationReader)
            : base(ref mySerializationReader)
        {
        }

        #endregion

        #region AttributeUUID(UUID)

        public AttributeUUID(UUID myUUID)
            : base(myUUID)
        {
        }

        #endregion

        #endregion


        #region NewUUID()

        public new static AttributeUUID NewUUID()
        {
            return new AttributeUUID(Guid.NewGuid().ToByteArray());
        }

        #endregion

    }
}
