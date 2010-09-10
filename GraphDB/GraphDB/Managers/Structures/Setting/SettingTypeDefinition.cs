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

    public class SettingTypeDefinition : ASettingDefinition
    {

        #region Fields

        private Dictionary<string, List<ADBSettingsBase>> _SettingTypeAttr = new Dictionary<string, List<ADBSettingsBase>>();
        private List<string> _TypeNames;

        #endregion

        #region Ctor

        public SettingTypeDefinition(List<string> myTypeNames)
        {
            System.Diagnostics.Debug.Assert(myTypeNames != null);
            _TypeNames = myTypeNames;
        }

        #endregion

        #region override ASettingDefinition.*

        public override Exceptional<SelectionResultSet> ExtractData(Dictionary<string, string> mySetting, DBContext context)
        {
            Dictionary<String, Object> SettingPair;

            var SettingList = new List<DBObjectReadout>();

            foreach (var typeName in _TypeNames)
            {

                var Type = context.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<SelectionResultSet>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var pSetting in mySetting)
                {
                    ADBSettingsBase Setting = context.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), context, TypesSettingScope.TYPE, Type).Value;
                    if (Setting != null)
                    {
                        if (!_SettingTypeAttr.ContainsKey(Type.Name))
                            _SettingTypeAttr.Add(Type.Name, new List<ADBSettingsBase>() { Setting });
                        else
                            _SettingTypeAttr[Type.Name].Add(Setting);
                    }
                    else
                        return new Exceptional<SelectionResultSet>(new Error_SettingDoesNotExist(pSetting.Key));

                    SettingPair = MakeOutputForAttribs(Setting);
                    SettingList.Add(new DBObjectReadout(SettingPair));
                }
            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(SettingList));
        }

        public override Exceptional<SelectionResultSet> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {

            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();

            foreach (var typeName in _TypeNames)
            {

                var Type = _DBContext.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<SelectionResultSet>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var pSetting in mySettingValues)
                {
                    if (_DBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                    {
                        var setSettingResult = _DBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), _DBContext, TypesSettingScope.TYPE, Type);
                        if (setSettingResult.Failed())
                        {
                            return new Exceptional<SelectionResultSet>(setSettingResult);
                        }
                    }
                    else
                    {
                        return new Exceptional<SelectionResultSet>(new Error_SettingDoesNotExist(pSetting.Key));
                    }
                }
            }

            foreach (var aType in _TypeNames)
            {
                resultingReadouts.Add(CreateNewTYPESettingReadoutOnSet(TypesSettingScope.TYPE, aType, mySettingValues));
            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(resultingReadouts) );

        }

        public override Exceptional<SelectionResultSet> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext)
        {
            foreach (var typeName in _TypeNames)
            {
                var type = _DBContext.DBTypeManager.GetTypeByName(typeName);
                if (type == null)
                {
                    return new Exceptional<SelectionResultSet>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var Setting in mySettings)
                {
                    var removeResult = type.RemovePersistentSetting(Setting.Key.ToUpper(), _DBContext.DBTypeManager);
                    if (removeResult.Failed())
                    {
                        return new Exceptional<SelectionResultSet>(removeResult);
                    }
                }
            }

            return new Exceptional<SelectionResultSet>();

        }

        private DBObjectReadout CreateNewTYPESettingReadoutOnSet(TypesSettingScope typesSettingScope, String aType, Dictionary<string, string> _Settings)
        {
            Dictionary<String, Object> payload = new Dictionary<string, object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());
            payload.Add(typesSettingScope.ToString(), aType);

            foreach (var aSetting in _Settings)
            {
                payload.Add(aSetting.Key, aSetting.Value);
            }

            return new DBObjectReadout(payload);
        }

        #endregion

    }

}
