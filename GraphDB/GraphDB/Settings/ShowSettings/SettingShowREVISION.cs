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
    public class SettingShowREVISION : ANonPersistentSetting, IDBShowSetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(114);

        #region TypeCode
        public override UInt32 TypeCode { get { return 529; } }
        #endregion

        public SettingShowREVISION()
        {
            Name            = "REVISION";
            Description     = "Show the revision of an object.";
            Type            = DBBoolean.UUID;
            Default         = new DBBoolean(false);
            Value           = new DBBoolean(false);
        }

        public SettingShowREVISION(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingShowREVISION(ANonPersistentSetting myCopy)
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
            return new SettingShowREVISION(this);
        }

        public override SettingUUID ID
        {
            get { return SettingShowREVISION.UUID; }
        }

        public Boolean IsShown()
        {
            if (Value != null)
                return (Boolean)Value.Value;
            else if (Default != null)
                return (Boolean)Default.Value;
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
