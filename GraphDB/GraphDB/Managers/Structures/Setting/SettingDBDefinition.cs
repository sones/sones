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
using sones.GraphDBInterface.Result;

namespace sones.GraphDB.Managers.Structures.Setting
{

    public class SettingDBDefinition : ASettingDefinition
    {

        #region override ASettingDefinition.*

        public override Exceptional<SelectionResultSet> ExtractData(Dictionary<string, string> mySetting, DBContext _DBContext)
        {
            ADBSettingsBase Setting = null;
            List<DBObjectReadout> SettingList = new List<DBObjectReadout>();
            Dictionary<string, ADBSettingsBase> _Settings = new Dictionary<string, ADBSettingsBase>(); ;

            foreach (var pSetting in mySetting)
            {

                Setting = _DBContext.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), _DBContext, TypesSettingScope.DB).Value;
                if (Setting != null && !_Settings.ContainsKey(Setting.Name))
                {
                    _Settings.Add(Setting.Name, (ADBSettingsBase)Setting.Clone());
                }


                var SettingPair = MakeOutputForAttribs(Setting);
                SettingList.Add(new DBObjectReadout(SettingPair));
            }


            return new Exceptional<SelectionResultSet>(new SelectionResultSet(SettingList));
        }

        public override Exceptional<SelectionResultSet> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {

            foreach (var pSetting in mySettingValues)
            {
                var setSettingResult = _DBContext.DBSettingsManager.SetSetting(pSetting.Key, GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key], pSetting.Value), _DBContext, TypesSettingScope.DB);
                if (setSettingResult.Failed())
                {
                    return new Exceptional<SelectionResultSet>(setSettingResult);
                }
            }

            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();
            resultingReadouts.Add(CreateNewSettingReadoutOnSet(TypesSettingScope.DB, mySettingValues));
            return new Exceptional<SelectionResultSet>(new SelectionResultSet(resultingReadouts));

        }

        public override Exceptional<SelectionResultSet> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext)
        {
            foreach (var Setting in mySettings)
            {

                var removeResult = _DBContext.DBSettingsManager.RemoveSetting(_DBContext, Setting.Key.ToUpper(), TypesSettingScope.DB);
                if (removeResult.Failed())
                {
                    return new Exceptional<SelectionResultSet>(removeResult);
                }
            }

            return new Exceptional<SelectionResultSet>();

        }

        #endregion

    }

}
