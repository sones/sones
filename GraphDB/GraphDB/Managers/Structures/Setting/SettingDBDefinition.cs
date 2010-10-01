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

    public class SettingDBDefinition : ASettingDefinition
    {

        #region override ASettingDefinition.*

        public override Exceptional<IEnumerable<Vertex>> ExtractData(Dictionary<String, String> mySetting, DBContext _DBContext)
        {

            ADBSettingsBase Setting = null;
            var _SettingList = new List<Vertex>();
            var _Settings    = new Dictionary<String, ADBSettingsBase>(); ;

            foreach (var pSetting in mySetting)
            {

                Setting = _DBContext.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), _DBContext, TypesSettingScope.DB).Value;
                if (Setting != null && !_Settings.ContainsKey(Setting.Name))
                {
                    _Settings.Add(Setting.Name, (ADBSettingsBase)Setting.Clone());
                }


                var SettingPair = MakeOutputForAttribs(Setting);
                _SettingList.Add(new Vertex(SettingPair));

            }


            return new Exceptional<IEnumerable<Vertex>>(_SettingList);
        }

        public override Exceptional<IEnumerable<Vertex>> SetData(Dictionary<String, String> mySettingValues, DBContext _DBContext)
        {

            foreach (var pSetting in mySettingValues)
            {
                var setSettingResult = _DBContext.DBSettingsManager.SetSetting(pSetting.Key, GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key], pSetting.Value), _DBContext, TypesSettingScope.DB);
                if (setSettingResult.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(setSettingResult);
                }
            }

            var resultingReadouts = new List<Vertex>();
            resultingReadouts.Add(CreateNewSettingReadoutOnSet(TypesSettingScope.DB, mySettingValues));
            return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);

        }

        public override Exceptional<IEnumerable<Vertex>> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext)
        {

            foreach (var Setting in mySettings)
            {

                var removeResult = _DBContext.DBSettingsManager.RemoveSetting(_DBContext, Setting.Key.ToUpper(), TypesSettingScope.DB);
                if (removeResult.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(removeResult);
                }
            }

            return new Exceptional<IEnumerable<Vertex>>();

        }

        #endregion

    }

}
