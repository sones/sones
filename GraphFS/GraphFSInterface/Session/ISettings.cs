/* <id name="GraphLib – Settings" />
 * <copyright file="ISettings.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

namespace sones.GraphFS.Session
{

    public interface ISettings
    {

        ISettings   Clone();

        //ADBBaseObject Default { get; }
        String      Description      { get; }
        //void Deserialize(ref sones.Lib.NewFastSerializer.SerializationReader mySerializationReader);
        //object Deserialize(sones.Lib.NewFastSerializer.SerializationReader reader, Type type);
        //void Deserialize(byte[] mySerializedData);

        //Boolean     doValidate(ADBSettingsBase.ValidateFunc pFunc, ADBSettingsBase pParam1, ADBSettingsBase pParam2);
        //SettingUUID ID               { get; }
        Boolean     isDirty          { get; set; }
        DateTime    ModificationTime { get; }
        String      Name             { get; }
        EntityUUID  OwnerID          { get; }

        //byte[] Serialize();
        //void Serialize(ref sones.Lib.NewFastSerializer.SerializationWriter mySerializationWriter);
        //void Serialize(sones.Lib.NewFastSerializer.SerializationWriter writer, object value);
        
        Boolean     SupportsType(Type myType);
        //sones.Graph.Database.TypeManagement.TypeUUID Type { get; }
        UInt32      TypeCode        { get; }
        //sones.Graph.Database.TypeManagement.GraphTypes.ADBBaseObject Value { get; set; }
    }

}
