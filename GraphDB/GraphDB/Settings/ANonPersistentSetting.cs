/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="GraphDB – ANonPersistentSetting" />
 * <copyright file="ANonPersistentSetting.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings


using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;
using System;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Errors;
using sones.Lib.Settings;

#endregion

namespace sones.GraphDB.Settings
{
    public abstract class ANonPersistentSetting: ADBSettingsBase
	{
        public ANonPersistentSetting()
        {
        }

        public ANonPersistentSetting(String myName, String myDesc, EntityUUID myOwner, UUID myType, ADBBaseObject myDefault)
        {
        }
        
        public ANonPersistentSetting(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        public ANonPersistentSetting(ANonPersistentSetting myCopy)
            : base(myCopy)
        {            
        }

        public override Exceptional<bool> Set(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null)
        {
            switch (scope)
            {
                case TypesSettingScope.DB:
                    #region db

                    return new Exceptional<bool>(context.SessionSettings.SetDBSetting(this));

                    #endregion

                case TypesSettingScope.SESSION:
                    #region session

                    return new Exceptional<bool>(context.SessionSettings.SetSessionSetting(this));

                    #endregion

                case TypesSettingScope.TYPE:
                    #region type

                    if (type != null)
                    {
                        return new Exceptional<bool>(context.SessionSettings.SetTypeSetting(this, type.UUID));
                    }

                    return new Exceptional<bool>(new Error_CouldNotSetSetting(this, scope));
                    #endregion

                case TypesSettingScope.ATTRIBUTE:
                    #region attribute

                    if ((type != null) && (attribute != null))
                    {
                        return new Exceptional<bool>(context.SessionSettings.SetAttributeSetting(this, type.UUID, attribute.UUID));
                    }

                    return new Exceptional<bool>(new Error_CouldNotSetSetting(this, scope, type, attribute));
                    #endregion

                default:

                    return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
            }
        }

        public override Exceptional<ADBSettingsBase> Get(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null)
        {
            switch (scope)
            {
                case TypesSettingScope.DB:
                    #region db

                    return new Exceptional<ADBSettingsBase>(context.SessionSettings.GetDBSetting(this.Name));

                    #endregion

                case TypesSettingScope.SESSION:
                    #region session

                    return new Exceptional<ADBSettingsBase>(context.SessionSettings.GetSessionSetting(this.Name));
                    
                    #endregion

                case TypesSettingScope.TYPE:
                    #region type

                    if (type != null)
                    {
                        return new Exceptional<ADBSettingsBase>(context.SessionSettings.GetTypeSetting(this.Name, type.UUID));
                    }

                    return new Exceptional<ADBSettingsBase>(new Error_CouldNotGetSetting(this, scope));
                    #endregion

                case TypesSettingScope.ATTRIBUTE:
                    #region attribute

                    if ((type != null) && (attribute != null))
                    {
                        return new Exceptional<ADBSettingsBase>(context.SessionSettings.GetAttributeSetting(this.Name, type.UUID, attribute.UUID));
                    }

                    return new Exceptional<ADBSettingsBase>(new Error_CouldNotGetSetting(this, scope, type, attribute));
                    #endregion

                default:

                    return new Exceptional<ADBSettingsBase>(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
            }
        }

        public override Exceptional<bool> Remove(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null)
        {
            switch (scope)
            {
                case TypesSettingScope.DB:
                    #region db

                    return new Exceptional<bool>(context.SessionSettings.RemoveDBSetting(this.Name));

                    #endregion

                case TypesSettingScope.SESSION:
                    #region session

                    return new Exceptional<bool>(context.SessionSettings.RemoveSessionSettingReloaded(this.Name));

                    #endregion

                case TypesSettingScope.TYPE:
                    #region type

                    if (type != null)
                    {
                        return new Exceptional<bool>(context.SessionSettings.RemoveTypeSetting(this.Name, type.UUID));
                    }

                    return new Exceptional<bool>(new Error_CouldNotRemoveSetting(this, scope));
                    #endregion

                case TypesSettingScope.ATTRIBUTE:
                    #region attribute

                    if ((type != null) && (attribute != null))
                    {
                        return new Exceptional<bool>(context.SessionSettings.RemoveAttributeSetting(this.Name, type.UUID, attribute.UUID));
                    }

                    return new Exceptional<bool>(new Error_CouldNotRemoveSetting(this, scope, type, attribute));
                    #endregion

                default:

                    return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
            }
        }
    }
}
