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

        public override Exceptional<IEnumerable<Vertex>> ExtractData(Dictionary<string, string> mySettingName, DBContext context)
        {
            Dictionary<String, Object> SettingPair;
            var SettingList = new List<Vertex>();



            foreach (var keyValPair in _Attributes)
            {
                var type = context.DBTypeManager.GetTypeByName(keyValPair.Key);
                if (type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(keyValPair.Key));
                }

                foreach (var idChain in keyValPair.Value)
                {

                    var validateResult = idChain.Validate(context, false, type);
                    if (validateResult.Failed())
                    {
                        return new Exceptional<IEnumerable<Vertex>>(validateResult);
                    }

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
                            return new Exceptional<IEnumerable<Vertex>>(new Error_SettingDoesNotExist(pSetting.Key));
                        }

                        SettingPair = MakeOutputForAttribs(Setting);
                        SettingList.Add(new Vertex(SettingPair));
                    }

                }
            }

            return new Exceptional<IEnumerable<Vertex>>(SettingList);
        }

        public override Exceptional<IEnumerable<Vertex>> SetData(Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {

            List<Vertex> resultingReadouts = new List<Vertex>();
            Dictionary<GraphDBType, List<TypeAttribute>> settingTypeAndAttributes = new Dictionary<GraphDBType, List<TypeAttribute>>();

            foreach (var keyValPair in _Attributes)
            {

                var type = _DBContext.DBTypeManager.GetTypeByName(keyValPair.Key);
                if (type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(keyValPair.Key));
                }

                foreach (var idChain in keyValPair.Value)
                {

                    var validateResult = idChain.Validate(_DBContext, false, type);
                    if (validateResult.Failed())
                    {
                        return new Exceptional<IEnumerable<Vertex>>(validateResult);
                    }

                    foreach (var pSetting in mySettingValues)
                    {
                        if (_DBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                        {
                            var setSettingResult = _DBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), _DBContext, TypesSettingScope.ATTRIBUTE, type, idChain.LastAttribute);
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
                resultingReadouts.Add(CreateNewATTRIBUTESettingReadoutOnSet(TypesSettingScope.ATTRIBUTE, type, keyValPair.Value, mySettingValues));

            }

            return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);

        }

        public override Exceptional<IEnumerable<Vertex>> RemoveData(Dictionary<String, String> mySettings, DBContext _DBContext)
        {

            foreach (var keyValPair in _Attributes)
            {

                var graphDBType = _DBContext.DBTypeManager.GetTypeByName(keyValPair.Key);
                if (graphDBType == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(keyValPair.Key));
                }

                foreach (var idChain in keyValPair.Value)
                {

                    Exceptional validateResult = idChain.Validate(_DBContext, false);
                    if (validateResult.Failed())
                    {
                        return new Exceptional<IEnumerable<Vertex>>(validateResult);
                    }

                    foreach (var Setting in mySettings)
                    {
                        var removeResult = idChain.LastAttribute.RemovePersistentSetting(Setting.Key.ToUpper(), _DBContext.DBTypeManager);
                        if (removeResult.Failed())
                        {
                            return new Exceptional<IEnumerable<Vertex>>(removeResult);
                        }
                    }

                }

            }

            return new Exceptional<IEnumerable<Vertex>>();

        }

        private Vertex CreateNewATTRIBUTESettingReadoutOnSet(TypesSettingScope myTypesSettingScope, GraphDBType myDBType, List<IDChainDefinition> myAttributes, Dictionary<String, String> mySettings)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add(DBConstants.SettingScopeAttribute, myTypesSettingScope.ToString());
            payload.Add(TypesSettingScope.TYPE.ToString(), myDBType.Name);

            var attributes = new List<Vertex>();

            foreach (var _Attribute in myAttributes)
            {

                var innerPayload = new Dictionary<String, Object>();

                innerPayload.Add(TypesSettingScope.ATTRIBUTE.ToString(), _Attribute.LastAttribute.Name);

                foreach (var aSetting in mySettings)
                {
                    innerPayload.Add(aSetting.Key, aSetting.Value);
                }

                attributes.Add(new Vertex(innerPayload));
            }

            payload.Add(DBConstants.SettingAttributesAttribute, new Edge(null, attributes) { EdgeTypeName = DBConstants.SettingAttributesAttributeTYPE });

            return new Vertex(payload);

        }

        #endregion

    }

}
