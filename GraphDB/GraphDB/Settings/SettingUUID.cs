/* <id name="GraphDB – SettingUUID" />
 * <copyright file="SettingUUID.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 */

#region usings

using System;
using System.Text;

using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;

#endregion

namespace sones.GraphDB.Settings
{

    /// <summary>
    /// This class has been created in favour of getting compile errors when referencing an attribute.
    /// </summary>
    
    public class SettingUUID : UUID
    {

        #region TypeCode

        public override UInt32 TypeCode { get { return 252; } }

        #endregion

        #region Constructors

        #region SettingUUID()

        public SettingUUID()
            : this(Guid.NewGuid().ToString())
        {
        }

        #endregion

        #region SettingUUID(myUInt64)

        public SettingUUID(UInt64 myUInt64)
            : this(myUInt64.ToString())
        {
        }

        #endregion

        #region SettingUUID(myString)

        public SettingUUID(String myString)
        {
            _UUID = Encoding.UTF8.GetBytes(myString);
        }

        #endregion

        #region SettingUUID(mySerializedData)

        public SettingUUID(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        #endregion

        #region SettingUUID(ref mySerializationReader)

        public SettingUUID(ref SerializationReader mySerializationReader)
            : base(ref mySerializationReader)
        {
        }

        #endregion

        #endregion


        #region NewUUID()

        public new static SettingUUID NewUUID
        {
            get
            {
                return new SettingUUID(Guid.NewGuid().ToByteArray());
            }
        }

        #endregion

        #region ToString()

        public override string ToString()
        {
            return Encoding.UTF8.GetString(_UUID);
        }

        #endregion

    }
}
