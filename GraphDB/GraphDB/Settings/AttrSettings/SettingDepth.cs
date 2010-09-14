/* <id name="GraphDB – Settings" />
 * <copyright file="SettingsDepth.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;

using sones.GraphFS.Session;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.NewFastSerializer;


#endregion

namespace sones.GraphDB.Settings
{

    public class SettingDepth : ANonPersistentSetting, IDBAttributeSetting
    {

        public static readonly SettingUUID UUID = new SettingUUID(100);

        #region TypeCode
        public override UInt32 TypeCode { get { return 509; } }
        #endregion

        public SettingDepth()
        {
            Name            = "DEPTH";
            Description     = "The depth of an request.";
            Type            = DBInt64.UUID;
            Default         = new DBInt64(0L);
            this._Value     = Default.Clone();
        }


        public SettingDepth(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingDepth(ANonPersistentSetting myCopy)
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
            return new SettingDepth(this);
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
