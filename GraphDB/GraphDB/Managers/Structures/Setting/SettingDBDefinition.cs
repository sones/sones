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
    /// Manipulate a DB setting
    /// </summary>
    public class SettingDBDefinition : ASettingDefinition
    {

        #region override ASettingDefinition.*

        #region extract
        
        /// <summary>
        /// Extract values from DB settings
        /// <seealso cref=" ASettingDefinition"/>
        /// </summary>
        public override Exceptional<IEnumerable<Vertex>> ExtractData(Dictionary<String, String> mySetting, DBContext myDBContext)
        {

            ADBSettingsBase Setting = null;
            var _SettingList = new List<Vertex>();
            var _Settings    = new Dictionary<String, ADBSettingsBase>(); ;

            foreach (var pSetting in mySetting)
            {

                Setting = myDBContext.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), myDBContext, TypesSettingScope.DB).Value;
                if (Setting != null && !_Settings.ContainsKey(Setting.Name))
                {
                    _Settings.Add(Setting.Name, (ADBSettingsBase)Setting.Clone());
                }


                var SettingPair = MakeOutputForAttribs(Setting);
                _SettingList.Add(new Vertex(SettingPair));

            }


            return new Exceptional<IEnumerable<Vertex>>(_SettingList);
        }

        #endregion

        #region set data
        
        /// <summary>
        /// Set values for DB settings
        /// <seealso cref=" ASettingDefinition"/>
        /// </summary>        
        public override Exceptional<IEnumerable<Vertex>> SetData(Dictionary<String, String> mySettingValues, DBContext myDBContext)
        {

            foreach (var pSetting in mySettingValues)
            {
                var setSettingResult = myDBContext.DBSettingsManager.SetSetting(pSetting.Key, GetValueForSetting(myDBContext.DBSettingsManager.AllSettingsByName[pSetting.Key], pSetting.Value), myDBContext, TypesSettingScope.DB);
                if (setSettingResult.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(setSettingResult);
                }
            }

            var resultingReadouts = new List<Vertex>();
            resultingReadouts.Add(CreateNewSettingReadoutOnSet(TypesSettingScope.DB, mySettingValues));
            return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);

        }

        #endregion

        #region remove data
        
        /// <summary>
        /// Remove settings from DB settings
        /// <seealso cref=" ASettingDefinition"/>
        /// </summary>        
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

        #endregion

    }

}
