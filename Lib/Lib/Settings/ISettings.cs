/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name="PandoraLib – Settings" />
 * <copyright file="ISettings.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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

namespace sones.Lib.Settings
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
