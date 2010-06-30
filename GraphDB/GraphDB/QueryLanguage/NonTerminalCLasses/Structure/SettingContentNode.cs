/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* <id name="sones GraphDB – SettingContentNode" />
 * <copyright file="SettingContentNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Settings;
using System.Runtime.InteropServices;

using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphFS.Session;
using sones.Lib.Session;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{

    public class SettingContentNode
    {
        #region Data

        private TypesSettingScope                           _ActScope;
        private TypesOfSettingOperation                     _ActOperation;
        private Dictionary<string, ADBSettingsBase>         _Settings;
        private Dictionary<string, List<ADBSettingsBase>>   _SettingTypeAttr;
        //private SessionSettings _CurrentToken;

        #endregion

        #region constructor

        public SettingContentNode(DBContext dbContext)
        {
            _Settings = new Dictionary<string, ADBSettingsBase>();
            _SettingTypeAttr = new Dictionary<string, List<ADBSettingsBase>>();
            //_CurrentToken = mySessionToken;
        }
        
        #endregion

        #region Implementation

        #region private helper methods

        private void AddOrChangeEntry(ADBSettingsBase mySetting, Dictionary<string, ADBSettingsBase> myDict)
        {
            if (!myDict.ContainsKey(mySetting.Name))
                myDict.Add(mySetting.Name, (ADBSettingsBase)mySetting.Clone());
            else
                myDict[mySetting.Name] = (ADBSettingsBase)mySetting.Clone();
        }

        private void MakeOutputForAttribs(ADBSettingsBase mySetting, Dictionary<String, Object> mySettingList)
        {
            mySettingList.Add("Name", mySetting.Name);
            mySettingList.Add("ID", mySetting.ID);
            mySettingList.Add("Type", mySetting.Type);
            mySettingList.Add("Value", mySetting.Value);
            mySettingList.Add("Default", mySetting.Default);
        }

        private void AddOrChangeEntry(string myName, ADBSettingsBase mySetting, Dictionary<string, Dictionary<string, ADBSettingsBase>> myDict)
        {
            if (!myDict.ContainsKey(myName))
            {
                Dictionary<string, ADBSettingsBase> NewDict = new Dictionary<string, ADBSettingsBase>();
                NewDict.Add(mySetting.Name, (ADBSettingsBase)mySetting.Clone());
                myDict.Add(myName, NewDict);
            }
            else
                AddOrChangeEntry(mySetting, myDict[myName]);
        }

        private string RemoveShow(string myInput)
        {
            string Temp = "SHOW";

            if (myInput.Contains("SHOW"))
                return myInput.Substring(Temp.Length, myInput.Length - Temp.Length);
            else
                return myInput;
        }

        private ADBBaseObject GetValueForSetting(ADBSettingsBase mySetting, String myValue)
        {

            var _TypeUUID = mySetting.Type;

            if (myValue != "DEFAULT")
                return PandoraTypeMapper.GetADBBaseObjectFromUUID(_TypeUUID, myValue);

            else
                return PandoraTypeMapper.GetADBBaseObjectFromUUID(_TypeUUID, mySetting.Default.Value);

        }

        #endregion


        #region extract data

        public List<SelectionResultSet> ExtractData(Dictionary<string, string> mySetting, DBContext _DBContext)
        {
            ADBSettingsBase Setting = null;
            List<SelectionResultSet> result = new List<SelectionResultSet>();
            List<DBObjectReadout> SettingList = new List<DBObjectReadout>();

            foreach (var pSetting in mySetting)
            {
                switch (_ActScope)
                {
                    case TypesSettingScope.DB:

                        Setting = _DBContext.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), _DBContext, TypesSettingScope.DB).Value;
                        if (Setting != null)
                        {
                            AddOrChangeEntry(Setting, _Settings);
                        }

                        break;

                    case TypesSettingScope.SESSION:

                        Setting = _DBContext.DBSettingsManager.GetSetting(pSetting.Key.ToUpper(), _DBContext, TypesSettingScope.SESSION).Value;
                        if (Setting != null)
                        {
                            AddOrChangeEntry(Setting, _Settings);
                        }

                        break;
                }

                var SettingPair = new Dictionary<String, Object>();
                MakeOutputForAttribs(Setting, SettingPair);
                SettingList.Add(new DBObjectReadout(SettingPair));
            }
            
            result.Add(new SelectionResultSet(SettingList));

            return result;
        }

        public List<SelectionResultSet> ExtractData(SettingTypeNode myType, Dictionary<string, string> mySetting, DBContext context, SessionSettings sessionSettings)
        {
            List<SelectionResultSet>    result      = new List<SelectionResultSet>();
            Dictionary<String, Object> SettingPair;
            
            foreach (var Type in myType.Types)
            {
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
                        throw (new GraphDBException(new Error_SettingDoesNotExist(pSetting.Key)));

                    SettingPair = new Dictionary<String, Object>();
                    MakeOutputForAttribs(Setting, SettingPair);
                    SettingList.Add(new DBObjectReadout(SettingPair));
                }
                result.Add(new SelectionResultSet(SettingList));
            }

            return result;
        }

        public List<SelectionResultSet> ExtractData(SettingAttrNode myAttribute, Dictionary<string, string> mySettingName, DBContext context, SessionSettings sessionSettings)
        {
            List<SelectionResultSet>   result       = new List<SelectionResultSet>();
            Dictionary<String, Object> SettingPair;
            
            foreach(var Attr in myAttribute.Attributes)
            {
                foreach (var Entry in Attr.Value)
                {
                    var SettingList = new List<DBObjectReadout>();
                    
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
                            throw (new GraphDBException(new Error_SettingDoesNotExist(pSetting.Key)));

                        SettingPair = new Dictionary<String, Object>();
                        MakeOutputForAttribs(Setting, SettingPair);
                        SettingList.Add(new DBObjectReadout(SettingPair));
                    }
                    result.Add(new SelectionResultSet(SettingList));
                }
            }

            return result;
        }

        #endregion

        #region set data

        public void SetData(SettingTypeNode myType, Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {
            foreach (var Type in myType.Types)
            {
                foreach (var pSetting in mySettingValues)
                {
                    if (_DBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                    {
                        _DBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), _DBContext, TypesSettingScope.TYPE, Type);
                    }
                    else
                    {
                        throw (new GraphDBException(new Error_SettingDoesNotExist(pSetting.Key)));
                    }
                }
            }
        }

        public void SetData(SettingAttrNode myAttr, Dictionary<string, string> mySettingValues, DBContext _DBContext)
        {
            
            foreach (var Type in myAttr.Attributes)
            {
                GraphDBType pType = _DBContext.DBTypeManager.GetTypeByName(Type.Key);
                if (pType != null)
                {
                    foreach (var Attr in Type.Value)
                    {
                        foreach (var pSetting in mySettingValues)
                        {
                            if (_DBContext.DBSettingsManager.HasSetting(pSetting.Key.ToUpper()))
                            {
                                _DBContext.DBSettingsManager.SetSetting(pSetting.Key.ToUpper(), GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key.ToUpper()], pSetting.Value), _DBContext, TypesSettingScope.ATTRIBUTE, pType, Attr);
                            }
                            else
                            {
                                throw (new GraphDBException(new Error_SettingDoesNotExist(pSetting.Key)));
                            }
                        }
                    }
                }
                else
                    throw new GraphDBException(new Error_TypeDoesNotExist(pType.Name));
            }
        }

        public void SetData(Dictionary<string, string> mySettings, DBContext _DBContext)
        {

            foreach (var pSetting in mySettings)
            {
                _DBContext.DBSettingsManager.SetSetting(pSetting.Key, GetValueForSetting(_DBContext.DBSettingsManager.AllSettingsByName[pSetting.Key], pSetting.Value), _DBContext, _ActScope);
            }

        }

        #endregion

        #region remove data

        public void RemoveData(SettingAttrNode myAttr, Dictionary<String, String> mySettings, DBContext _DBContext)
        {

            foreach (var myType in myAttr.Attributes)
            { 

                var _GraphDBType = _DBContext.DBTypeManager.GetTypeByName(myType.Key);

                if (_GraphDBType != null)
                {
                    foreach (var Attr in myType.Value)
                        foreach (var Setting in mySettings)
                            Attr.RemovePersistentSetting(Setting.Key.ToUpper(), _DBContext.DBTypeManager);
                }

            }

        }

        public void RemoveData(SettingTypeNode myType, Dictionary<String, String> mySettings, DBTypeManager myTypeManager)
        {
            foreach (var Type in myType.Types)
            {
                foreach (var Setting in mySettings)
                {
                    Type.RemovePersistentSetting(Setting.Key.ToUpper(), myTypeManager);
                }
            }
        }

        public void RemoveData(Dictionary<string, string> mySettings, DBContext _DBContext)
        { 
            foreach(var Setting in mySettings)
            {
                switch (_ActScope)
                { 
                    case TypesSettingScope.DB:
                        _DBContext.DBSettingsManager.RemoveSetting(_DBContext, Setting.Key.ToUpper(), TypesSettingScope.DB);
                    break;

                    case TypesSettingScope.SESSION:
                    _DBContext.DBSettingsManager.RemoveSetting(_DBContext, Setting.Key.ToUpper(), TypesSettingScope.SESSION);

                    break;
                }
            }
        }

        #endregion

        #endregion

        #region Accessessors

        public TypesSettingScope ActScope
        {
            get { return _ActScope;  }
            set { _ActScope = value; }
        }

        public TypesOfSettingOperation ActOperation
        {
            get { return _ActOperation; }
            set { _ActOperation = value; }        
        }

        public Dictionary<string, ADBSettingsBase> Settings
        {
            get { return _Settings; }
        }

        public Dictionary<string, List<ADBSettingsBase>> SettingsAttr
        {
            get { return _SettingTypeAttr; }
        }

        #endregion
    }
}
