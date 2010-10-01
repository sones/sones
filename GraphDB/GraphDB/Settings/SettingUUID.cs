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

/* <id name="GraphDB – SettingUUID" />
 * <copyright file="SettingUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 */

#region usings

using System;
using System.Text;

using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphDB.Settings
{

    /// <summary>
    /// This class has been created in favour of getting compile errors when referencing an attribute.
    /// </summary>
    
    public class SettingUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 252; } }

        #endregion

        #region Constructors

        #region SettingUUID()

        public SettingUUID()
            : this(Guid.NewGuid().ToString())
        {
        }

        #endregion

        #region SettingUUID(myUInt64)

        public SettingUUID(UInt64 myUInt64)
            : this(myUInt64.ToString())
        {
        }

        #endregion

        #region SettingUUID(myString)

        public SettingUUID(String myString)
        {
            _UUID = Encoding.UTF8.GetBytes(myString);
        }

        #endregion

        #region SettingUUID(mySerializedData)

        public SettingUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region SettingUUID(ref mySerializationReader)

        public SettingUUID(ref SerializationReader mySerializationReader)
            : base(ref mySerializationReader)
        {
        }

        #endregion

        #endregion


        #region NewUUID()

        public new static SettingUUID NewUUID
        {
            get
            {
                return new SettingUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

        #region ToString()

        public override string ToString()
        {
            return Encoding.UTF8.GetString(_UUID);
        }

        #endregion

    }
}
