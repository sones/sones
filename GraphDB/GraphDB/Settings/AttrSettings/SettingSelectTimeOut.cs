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
 * <copyright file="SettingObjectPath.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;

using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.NewFastSerializer;
using sones.GraphFS.Session;



#endregion

namespace sones.GraphDB.Settings
{
    //Todo: why ADBAttributeSetting !?!?
    public class SettingSelectTimeOut : ANonPersistentSetting, IDBAttributeSetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(102);

        #region TypeCode
        public override UInt32 TypeCode { get { return 523; } }
        #endregion

        public SettingSelectTimeOut()
        {
            Name = "SELECTTIMEOUT"; //uppercase
            Description = "TimeOut for Select";
            Type = DBInt64.UUID;
            Default = new DBInt64(300000);
            this._Value = Default.Clone();
        }

        public SettingSelectTimeOut(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingSelectTimeOut(ANonPersistentSetting myCopy)
            : base(myCopy)
        { }

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
            return new SettingSelectTimeOut(this);
        }

        #region IFastSerializationTypeSurrogate Members

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
