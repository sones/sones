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

        public override Exceptional<IEnumerable<Vertex>> ExtractData(Dictionary<string, string> mySetting, DBContext context)
        {

            Dictionary<String, Object> SettingPair;

            var SettingList = new List<Vertex>();

            foreach (var typeName in _TypeNames)
            {

                var Type = context.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(typeName));
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
                        return new Exceptional<IEnumerable<Vertex>>(new Error_SettingDoesNotExist(pSetting.Key));

                    SettingPair = MakeOutputForAttribs(Setting);
                    SettingList.Add(new Vertex(SettingPair));

                }

            }

            return new Exceptional<IEnumerable<Vertex>>(SettingList);

        }

        public override Exceptional<IEnumerable<Vertex>> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {

            var resultingReadouts = new List<Vertex>();

            foreach (var typeName in _TypeNames)
            {

                var Type = _DBContext.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var pSetting in mySettingValues)
                {
                    if (_DBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                    {
                        var setSettingResult = _DBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), _DBContext, TypesSettingScope.TYPE, Type);
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

        public override Exceptional<IEnumerable<Vertex>> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext)
        {

            foreach (var typeName in _TypeNames)
            {
                var type = _DBContext.DBTypeManager.GetTypeByName(typeName);
                if (type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var Setting in mySettings)
                {
                    var removeResult = type.RemovePersistentSetting(Setting.Key.ToUpper(), _DBContext.DBTypeManager);
                    if (removeResult.Failed())
                    {
                        return new Exceptional<IEnumerable<Vertex>>(removeResult);
                    }
                }
            }

            return new Exceptional<IEnumerable<Vertex>>();

        }

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

    }

}
