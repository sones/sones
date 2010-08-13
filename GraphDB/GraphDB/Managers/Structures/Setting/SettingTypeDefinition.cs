using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;

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

        public override Exceptional<List<SelectionResultSet>> ExtractData(Dictionary<string, string> mySetting, DBContext context)
        {
            List<SelectionResultSet> result = new List<SelectionResultSet>();
            Dictionary<String, Object> SettingPair;

            foreach (var typeName in _TypeNames)
            {

                var Type = context.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(typeName));
                }

                var SettingList = new List<DBObjectReadout>();

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
                        return new Exceptional<List<SelectionResultSet>>(new Error_SettingDoesNotExist(pSetting.Key));

                    SettingPair = MakeOutputForAttribs(Setting);
                    SettingList.Add(new DBObjectReadout(SettingPair));
                }
                result.Add(new SelectionResultSet(SettingList));
            }

            return new Exceptional<List<SelectionResultSet>>(result);
        }

        public override Exceptional<List<SelectionResultSet>> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {

            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();

            foreach (var typeName in _TypeNames)
            {

                var Type = _DBContext.DBTypeManager.GetTypeByName(typeName);
                if (Type == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var pSetting in mySettingValues)
                {
                    if (_DBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                    {
                        var setSettingResult = _DBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), _DBContext, TypesSettingScope.TYPE, Type);
                        if (setSettingResult.Failed())
                        {
                            return new Exceptional<List<SelectionResultSet>>(setSettingResult);
                        }
                    }
                    else
                    {
                        return new Exceptional<List<SelectionResultSet>>(new Error_SettingDoesNotExist(pSetting.Key));
                    }
                }
            }

            foreach (var aType in _TypeNames)
            {
                resultingReadouts.Add(CreateNewTYPESettingReadoutOnSet(TypesSettingScope.TYPE, aType, mySettingValues));
            }

            return new Exceptional<List<SelectionResultSet>>(new List<SelectionResultSet>() { new SelectionResultSet(resultingReadouts) });

        }

        public override Exceptional<List<SelectionResultSet>> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext)
        {
            foreach (var typeName in _TypeNames)
            {
                var type = _DBContext.DBTypeManager.GetTypeByName(typeName);
                if (type == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(typeName));
                }

                foreach (var Setting in mySettings)
                {
                    var removeResult = type.RemovePersistentSetting(Setting.Key.ToUpper(), _DBContext.DBTypeManager);
                    if (removeResult.Failed())
                    {
                        return new Exceptional<List<SelectionResultSet>>(removeResult);
                    }
                }
            }

            return new Exceptional<List<SelectionResultSet>>();

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
