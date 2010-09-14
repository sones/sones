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
using sones.GraphDB.TypeManagement;

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
