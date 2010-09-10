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

/* <id name="GraphDB – Settings" />
 * <copyright file="SelectSettingCache.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */


#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Settings;
using sones.GraphDB.TypeManagement;

using sones.GraphFS.Session;
using sones.GraphFS.Session;
using sones.Lib.DataStructures.UUID;
using sones.GraphDB.Structures.Enums;
using sones.GraphDBInterface.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Select
{
    
    public class SelectSettingCache
    {
        #region Data
        private Dictionary<TypeUUID, Dictionary<AttributeUUID, Dictionary<UUID, ADBBaseObject>>> _TypeAttrSetting;
        #endregion

        #region constructor
        public SelectSettingCache()
        {
            _TypeAttrSetting = new Dictionary<TypeUUID, Dictionary<AttributeUUID, Dictionary<UUID, ADBBaseObject>>>();
        }
        #endregion

        public ADBBaseObject GetValue(GraphDBType myTypeID, TypeAttribute myAttrID, UUID mySettingUUID, DBContext context)
        {
            if (_TypeAttrSetting.ContainsKey(myTypeID.UUID))
            {
                if (_TypeAttrSetting[myTypeID.UUID].ContainsKey(myAttrID.UUID))
                {
                    if (_TypeAttrSetting[myTypeID.UUID][myAttrID.UUID].ContainsKey(mySettingUUID))
                    {
                        return _TypeAttrSetting[myTypeID.UUID][myAttrID.UUID][mySettingUUID];
                    }
                }
                else
                {
                    _TypeAttrSetting[myTypeID.UUID].Add(myAttrID.UUID, new Dictionary<UUID, ADBBaseObject>());
                }
            }
            else
            {
                _TypeAttrSetting.Add(myTypeID.UUID, new Dictionary<AttributeUUID, Dictionary<UUID, ADBBaseObject>>());
                _TypeAttrSetting[myTypeID.UUID].Add(myAttrID.UUID, new Dictionary<UUID, ADBBaseObject>());
            }

            //we are here, so we have to add the setting and return it

            var settingValue = context.DBSettingsManager.GetSettingValue(mySettingUUID, context, TypesSettingScope.ATTRIBUTE, myTypeID, myAttrID).Value.Clone();

            _TypeAttrSetting[myTypeID.UUID][myAttrID.UUID].Add(mySettingUUID, settingValue);

            return settingValue;
        }

    }
}
