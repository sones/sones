#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.GraphFS.Session;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.DataStructures;
using sones.GraphDB.Exceptions;
using sones.Lib;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.Settings
{
    public class SettingShowLASTMODIFICATIONTIME : ANonPersistentSetting, IDBShowSetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(120);

        #region TypeCode
        public override UInt32 TypeCode { get { return 536; } }
        #endregion

        public SettingShowLASTMODIFICATIONTIME()
        {
            Name = "LASTMODIFICATIONTIME";
            Description = "Show the last modification time of an object.";
            Type = DBBoolean.UUID;
            Default = new DBBoolean(false);
            Value = new DBBoolean(false);
        }

        public SettingShowLASTMODIFICATIONTIME(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingShowLASTMODIFICATIONTIME(ANonPersistentSetting myCopy)
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
            return new SettingShowLASTMODIFICATIONTIME(this);
        }

        public override SettingUUID ID
        {
            get { return SettingShowLASTMODIFICATIONTIME.UUID; }
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

