/* <id name="GraphDB – readonly setting" />
 * <copyright file="SettingReadonly.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
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
    public class SettingReadonly : APersistentSetting, IDBDatabaseOnlySetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(115);

        #region TypeCode
        public override UInt32 TypeCode { get { return 530; } }
        #endregion

        public SettingReadonly()
        {
            Name            = "SETREADONLY";
            Description     = "Decides whether the database is readonly or writeable.";
            Type            = DBBoolean.UUID;
            Default         = new DBBoolean(false);
            Value           = new DBBoolean(false);
        }

        public SettingReadonly(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingReadonly(APersistentSetting myCopy)
            : base(myCopy)
        { }

        public override ADBBaseObject Value
        {
            get
            {
                return this._Value;
            }

            set
            {
                _Value = value;
            }
        }
        
        private bool IsLittleEndian()
        {
            throw new NotImplementedException();
        }

        public override ISettings Clone()
        {
            return new SettingReadonly(this);
        }

        public override SettingUUID ID
        {
            get { return SettingReadonly.UUID; }
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
