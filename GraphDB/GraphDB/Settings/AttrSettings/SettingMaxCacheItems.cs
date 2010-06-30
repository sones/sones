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

/* <id name="sones GraphDB – Setting MaxCacheItems" />
 * <copyright file="SettingMaxCacheItems.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This setting limits the count of DBObject/BackwardEdge tuples in the DBObjectQueryCache.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using sones.Lib.Settings;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDB.Settings
{
    /// <summary>
    /// This setting limits the count of DBObject/BackwardEdge tuples in the DBObjectQueryCache.
    /// </summary>

    public class SettingMaxCacheItems : ANonPersistentSetting, IDBAttributeSetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(105);

        #region TypeCode
        public override UInt32 TypeCode { get { return 524; } }
        #endregion

        public SettingMaxCacheItems()
        {
            Name = "SettingMaxCacheItems";
            Description = "This setting limits the count of DBObject/BackwardEdge tuples in the DBObjectQueryCache.";
            Type = DBInt64.UUID;
            Default = new DBInt64(2000000);
            this._Value = Default.Clone();
        }


        public SettingMaxCacheItems(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingMaxCacheItems(ANonPersistentSetting myCopy)
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
            return new SettingMaxCacheItems(this);
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
