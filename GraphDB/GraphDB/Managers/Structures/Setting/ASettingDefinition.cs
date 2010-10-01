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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;


namespace sones.GraphDB.Managers.Structures.Setting
{

    public abstract class ASettingDefinition
    {

        #region Abstracts

        public abstract Exceptional<IEnumerable<Vertex>> ExtractData(Dictionary<string, string> mySetting, DBContext _DBContext);
        public abstract Exceptional<IEnumerable<Vertex>> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext);
        public abstract Exceptional<IEnumerable<Vertex>> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext);

        #endregion

        #region Protected

        protected Dictionary<String, Object> MakeOutputForAttribs(ADBSettingsBase mySetting)
        {
            var settingList = new Dictionary<String, Object>();
            settingList.Add("Name", mySetting.Name);
            settingList.Add("ID", mySetting.ID);
            settingList.Add("Type", mySetting.Type);
            settingList.Add("Value", mySetting.Value);
            settingList.Add("Default", mySetting.Default);

            return settingList;
        }

        protected ADBBaseObject GetValueForSetting(ADBSettingsBase mySetting, String myValue)
        {

            var _TypeUUID = mySetting.Type;

            if (myValue != "DEFAULT")
                return GraphDBTypeMapper.GetADBBaseObjectFromUUID(_TypeUUID, myValue);

            else
                return GraphDBTypeMapper.GetADBBaseObjectFromUUID(_TypeUUID, mySetting.Default.Value);

        }

        protected Vertex CreateNewSettingReadoutOnSet(TypesSettingScope typesSettingScope, Dictionary<String, String> _Settings)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());

            foreach (var aSetting in _Settings)
            {
                payload.Add(aSetting.Key, aSetting.Value);
            }

            return new Vertex(payload);

        }

        #endregion

    }

}
