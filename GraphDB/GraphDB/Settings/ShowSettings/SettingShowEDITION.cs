#region Usings
using System;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.Session;


#endregion

namespace sones.GraphDB.Settings
{
    public class SettingShowEDITION : ANonPersistentSetting, IDBShowSetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(116);

        #region TypeCode
        public override UInt32 TypeCode { get { return 531; } }
        #endregion

        public SettingShowEDITION()
        {
            Name = "EDITION";
            Description = "Show the edition of an object.";
            Type = DBBoolean.UUID;
            Default = new DBBoolean(false);
            Value = new DBBoolean(false);
        }

        public SettingShowEDITION(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingShowEDITION(ANonPersistentSetting myCopy)
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
            return new SettingShowEDITION(this);
        }

        public override SettingUUID ID
        {
            get { return SettingShowEDITION.UUID; }
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

