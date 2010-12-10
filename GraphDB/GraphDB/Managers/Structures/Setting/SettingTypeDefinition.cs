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

    public class SettingTypeDefinition : ASettingDefinition
    {

        #region Fields

        /// <summary>
        /// The settings for the type
        /// </summary>
        private Dictionary<string, List<ADBSettingsBase>> _SettingTypeAttr = new Dictionary<string, List<ADBSettingsBase>>();

        /// <summary>
        /// Type names
        /// </summary>
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

        #region extract
        
        /// <summary>
        /// Extract values from type
        /// <seealso cref=" ASettingDefinition"/>
        /// </summary>
        public override Exceptional<IEnumerable<Vertex>> ExtractData(Dictionary<string, string> mySetting, DBContext myDBContext)
        {

            Dictionary<String, Object> SettingPair;

            var SettingList = new List<Vertex>();

            foreach (var typeName in _TypeNames)
            {

                var Type = myDBContext.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var pSetting in mySetting)
                {

                    ADBSettingsBase Setting = myDBContext.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), myDBContext, TypesSettingScope.TYPE, Type).Value;
                    if (Setting != null)
                    {
                        if (!_SettingTypeAttr.ContainsKey(Type.Name))
                            _SettingTypeAttr.Add(Type.Name, new List<ADBSettingsBase>() { Setting });
                        else
                            _SettingTypeAttr[Type.Name].Add(Setting);
                    }
                    else
                        return new Exceptional<IEnumerable<Vertex>>(new Error_SettingDoesNotExist(pSetting.Key));

                    SettingPair = MakeOutputForAttribs(Setting);
                    SettingList.Add(new Vertex(SettingPair));

                }

            }

            return new Exceptional<IEnumerable<Vertex>>(SettingList);

        }

        #endregion


        #region set
        
        /// <summary>
        /// Set values for type settings
        /// <seealso cref=" ASettingDefinition"/>
        /// </summary>
        public override Exceptional<IEnumerable<Vertex>> SetData(Dictionary<string, string> mySettingValues, DBContext myDBContext)
        {

            var resultingReadouts = new List<Vertex>();

            foreach (var typeName in _TypeNames)
            {

                var Type = myDBContext.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var pSetting in mySettingValues)
                {
                    if (myDBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                    {
                        var setSettingResult = myDBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(myDBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), myDBContext, TypesSettingScope.TYPE, Type);
                        if (setSettingResult.Failed())
                        {
                            return new Exceptional<IEnumerable<Vertex>>(setSettingResult);
                        }
                    }
                    else
                    {
                        return new Exceptional<IEnumerable<Vertex>>(new Error_SettingDoesNotExist(pSetting.Key));
                    }
                }
            }

            foreach (var aType in _TypeNames)
            {
                resultingReadouts.Add(CreateNewTYPESettingReadoutOnSet(TypesSettingScope.TYPE, aType, mySettingValues));
            }

            return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);

        }

        #endregion

        #region remove
        
        /// <summary>
        /// Remove values from type settings
        /// <seealso cref=" ASettingDefinition"/>
        /// </summary>
        public override Exceptional<IEnumerable<Vertex>> RemoveData(Dictionary<String, String> mySettings, DBContext myDBContext)
        {

            foreach (var typeName in _TypeNames)
            {
                var type = myDBContext.DBTypeManager.GetTypeByName(typeName);
                if (type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var Setting in mySettings)
                {
                    var removeResult = type.RemovePersistentSetting(Setting.Key.ToUpper(), myDBContext.DBTypeManager);
                    if (removeResult.Failed())
                    {
                        return new Exceptional<IEnumerable<Vertex>>(removeResult);
                    }
                }
            }

            return new Exceptional<IEnumerable<Vertex>>();

        }

        #endregion

        #region private helper
        
        private Vertex CreateNewTYPESettingReadoutOnSet(TypesSettingScope typesSettingScope, String aType, Dictionary<String, String> _Settings)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());
            payload.Add(typesSettingScope.ToString(), aType);

            foreach (var aSetting in _Settings)
            {
                payload.Add(aSetting.Key, aSetting.Value);
            }

            return new Vertex(payload);

        }

        #endregion

        #endregion

    }

}
