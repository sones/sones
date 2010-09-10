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

/* <id name="GraphDB – Settings" />
 * <copyright file="SettingTypeDirBlocksize.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary></summary>
 */

#region Usings

using System;

using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Session;
using sones.Lib.NewFastSerializer;

#endregion

namespace sones.GraphDB.Settings
{
    public class SettingTypeDirBlocksize : APersistentSetting, IDBAttributeSetting
    {

        #region Data

        public static readonly SettingUUID UUID = new SettingUUID(112);

        #endregion

        #region Constructors

        #region SettingTypeDirBlocksize()

        public SettingTypeDirBlocksize()
            : this(0)
        { }

        #endregion

        #region SettingTypeDirBlocksize(myValue)

        public SettingTypeDirBlocksize(UInt64 myValue)
        {

            Name        = "TYPEDIRBLOCKSIZE";
            Description = "The file system blocksize of the type directory.";
            Type        = DBInt64.UUID;
            Default     = new DBInt64(1000000L);

            if (myValue == 0)
                _Value = Default.Clone();
            else
                _Value = new DBInt64((Int64) myValue);

        }

        #endregion

        #region SettingTypeDirBlocksize(mySerializedData)

        public SettingTypeDirBlocksize(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        #endregion

        #region SettingTypeDirBlocksize(myCopy)

        public SettingTypeDirBlocksize(APersistentSetting myCopy)
            : base(myCopy)
        { }

        #endregion

        #endregion


        public override SettingUUID ID
        {
            get { return UUID; }
        }
        
        public override ADBBaseObject Value
        {

            get
            {
                return this._Value;
            }

            set
            {
                this._Value = value;
            }

        }

        public override ISettings Clone()
        {
            return new SettingTypeDirBlocksize(this);
        }

        #region IFastSerializationTypeSurrogate Members

        public override UInt32 TypeCode { get { return 527; } }

        public new bool SupportsType(Type type)
        {
            return GetType() == type;
        }

        public new void Serialize(SerializationWriter writer, object value)
        {
            base.Serialize(writer, this);
        }

        public new object Deserialize(SerializationReader reader, Type type)
        {
            return base.Deserialize(reader, type);
        }

        #endregion

    }
}
