/*
 * DescribeSettingDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Indices;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;
using sones.Lib.Session;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{

    public class DescribeSettingDefinition : ADescribeDefinition
    {

        #region Data

        private TypesSettingScope? _SettingType;
        private String _SettingName;
        private String _SettingTypeName;
        private IDChainDefinition _SettingAttribute;

        #endregion

        #region Ctor

        public DescribeSettingDefinition() { }

        public DescribeSettingDefinition(TypesSettingScope? mySettingType, String mySettingName = null, String myTypeName = null, IDChainDefinition myIDChain = null)
        {
            _SettingType = mySettingType;
            _SettingName = mySettingName;
            _SettingTypeName = myTypeName;
            _SettingAttribute = myIDChain;
        }

        #endregion

        #region ADescribeDefinition

        public override Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext)
        {
            var result = new List<SelectionResultSet>();
            Exceptional<SelectionResultSet> outputResult = null;

            if (_SettingType.HasValue)
            {

                #region Special _SettingType

                switch (_SettingType.Value)
                {
                    case TypesSettingScope.TYPE:

                        #region TYPE

                        var type = myDBContext.DBTypeManager.GetTypeByName(_SettingTypeName);
                        if (type == null)
                        {
                            return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(_SettingTypeName));
                        }
                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            result.AddRange(GenerateTypeResult(type, myDBContext));
                        }
                        else
                        {
                            var descrTypeRes = GenerateTypeResult(_SettingName, type, myDBContext);
                            if (descrTypeRes.Failed())
                            {
                                return new Exceptional<List<SelectionResultSet>>(descrTypeRes);
                            }
                            result.Add(descrTypeRes.Value);
                        }
                        break;

                        #endregion

                    case TypesSettingScope.ATTRIBUTE:

                        #region ATTRIBUTE

                        Exceptional validateResult = _SettingAttribute.Validate(myDBContext, false);
                        if (validateResult.Failed())
                        {
                            return new Exceptional<List<SelectionResultSet>>(validateResult.Errors);
                        }

                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            result.AddRange(GenerateAttrResult(_SettingAttribute.LastAttribute, myDBContext));
                        }
                        else
                        {
                            outputResult = GenerateAttrResult(_SettingName, _SettingAttribute.LastAttribute, myDBContext);

                            if (outputResult.Failed())
                            {
                                return new Exceptional<List<SelectionResultSet>>(outputResult);
                            }
                            result.Add(outputResult.Value);
                        }
                        break;

                        #endregion

                    case TypesSettingScope.SESSION:

                        #region SESSION

                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            result.AddRange(GenerateSessionResult(myDBContext));
                        }
                        else
                        {
                            outputResult = GenerateSessionResult(_SettingName, myDBContext);

                            if (outputResult.Failed())
                            {
                                return new Exceptional<List<SelectionResultSet>>(outputResult);
                            }
                            result.Add(outputResult.Value);
                        }
                        break;

                        #endregion

                    case TypesSettingScope.DB:

                        #region DB

                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            result.AddRange(GenerateDBResult(myDBContext));
                        }
                        else
                        {
                            outputResult = GenerateDBResult(_SettingName, myDBContext);
                            if (outputResult.Failed())
                            {
                                return new Exceptional<List<SelectionResultSet>>(outputResult);
                            }
                            result.Add(outputResult.Value);
                        }

                        #endregion

                        break;
                    default:

                        return new Exceptional<List<SelectionResultSet>>(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
                }

                #endregion

            }
            else
            {

                #region No SettingType

                if (String.IsNullOrEmpty(_SettingName))
                {

                    #region Describe all settings

                    var readOutList = new List<DBObjectReadout>();
                    foreach (var Item in myDBContext.DBSettingsManager.GetAllSettings())
                    {
                        readOutList.Add(GenerateResult(Item, myDBContext.DBTypeManager));
                    }
                    result.Add(new SelectionResultSet(readOutList));

                    #endregion

                }
                else
                {

                    #region Describe named setting

                    outputResult = GenerateStdResult(_SettingName, myDBContext);

                    if (outputResult.Failed())
                    {
                        return new Exceptional<List<SelectionResultSet>>(outputResult);
                    }
                    result.Add(outputResult.Value);

                    #endregion

                }

                #endregion

            }

            return new Exceptional<List<SelectionResultSet>>(result);

        }
        
        #endregion

        #region Output

        #region Standard output value

        /// <summary>
        /// generate a output result if no database, session, type or attribute is requested, then you can get information about the setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="myDBContext"></param>
        private Exceptional<SelectionResultSet> GenerateStdResult(string mySettingName, DBContext myDBContext)
        {

            var settingResult = myDBContext.DBSettingsManager.GetSetting(mySettingName);
            if (settingResult.Failed())
            {
                return new Exceptional<SelectionResultSet>(settingResult);
            }
            else
            {
                return new Exceptional<SelectionResultSet>(new SelectionResultSet(GenerateResult(settingResult.Value, myDBContext.DBTypeManager)));
            }


        }

        #endregion

        #region output result for a attribute

        /// <summary>
        /// generate a output result for setting on a attribute
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="myTypeNode">typenode</param>
        /// <param name="mySessionToken"></param>
        private Exceptional<SelectionResultSet> GenerateAttrResult(string mySettingName, TypeAttribute myAttribute, DBContext myDBContext)
        {
            var setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.ATTRIBUTE, myAttribute.GetRelatedType(myDBContext.DBTypeManager), myAttribute);
            if (setting.Failed())
            {
                return new Exceptional<SelectionResultSet>(setting);
            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(GenerateResult(setting.Value, myDBContext.DBTypeManager)));
        }

        /// <summary>
        /// generate a output result for setting on a attribute
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="myTypeNode">typenode</param>
        /// <param name="mySessionToken"></param>
        private List<SelectionResultSet> GenerateAttrResult(TypeAttribute myAttribute, DBContext myDBContext)
        {
            var result = new List<SelectionResultSet>();
            var settings = myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.ATTRIBUTE, myAttribute.GetRelatedType(myDBContext.DBTypeManager), myAttribute);
            foreach (var setting in settings)
            {
                result.Add(new SelectionResultSet(GenerateResult(setting.Value, myDBContext.DBTypeManager)));
            }

            return result;
        }

        #endregion

        #region generate a output result for a db setting
        
        /// <summary>
        /// generate a output result for a database setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="myDBContext"></param>
        private Exceptional<SelectionResultSet> GenerateDBResult(string mySettingName, DBContext myDBContext)
        {
            var setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.DB, includingDefaults: false);
            if (setting.Failed())
            {
                return new Exceptional<SelectionResultSet>(setting);
            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(GenerateResult(setting.Value, myDBContext.DBTypeManager)));
        }

        /// <summary>
        /// generate a output result for a database setting
        /// </summary>
        /// <param name="myDBContext"></param>
        private List<SelectionResultSet> GenerateDBResult(DBContext myDBContext)
        {
            var _SettingItemValues = new List<SelectionResultSet>();
            foreach (var aDBSetting in myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.DB, includingDefaults: false))
            {
                //aDBSetting.Value.Get(myDBContext, TypesSettingScope.DB);
                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(aDBSetting.Value, myDBContext.DBTypeManager)));
            }

            return _SettingItemValues;

        }

        #endregion

        #region generate a output result for a session setting

        /// <summary>
        /// generate a output result for a session setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="mySessionToken"></param>
        private Exceptional<SelectionResultSet> GenerateSessionResult(string mySettingName, DBContext myDBContext)
        {
            var setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.SESSION);
            if (setting.Failed())
            {
                return new Exceptional<SelectionResultSet>(setting);
            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(GenerateResult(setting.Value, myDBContext.DBTypeManager)));
        }

        /// <summary>
        /// generate a output result for a session setting
        /// </summary>
        /// <param name="mySessionToken"></param>
        private List<SelectionResultSet> GenerateSessionResult(DBContext myDBContext)
        {
            var _SettingItemValues = new List<SelectionResultSet>();
            foreach (var Setting in myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.SESSION, null, null, false))
            {
                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting.Value, myDBContext.DBTypeManager)));
            }
            return _SettingItemValues;
        }

        #endregion  

        #region output for a type

        /// <summary>
        /// generate a output result for settings on a type
        /// </summary>
        /// <param name="myType">type</param>
        /// <param name="mySessionToken">session token</param>
        private List<SelectionResultSet> GenerateTypeResult(GraphDBType myType, DBContext myDBContext)
        {
            var _SettingItemValues = new List<SelectionResultSet>();
            foreach (var Setting in myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.TYPE, myType))
            {
                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting.Value, myDBContext.DBTypeManager)));
            }

            return _SettingItemValues;
        }

        /// <summary>
        /// generate a output result for settings on a type
        /// </summary>
        /// <param name="myType">type</param>
        /// <param name="mySessionToken">session token</param>
        private Exceptional<SelectionResultSet> GenerateTypeResult(string mySettingName, GraphDBType myType, DBContext myDBContext)
        {
            var _SettingItemValues = new List<SelectionResultSet>();
            var Setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.TYPE, myType);
            if (Setting.Failed())
            {
                return new Exceptional<SelectionResultSet>(Setting);
            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(GenerateResult(Setting.Value, myDBContext.DBTypeManager)));
        }

        #endregion

        /// <summary>
        /// generate a output result
        /// </summary>
        /// <param name="mySetting">the setting</param>
        /// <param name="myReadOutList">list of dbreadouts</param>
        private DBObjectReadout GenerateResult(ADBSettingsBase mySetting, DBTypeManager typeManager)
        {

            var Setting = new Dictionary<String, Object>();
            Setting.Add("Name", mySetting.Name);
            Setting.Add("ID", mySetting.ID);
            Setting.Add("Type", typeManager.GetTypeByUUID(mySetting.Type).Name);
            Setting.Add("Desc", mySetting.Description);
            Setting.Add("Default", mySetting.Default);
            Setting.Add("Value", mySetting.Value);

            return new DBObjectReadout(Setting);

        }

        #endregion

    }
}
