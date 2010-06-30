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

/* <id name="sones GraphDB – DescrSettItemNode" />
 * <copyright file="DescrSettItemNode.cs"
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
    public class DescribeSettItemNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet> _SettingItemValues;
        #endregion

        #region constructor
        public DescribeSettItemNode()
        {
        }
        #endregion


        #region output result for a type
        /// <summary>
        /// generate a output result for setting on a type
        /// </summary>
        /// <param name="myType">the name of the type</param>
        /// <param name="myTypeName">name of the type which has the setting</param>
        /// <param name="mySessionToken"></param>
        private void GenerateTypeResult(string mySettingName, GraphDBType myType, SessionSettings mySessionToken, DBContext context)
        {
            ADBSettingsBase Setting = context.DBSettingsManager.GetSetting(mySettingName, context, TypesSettingScope.TYPE, myType).Value;

            if (Setting != null)
            {
                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting, mySettingName)));
            }
            else
            {
                new GraphDBException(new Error_SettingDoesNotExist(mySettingName));
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
        private void GenerateAttrResult(string mySettingName, TypeAttribute myAttribute, DBContext context)
        {
            try
            {
                ADBSettingsBase Setting = context.DBSettingsManager.GetSetting(mySettingName, context, TypesSettingScope.ATTRIBUTE, myAttribute.GetRelatedType(context.DBTypeManager), myAttribute).Value;

                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting, mySettingName)));
            }
            catch
            {
                new GraphDBException(new Error_SettingDoesNotExist(mySettingName));
            }
        }
        #endregion

        /// <summary>
        /// generate an output result for an specific setting
        /// </summary>
        /// <param name="mySetting">the setting</param>
        /// <param name="mySettingName">the name of the setting</param>
        /// <returns></returns>        
        private IEnumerable<DBObjectReadout> GenerateResult(ADBSettingsBase mySetting, string mySettingName)
        {
            var RetVal = new List<DBObjectReadout>();

            Dictionary<string, object> SettingName = new Dictionary<string, object>();
            SettingName.Add("Name", mySettingName);
            SettingName.Add("ID", mySetting.ID);
            SettingName.Add("Type", mySetting.Type);
            SettingName.Add("Desc", mySetting.Description);
            SettingName.Add("Default", mySetting.Default);
            SettingName.Add("Value", mySetting.Value);
            RetVal.Add(new DBObjectReadout(SettingName));

            return RetVal;
        }

        #region generate a output result for a db setting
        /// <summary>
        /// generate a output result for a database setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="dbContext"></param>
        private void GenerateDBResult(string mySettingName, DBContext dbContext)
        {
            try
            {
                var Setting = dbContext.DBSettingsManager.AllSettingsByName[mySettingName].Get(dbContext, TypesSettingScope.DB);
                if (Setting.Failed)
                    throw new GraphDBException(Setting.Errors);

                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting.Value, mySettingName)));
            }
            catch
            {
                new GraphDBException(new Error_SettingDoesNotExist(mySettingName));
            }
        }
        #endregion

        #region generate a output result for a session setting
        /// <summary>
        /// generate a output result for a session setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="mySessionToken"></param>
        private void GenerateSessionResult(string mySettingName, SessionSettings mySessionToken)
        {
            try
            {
                ADBSettingsBase Setting = (ADBSettingsBase)mySessionToken.GetSettingValue(mySettingName);

                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(Setting, mySettingName)));
            }
            catch
            {
                new GraphDBException(new Error_SettingDoesNotExist(mySettingName));
            }
        }
        #endregion


        #region standard output value
        /// <summary>
        /// generate a output result if no database, session, type or attribute is requested, then you can get information about the setting
        /// </summary>
        /// <param name="mySettingName">the name of the setting</param>
        /// <param name="dbContext"></param>
        private void GenerateStdResult(string mySettingName, DBContext dbContext)
        {
            if (dbContext.DBSettingsManager.HasSetting(mySettingName))
            {
                _SettingItemValues.Add(new SelectionResultSet(GenerateResult(dbContext.DBSettingsManager.AllSettingsByName[mySettingName], mySettingName)));
            }
            else
            {
                throw new GraphDBException(new Error_SettingDoesNotExist(mySettingName));
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
                    if (parseNode.ChildNodes.Count >= 3)
                    {
                        switch (parseNode.ChildNodes[2].Token.Text.ToUpper())
                        {
                            case "TYPE":
                                if(parseNode.ChildNodes[3].AstNode != null)
                                    GenerateTypeResult(parseNode.ChildNodes[0].Token.ValueString.ToUpper(), (parseNode.ChildNodes[3].AstNode as ATypeNode).DBTypeStream, dbContext.SessionSettings, dbContext);
                                break;

                            case "ATTRIBUTE":
                                if(parseNode.ChildNodes[3].HasChildNodes() && parseNode.ChildNodes[3].ChildNodes.Count >= 3)
                                    GenerateAttrResult(parseNode.ChildNodes[0].Token.ValueString.ToUpper(), (parseNode.ChildNodes[3].ChildNodes[2].AstNode as IDNode).LastAttribute, dbContext);
                                break;

                            case "DB":
                                GenerateDBResult(parseNode.ChildNodes[0].Token.ValueString.ToUpper(), dbContext);
                                break;

                            case "SESSION":
                                GenerateSessionResult(parseNode.ChildNodes[0].Token.ValueString.ToUpper(), dbContext.SessionSettings);
                                break;
                        }
                        
                    }
                    else
                        GenerateStdResult(parseNode.ChildNodes[0].Token.ValueString.ToUpper(), dbContext);
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
        { get { return _SettingItemValues; } }
        #endregion
    }
}
