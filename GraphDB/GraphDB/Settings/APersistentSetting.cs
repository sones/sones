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

/* <id name="GraphDB – APersistentSetting" />
 * <copyright file="APersistentSetting.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings


using System;

using sones.GraphFS.DataStructures;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;


#endregion

namespace sones.GraphDB.Settings
{

    public abstract class APersistentSetting: ADBSettingsBase
	{

        public APersistentSetting()
        {
        }

        public APersistentSetting(String myName, String myDesc, EntityUUID myOwner, UUID myType, ADBBaseObject myDefault)
        {
        }
        
        public APersistentSetting(Byte[] mySerializedData)
            : base(mySerializedData)
        {
        }

        public APersistentSetting(APersistentSetting myCopy)
            : base(myCopy)
        {            
        }

        public override Exceptional Set(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null)
        {

            switch (scope)
            {
                case TypesSettingScope.DB:
                    #region db

                    return context.DBSettingsManager.SetPersistentDBSetting(context, this);

                    #endregion

                case TypesSettingScope.SESSION:
                    #region session

                    return context.SessionSettings.SetSessionSetting(this);

                    #endregion

                case TypesSettingScope.TYPE:
                    #region type

                    if (type != null)
                    {
                        return type.SetPersistentSetting(this.Name, this, context.DBTypeManager);
                    }

                    return new Exceptional(new Error_CouldNotSetSetting(this, scope));

                    #endregion

                case TypesSettingScope.ATTRIBUTE:
                    #region attribute

                    if ((type != null) && (attribute != null))
                    {
                        return attribute.SetPersistentSetting(this.Name, this, context.DBTypeManager);
                    }

                    return new Exceptional(new Error_CouldNotSetSetting(this, scope, type, attribute));
                    #endregion

                default:

                    return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
            }
        }

        public override Exceptional<ADBSettingsBase> Get(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null)
        {
            switch (scope)
            {
                case TypesSettingScope.DB:
                    #region db

                    return new Exceptional<ADBSettingsBase>(context.DBSettingsManager.GetPersistentDBSetting(this.Name).Value);

                    #endregion

                case TypesSettingScope.SESSION:
                    #region session

                    return GetSessionSetting(context);

                    #endregion

                case TypesSettingScope.TYPE:
                    #region type

                    if (type != null)
                    {
                        return GetPersistentTypeSetting(context, type);
                    }

                    return new Exceptional<ADBSettingsBase>(new Error_CouldNotGetSetting(this, scope));
                    #endregion

                case TypesSettingScope.ATTRIBUTE:
                    #region attribute

                    if ((type != null) && (attribute != null))
                    {
                        return GetPersistentAttributeSetting(context, type, attribute);
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

                    return new Exceptional<bool>(context.DBSettingsManager.RemovePersistentDBSetting(context, this.Name));

                    #endregion

                case TypesSettingScope.SESSION:
                    #region session

                    return new Exceptional<bool>(context.SessionSettings.RemoveSessionSettingReloaded(this.Name));

                    #endregion

                case TypesSettingScope.TYPE:
                    #region type

                    if (type != null)
                    {
                        return new Exceptional<bool>(type.RemovePersistentSetting(this.Name, context.DBTypeManager));
                    }

                    return new Exceptional<bool>(new Error_CouldNotRemoveSetting(this, scope));
                    #endregion

                case TypesSettingScope.ATTRIBUTE:
                    #region attribute

                    if ((type != null) && (attribute != null))
                    {
                        return new Exceptional<bool>(attribute.RemovePersistentSetting(this.Name, context.DBTypeManager));
                    }

                    return new Exceptional<bool>(new Error_CouldNotRemoveSetting(this, scope, type, attribute));
                    #endregion

                default:

                    return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
            }
        }


        #region private helper


        /// <summary>
        /// Gets a persistent setting for a given session. If there was no direct hit, the database settings are consulted.
        /// </summary>
        /// <param name="context">The context of the database</param>
        /// <returns>A setting</returns>
        private Exceptional<ADBSettingsBase> GetSessionSetting(DBContext context)
        {
            var sessionSetting = context.SessionSettings.GetSessionSetting(this.Name, false);

            if (sessionSetting != null)
            {
                return new Exceptional<ADBSettingsBase>(sessionSetting);
            }
            else
            {
                return new Exceptional<ADBSettingsBase>(context.DBSettingsManager.GetPersistentDBSetting(this.Name));
            }
        }

        /// <summary>
        /// Gets a persistent attribute setting. If there was no direct hit, the type, session and database settings are consulted.
        /// </summary>
        /// <param name="context">The context of the database</param>
        /// <param name="type">The related type of the attribute</param>
        /// <param name="attribute">The attribute</param>
        /// <returns>A setting</returns>
        private Exceptional<ADBSettingsBase> GetPersistentAttributeSetting(DBContext context, GraphDBType type, TypeAttribute attribute)
        {
            var attributeSetting = attribute.GetPersistentSetting(this.Name);

            if (attributeSetting != null)
            {
                return new Exceptional<ADBSettingsBase>(attributeSetting);
            }
            else
            {
                return GetPersistentTypeSetting(context, type);
            }
        }

        /// <summary>
        /// Gets a persistent type setting. If there was no direct hit, session and database settings are consulted.
        /// </summary>
        /// <param name="context">The context of the database</param>
        /// <param name="type">The type</param>
        /// <returns>A setting</returns>
        private Exceptional<ADBSettingsBase> GetPersistentTypeSetting(DBContext context, GraphDBType type)
        {
            var typesetting = type.GetPersisitentSetting(this.Name);

            if (typesetting != null)
            {
                return new Exceptional<ADBSettingsBase>(typesetting);
            }
            else
            {
                return GetSessionSetting(context);
            }
        }

        #endregion
    
    }

}
