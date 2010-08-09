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

    public class SettingAttributeDefinition : ASettingDefinition
    {

        #region Fields

        private Dictionary<string, List<ADBSettingsBase>> _SettingTypeAttr = new Dictionary<string, List<ADBSettingsBase>>();
        private Dictionary<String, List<IDChainDefinition>> _Attributes;

        #endregion

        #region Ctor

        public SettingAttributeDefinition(Dictionary<String, List<IDChainDefinition>> myAttributes)
        {
            System.Diagnostics.Debug.Assert(myAttributes != null);
            _Attributes = myAttributes;
        }

        #endregion

        #region override ASettingDefinition.*

        public override Exceptional<List<SelectionResultSet>> ExtractData(Dictionary<string, string> mySettingName, DBContext context)
        {
            List<SelectionResultSet> result = new List<SelectionResultSet>();
            Dictionary<String, Object> SettingPair;

            foreach (var keyValPair in _Attributes)
            {

                var type = context.DBTypeManager.GetTypeByName(keyValPair.Key);
                if (type == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(keyValPair.Key));
                }

                foreach (var idChain in keyValPair.Value)
                {

                    var validateResult = idChain.Validate(context, false, type);
                    if (validateResult.Failed)
                    {
                        return new Exceptional<List<SelectionResultSet>>(validateResult);
                    }

                    var SettingList = new List<DBObjectReadout>();
                    var Entry = idChain.LastAttribute;

                    foreach (var pSetting in mySettingName)
                    {
                        ADBSettingsBase Setting = context.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), context, TypesSettingScope.ATTRIBUTE, Entry.GetRelatedType(context.DBTypeManager), Entry).Value;
                        if (Setting != null)
                        {
                            if (!_SettingTypeAttr.ContainsKey(Entry.Name))
                                _SettingTypeAttr.Add(Entry.Name, new List<ADBSettingsBase>() { Setting });
                            else
                                _SettingTypeAttr[Entry.Name].Add(Setting);
                        }
                        else
                        {
                            return new Exceptional<List<SelectionResultSet>>(new Error_SettingDoesNotExist(pSetting.Key));
                        }

                        SettingPair = MakeOutputForAttribs(Setting);
                        SettingList.Add(new DBObjectReadout(SettingPair));
                    }
                    result.Add(new SelectionResultSet(SettingList));

                }
            }

            return new Exceptional<List<SelectionResultSet>>(result);
        }

        public override Exceptional<List<SelectionResultSet>> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {

            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();
            Dictionary<GraphDBType, List<TypeAttribute>> settingTypeAndAttributes = new Dictionary<GraphDBType, List<TypeAttribute>>();

            foreach (var keyValPair in _Attributes)
            {

                var type = _DBContext.DBTypeManager.GetTypeByName(keyValPair.Key);
                if (type == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(keyValPair.Key));
                }

                foreach (var idChain in keyValPair.Value)
                {

                    var validateResult = idChain.Validate(_DBContext, false, type);
                    if (validateResult.Failed)
                    {
                        return new Exceptional<List<SelectionResultSet>>(validateResult);
                    }

                    foreach (var pSetting in mySettingValues)
                    {
                        if (_DBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                        {
                            var setSettingResult = _DBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), _DBContext, TypesSettingScope.ATTRIBUTE, type, idChain.LastAttribute);
                            if (setSettingResult.Failed)
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
                resultingReadouts.Add(CreateNewATTRIBUTESettingReadoutOnSet(TypesSettingScope.ATTRIBUTE, type, keyValPair.Value, mySettingValues));

            }

            return new Exceptional<List<SelectionResultSet>>(new List<SelectionResultSet>() { new SelectionResultSet(resultingReadouts) });

        }

        public override Exceptional<List<SelectionResultSet>> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext)
        {

            foreach (var keyValPair in _Attributes)
            {

                var graphDBType = _DBContext.DBTypeManager.GetTypeByName(keyValPair.Key);
                if (graphDBType == null)
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(keyValPair.Key));
                }

                foreach (var idChain in keyValPair.Value)
                {

                    Exceptional validateResult = idChain.Validate(_DBContext, false);
                    if (validateResult.Failed)
                    {
                        return new Exceptional<List<SelectionResultSet>>(validateResult);
                    }

                    foreach (var Setting in mySettings)
                    {
                        var removeResult = idChain.LastAttribute.RemovePersistentSetting(Setting.Key.ToUpper(), _DBContext.DBTypeManager);
                        if (removeResult.Failed)
                        {
                            return new Exceptional<List<SelectionResultSet>>(removeResult);
                        }
                    }

                }

            }

            return new Exceptional<List<SelectionResultSet>>();

        }

        private DBObjectReadout CreateNewATTRIBUTESettingReadoutOnSet(TypesSettingScope typesSettingScope, GraphDBType myDBType, List<IDChainDefinition> myAttributes, Dictionary<string, string> _Settings)
        {
            Dictionary<String, Object> payload = new Dictionary<string, object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());
            payload.Add(TypesSettingScope.TYPE.ToString(), myDBType.Name);

            List<DBObjectReadout> attributes = new List<DBObjectReadout>();

            foreach (var aAttribute in myAttributes)
            {
                Dictionary<String, Object> innerPayload = new Dictionary<string, object>();

                innerPayload.Add(TypesSettingScope.ATTRIBUTE.ToString(), aAttribute.LastAttribute.Name);

                foreach (var aSetting in _Settings)
                {
                    innerPayload.Add(aSetting.Key, aSetting.Value);
                }

                attributes.Add(new DBObjectReadout(innerPayload));
            }

            payload.Add(DBConstants.SettingAttributesAttribute, new Edge(attributes, DBConstants.SettingAttributesAttributeTYPE));

            return new DBObjectReadout(payload);
        }

        #endregion

    }

}
