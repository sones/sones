/* <id name="GraphDB – Settings" />
 * <copyright file="SettingMandatoryError.cs"
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
    public class SettingDefaultsOnMandatory : APersistentSetting, IDBAttributeSetting
    {

        public static readonly SettingUUID UUID = new SettingUUID(106);

        #region TypeCode
        public override UInt32 TypeCode { get { return 508; } }
        #endregion

        public SettingDefaultsOnMandatory()
        {
            Name        = "USE_DEFAULTS_ON_MANDATORY";
            Description = "Set the behaviour for mandatory attributes.";
            Type        = DBBoolean.UUID;
            Default     = new DBBoolean(false);
            this._Value = Default.Clone();
        }


        public SettingDefaultsOnMandatory(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingDefaultsOnMandatory(APersistentSetting myCopy)
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
            return new SettingDefaultsOnMandatory(this);
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
