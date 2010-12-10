#region Usings

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

#endregion

namespace sones.GraphDB.Managers.Structures.Setting
{
    /// <summary>
    /// The abstract base class for all settings
    /// </summary>
    public abstract class ASettingDefinition
    {

        #region Abstracts

        /// <summary>
        /// Extracts the settings values
        /// </summary>
        /// <param name="mySetting">The setting name</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>The extracted values</returns>
        public abstract Exceptional<IEnumerable<Vertex>> ExtractData(Dictionary<string, string> mySetting, DBContext myDBContext);

        /// <summary>
        /// Set setting values
        /// </summary>
        /// <param name="mySettingValues">The setting name and values</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns></returns>
        public abstract Exceptional<IEnumerable<Vertex>> SetData(Dictionary<string, string> mySettingValues, DBContext myDBContext);

        /// <summary>
        /// Remove setting values
        /// </summary>
        /// <param name="mySettings">The setting name and values</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns></returns>
        public abstract Exceptional<IEnumerable<Vertex>> RemoveData(Dictionary<String, String> mySettings, DBContext myDBContext);

        #endregion

        #region Protected

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mySetting"></param>
        /// <returns></returns>
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

        
        /// <summary>
        /// Converts the setting value to a ADBBaseObject
        /// </summary>
        /// <param name="mySetting">The setting</param>
        /// <param name="myValue">The value as string</param>
        /// <returns>The value as ADBBaseObject</returns>
        protected ADBBaseObject GetValueForSetting(ADBSettingsBase mySetting, String myValue)
        {

            var _TypeUUID = mySetting.Type;

            if (myValue != "DEFAULT")
                return GraphDBTypeMapper.GetADBBaseObjectFromUUID(_TypeUUID, myValue);

            else
                return GraphDBTypeMapper.GetADBBaseObjectFromUUID(_TypeUUID, mySetting.Default.Value);

        }

        
        /// <summary>
        /// Creates a new readout if a setting get a new value
        /// </summary>
        /// <param name="myTypesSettingScope">The setting scope</param>
        /// <param name="mySettings">The settings that was changed</param>
        /// <returns>The readout</returns>
        protected Vertex CreateNewSettingReadoutOnSet(TypesSettingScope myTypesSettingScope, Dictionary<String, String> mySettings)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add(DBConstants.SettingScopeAttribute, myTypesSettingScope.ToString());

            foreach (var aSetting in mySettings)
            {
                payload.Add(aSetting.Key, aSetting.Value);
            }

            return new Vertex(payload);

        }

        #endregion

    }

}
