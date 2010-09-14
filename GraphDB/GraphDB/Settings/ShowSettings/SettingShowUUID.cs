/* <id name="GraphDB – Settings" />
 * <copyright file="SettingUUID.cs"
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
    public class SettingShowUUID : ANonPersistentSetting, IDBShowSetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(103);
        public static readonly String SettingName = "UUID";

        #region TypeCode
        public override UInt32 TypeCode { get { return 513; } }
        #endregion

        public SettingShowUUID()
        {
            Name        = SettingShowUUID.SettingName;
            Description = "Show the uuid of an object.";
            Type        = DBBoolean.UUID;
            Default     = new DBBoolean(false);
            this._Value = new DBBoolean(false);
        }

        public SettingShowUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingShowUUID(ANonPersistentSetting myCopy)
            : base(myCopy)
        { }

        public override ADBBaseObject Value
        {
            get
            {
                return _Value;
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
            return new SettingShowUUID(this);
        }

        public override SettingUUID ID
        {
            get { return UUID; }
        }

        public Boolean IsShown()
        {
            if (Value != null)
                return (Value as DBBoolean).GetValue();
            else if (Default != null)
                return (Default as DBBoolean).GetValue();
            else return false;
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
