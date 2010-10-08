/* <id name="GraphDB – Setting MaxCacheItems" />
 * <copyright file="SettingMaxCacheItems.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This setting limits the count of DBObject/BackwardEdge tuples in the DBObjectQueryCache.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using sones.GraphFS.Session;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
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
