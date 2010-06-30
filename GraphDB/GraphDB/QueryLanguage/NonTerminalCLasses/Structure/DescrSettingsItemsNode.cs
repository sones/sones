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

/* <id name="sones GraphDB – DescrSettingsItemsNode" />
 * <copyright file="DescrSettingsItemsNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */
#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Settings;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
#endregion


namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescribeSettingsItemsNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet> _SettingItemValues;
        #endregion
        
        #region constructor
        public DescribeSettingsItemsNode()
        { }
        #endregion


        #region output
        /// <summary>
        /// generate an output result for an specific setting
        /// </summary>
        /// <param name="mySetting">the setting</param>
        /// <param name="mySettingName">the name of the setting</param>
        /// <returns></returns>        
        private IEnumerable<DBObjectReadout> GenerateResult(ADBSettingsBase mySetting, String mySettingName)
        {

            var SettingName = new Dictionary<String, Object>();
            SettingName.Add("Name", mySettingName);
            SettingName.Add("ID", mySetting.ID);
            SettingName.Add("Type", mySetting.Type);
            SettingName.Add("Desc", mySetting.Description);
            SettingName.Add("Default", mySetting.Default);
            SettingName.Add("Value", mySetting.Value);

            return new List<DBObjectReadout>() { new DBObjectReadout(SettingName) };

        }

        #endregion

        #region output for a type
        /// <summary>
        /// generate a output result for settings on a type
        /// </summary>
        /// <param name="myType">type</param>
        /// <param name="mySessionToken">session token</param>
        private void GenerateTypeResult(GraphDBType myType, SessionSettings mySessionToken)
        {
            try
            {
                Dictionary<string, ADBSettingsBase> TypeSettings = myType.GetTypeSettings;
                foreach (var Setting in TypeSettings)
                    _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting.Value, Setting.Key)));
            }
            catch (Exception e)
            {
                throw new GraphDBException(new Error_UnknownDBError(e));
            }


        }
        #endregion


        #region output result for a attribute
        /// <summary>
        /// generate a output result for setting on a attribute
        /// </summary>        
        /// <param name="myTypeNode">typenode</param>
        /// <param name="mySessionToken"></param>
        private void GenerateAttrResult(TypeAttribute myAttribute, SessionSettings mySessionToken)
        {
            try
            {
                Dictionary<string, ADBSettingsBase> AttrSettings = myAttribute.GetObjectSettings;

                foreach (var Setting in AttrSettings)
                    _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting.Value, Setting.Key)));
            }
            catch(Exception e)
            {
                new GraphDBException(new Error_UnknownDBError(e));
            }
        }
        #endregion

        #region generate a output result for a db setting
        /// <summary>
        /// generate a output result for a database setting
        /// </summary>
        /// <param name="dbContext"></param>
        private void GenerateDBResult(DBContext dbContext)
        {
            try
            {
                foreach (var aDBSetting  in dbContext.DBSettingsManager.AllSettingsByName)
	            {
                    var currentDBSetting = aDBSetting.Value.Get(dbContext, TypesSettingScope.DB);
                    if (currentDBSetting.Failed)
                    {
                        throw new GraphDBException(currentDBSetting.Errors);
                    }

                    if (currentDBSetting.Value != null)
                    {
                        _SettingItemValues.Add(new SelectionResultSet(GenerateResult(currentDBSetting.Value, currentDBSetting.Value.Name)));
                    }
	            }
            }
            catch(Exception e)
            {
                new GraphDBException(new Error_UnknownDBError(e));
            }
        }
        #endregion

        #region generate a output result for a session setting
        /// <summary>
        /// generate a output result for a session setting
        /// </summary>
        /// <param name="mySessionToken"></param>
        private void GenerateSessionResult(DBContext context)
        {
            try
            {
                foreach (var Setting in context.DBSettingsManager.GetAllSettings(context, TypesSettingScope.SESSION, null, null, false))
                    _SettingItemValues.Add(new SelectionResultSet(GenerateResult((ADBSettingsBase)Setting.Value, Setting.Key)));
            }
            catch(Exception e)
            {
                new GraphDBException(new Error_UnknownDBError(e));
            }
        }
        #endregion        

        #region AStructureNode

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            _SettingItemValues = new List<SelectionResultSet>();
            try
            {
                if (parseNode.HasChildNodes())
                {
                    if (parseNode.ChildNodes.Count >= 2)
                    {
                        switch (parseNode.ChildNodes[1].Token.Text.ToUpper())
                        {
                            case "TYPE":
                                if(parseNode.ChildNodes[2].ChildNodes[0].AstNode != null)
                                    GenerateTypeResult((parseNode.ChildNodes[2].ChildNodes[0].AstNode as ATypeNode).DBTypeStream, dbContext.SessionSettings);
                                break;

                            case "ATTRIBUTE":
                                if(parseNode.ChildNodes[2].HasChildNodes() && parseNode.ChildNodes[2].ChildNodes.Count >= 3)
                                    GenerateAttrResult((parseNode.ChildNodes[2].ChildNodes[2].AstNode as IDNode).LastAttribute, dbContext.SessionSettings);
                                break;

                            case "DB":
                                GenerateDBResult(dbContext);
                                break;

                            case "SESSION":
                                GenerateSessionResult(dbContext);
                                break;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new GraphDBException(new Error_UnknownDBError(e));
            }
        }
        #endregion

        #region Accessor
        public override List<SelectionResultSet> Result
        {
            get { return _SettingItemValues; }
        }
        #endregion
    }
}
