using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Settings;
using sones.GraphDB.QueryLanguage.Enums;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Managers.Structures.Setting
{

    public abstract class ASettingDefinition
    {

        #region Abstracts

        public abstract Exceptional<List<SelectionResultSet>> ExtractData(Dictionary<string, string> mySetting, DBContext _DBContext);
        public abstract Exceptional<List<SelectionResultSet>> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext);
        public abstract Exceptional<List<SelectionResultSet>> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext);

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

        protected DBObjectReadout CreateNewSettingReadoutOnSet(TypesSettingScope typesSettingScope, Dictionary<string, string> _Settings)
        {
            Dictionary<String, Object> payload = new Dictionary<string, object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());

            foreach (var aSetting in _Settings)
            {
                payload.Add(aSetting.Key, aSetting.Value);
            }

            return new DBObjectReadout(payload);
        }

        #endregion

    }

}
