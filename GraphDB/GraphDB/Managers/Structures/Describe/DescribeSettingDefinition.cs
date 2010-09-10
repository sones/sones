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

/*
 * DescribeSettingDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Indices;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;
using sones.GraphFS.Session;
using sones.GraphDBInterface.Result;

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

        public override Exceptional<SelectionResultSet> GetResult(DBContext myDBContext)
        {
            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();

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
                            return new Exceptional<SelectionResultSet>(new Error_TypeDoesNotExist(_SettingTypeName));
                        }
                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            var typeResult = GenerateTypeResult(type, myDBContext);
                            if (typeResult.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(typeResult);
                            }

                            resultingReadouts.AddRange(typeResult.Value);
                        }
                        else
                        {
                            var descrTypeRes = GenerateTypeResult(_SettingName, type, myDBContext);
                            if (descrTypeRes.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(descrTypeRes);
                            }
                            resultingReadouts.AddRange(descrTypeRes.Value);
                        }
                        break;

                        #endregion

                    case TypesSettingScope.ATTRIBUTE:

                        #region ATTRIBUTE

                        Exceptional validateResult = _SettingAttribute.Validate(myDBContext, false);
                        if (validateResult.Failed())
                        {
                            return new Exceptional<SelectionResultSet>(validateResult.Errors);
                        }

                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            var attributeResult = GenerateAttrResult(_SettingAttribute.LastAttribute, myDBContext);
                            if (attributeResult.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(attributeResult);
                            }
                            resultingReadouts.AddRange(attributeResult.Value);
                        }
                        else
                        {
                            var outputResult = GenerateAttrResult(_SettingName, _SettingAttribute.LastAttribute, myDBContext);

                            if (outputResult.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(outputResult);
                            }
                            resultingReadouts.Add(outputResult.Value);
                        }
                        break;

                        #endregion

                    case TypesSettingScope.SESSION:

                        #region SESSION

                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            var sessionReadouts = GenerateSessionResult(myDBContext);
                            if (sessionReadouts.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(sessionReadouts);
                            }
                            resultingReadouts.AddRange(sessionReadouts.Value);
                        }
                        else
                        {
                            var outputResult = GenerateSessionResult(_SettingName, myDBContext);

                            if (outputResult.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(outputResult);
                            }
                            resultingReadouts.Add(outputResult.Value);
                        }
                        break;

                        #endregion

                    case TypesSettingScope.DB:

                        #region DB

                        if (String.IsNullOrEmpty(_SettingName))
                        {
                            var dbResult = GenerateDBResult(myDBContext);
                            if (dbResult.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(dbResult);
                            }
                            resultingReadouts.AddRange(dbResult.Value);
                        }
                        else
                        {
                            var outputResult = GenerateDBResult(_SettingName, myDBContext);
                            if (outputResult.Failed())
                            {
                                return new Exceptional<SelectionResultSet>(outputResult);
                            }
                            resultingReadouts.Add(outputResult.Value);
                        }

                        #endregion

                        break;
                    default:

                        return new Exceptional<SelectionResultSet>(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
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
                    resultingReadouts.AddRange(readOutList);

                    #endregion

                }
                else
                {

                    #region Describe named setting

                    var outputResult = GenerateStdResult(_SettingName, myDBContext);

                    if (outputResult.Failed())
                    {
                        return new Exceptional<SelectionResultSet>(outputResult);
                    }

                    resultingReadouts.Add(outputResult.Value);

                    #endregion

                }

                #endregion

            }

            return new Exceptional<SelectionResultSet>(new SelectionResultSet(resultingReadouts));

        }
        
        #endregion

        #region Output

        #region Standard output value

        /// <summary>
        /// generate a output result if no database, session, type or attribute is requested, then you can get information about the setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="myDBContext"></param>
        private Exceptional<DBObjectReadout> GenerateStdResult(string mySettingName, DBContext myDBContext)
        {

            var settingResult = myDBContext.DBSettingsManager.GetSetting(mySettingName);
            if (settingResult.Failed())
            {
                return new Exceptional<DBObjectReadout>(settingResult);
            }
            else
            {
                return new Exceptional<DBObjectReadout>(GenerateResult(settingResult.Value, myDBContext.DBTypeManager));
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
        private Exceptional<DBObjectReadout> GenerateAttrResult(string mySettingName, TypeAttribute myAttribute, DBContext myDBContext)
        {
            var setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.ATTRIBUTE, myAttribute.GetRelatedType(myDBContext.DBTypeManager), myAttribute);
            if (setting.Failed())
            {
                return new Exceptional<DBObjectReadout>(setting);
            }

            return new Exceptional<DBObjectReadout>(GenerateResult(setting.Value, myDBContext.DBTypeManager));
        }

        /// <summary>
        /// generate a output result for setting on a attribute
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="myTypeNode">typenode</param>
        /// <param name="mySessionToken"></param>
        private Exceptional<List<DBObjectReadout>> GenerateAttrResult(TypeAttribute myAttribute, DBContext myDBContext)
        {
            List<DBObjectReadout> resultingObjects = new List<DBObjectReadout>();

            var settings = myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.ATTRIBUTE, myAttribute.GetRelatedType(myDBContext.DBTypeManager), myAttribute);
            foreach (var setting in settings)
            {
                resultingObjects.Add(GenerateResult(setting.Value, myDBContext.DBTypeManager));
            }

            return new Exceptional<List<DBObjectReadout>>(resultingObjects);
        }

        #endregion

        #region generate a output result for a db setting
        
        /// <summary>
        /// generate a output result for a database setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="myDBContext"></param>
        private Exceptional<DBObjectReadout> GenerateDBResult(string mySettingName, DBContext myDBContext)
        {
            var setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.DB, includingDefaults: false);
            if (setting.Failed())
            {
                return new Exceptional<DBObjectReadout>(setting);
            }

            return new Exceptional<DBObjectReadout>(GenerateResult(setting.Value, myDBContext.DBTypeManager));
        }

        /// <summary>
        /// generate a output result for a database setting
        /// </summary>
        /// <param name="myDBContext"></param>
        private Exceptional<List<DBObjectReadout>> GenerateDBResult(DBContext myDBContext)
        {
            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();

            foreach (var aDBSetting in myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.DB, includingDefaults: false))
            {
                resultingReadouts.Add(GenerateResult(aDBSetting.Value, myDBContext.DBTypeManager));
            }

            return new Exceptional<List<DBObjectReadout>>(resultingReadouts);

        }

        #endregion

        #region generate a output result for a session setting

        /// <summary>
        /// generate a output result for a session setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="mySessionToken"></param>
        private Exceptional<DBObjectReadout> GenerateSessionResult(string mySettingName, DBContext myDBContext)
        {
            var setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.SESSION);
            if (setting.Failed())
            {
                return new Exceptional<DBObjectReadout>(setting);
            }

            return new Exceptional<DBObjectReadout>(GenerateResult(setting.Value, myDBContext.DBTypeManager));
        }

        /// <summary>
        /// generate a output result for a session setting
        /// </summary>
        /// <param name="mySessionToken"></param>
        private Exceptional<List<DBObjectReadout>> GenerateSessionResult(DBContext myDBContext)
        {
            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();

            foreach (var Setting in myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.SESSION, null, null, false))
            {
                resultingReadouts.Add(GenerateResult(Setting.Value, myDBContext.DBTypeManager));
            }
            return new Exceptional<List<DBObjectReadout>>(resultingReadouts);
        }

        #endregion  

        #region output for a type

        /// <summary>
        /// generate a output result for settings on a type
        /// </summary>
        /// <param name="myType">type</param>
        /// <param name="mySessionToken">session token</param>
        private Exceptional<List<DBObjectReadout>> GenerateTypeResult(GraphDBType myType, DBContext myDBContext)
        {
            List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();

            foreach (var Setting in myDBContext.DBSettingsManager.GetAllSettings(myDBContext, TypesSettingScope.TYPE, myType))
            {
                resultingReadouts.Add(GenerateResult(Setting.Value, myDBContext.DBTypeManager));
            }

            return new Exceptional<List<DBObjectReadout>>(resultingReadouts);
        }

        /// <summary>
        /// generate a output result for settings on a type
        /// </summary>
        /// <param name="myType">type</param>
        /// <param name="mySessionToken">session token</param>
        private Exceptional<List<DBObjectReadout>> GenerateTypeResult(string mySettingName, GraphDBType myType, DBContext myDBContext)
        {
            var _SettingItemValues = new List<SelectionResultSet>();
            var Setting = myDBContext.DBSettingsManager.GetSetting(mySettingName, myDBContext, TypesSettingScope.TYPE, myType);
            if (Setting.Failed())
            {
                return new Exceptional<List<DBObjectReadout>>(Setting);
            }

            return new Exceptional<List<DBObjectReadout>>(new List<DBObjectReadout>() { GenerateResult(Setting.Value, myDBContext.DBTypeManager) });
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
