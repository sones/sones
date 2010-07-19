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


/* <id name="sones GraphDB – SettingNode" />
 * <copyright file="SettingNode.cs"
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
using sones.Lib.Frameworks.Irony.Parsing;

using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Errors;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.Exceptions;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Setting
{

    public class SettingNode : AStatement
    {

        #region Data

        private SettingContentNode          _SettingContentNode = null;
        private SettingScopeNode            _Scope = null;
        private Dictionary<string, string>  _Settings = null;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Setting"; } }
        
        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Setting; }
        }

        #endregion

        #region constructor
        public SettingNode()
        {
        }
        #endregion

        #region private helper methods

        /// <summary>
        /// this is whaer the aektschn haeppens
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private QueryResult DoAction(DBContext dbContext)
        {

            QueryResult result = new QueryResult();

            try
            {
                if (_Scope.SettingNode is SettingAttrNode)
                {

                    var attrNode = _Scope.SettingNode as SettingAttrNode;

                    switch (_SettingContentNode.ActOperation)
                    {
                        case TypesOfSettingOperation.GET:
                            return new QueryResult(_SettingContentNode.ExtractData(attrNode, _Settings, dbContext, dbContext.SessionSettings));

                        case TypesOfSettingOperation.SET:
                            _SettingContentNode.SetData(attrNode, _Settings, dbContext);
                            break;

                        case TypesOfSettingOperation.REMOVE:
                            _SettingContentNode.RemoveData(attrNode, _Settings, dbContext);
                            break;
                    }

                }

                else if (_Scope.SettingNode is SettingTypeNode)
                {

                    var typeNode = _Scope.SettingNode as SettingTypeNode;

                    switch (_SettingContentNode.ActOperation)
                    {
                        case TypesOfSettingOperation.GET:
                            return new QueryResult(_SettingContentNode.ExtractData(typeNode, _Settings, dbContext, dbContext.SessionSettings));

                        case TypesOfSettingOperation.SET:
                            _SettingContentNode.SetData(typeNode, _Settings, dbContext);
                            break;

                        case TypesOfSettingOperation.REMOVE:
                            _SettingContentNode.RemoveData(typeNode, _Settings, dbContext.DBTypeManager);
                            break;
                    }

                }
                else
                {
                    switch (_SettingContentNode.ActOperation)
                    {
                        case TypesOfSettingOperation.GET:
                            return new QueryResult(_SettingContentNode.ExtractData(_Settings, dbContext));

                        case TypesOfSettingOperation.SET:
                            _SettingContentNode.SetData(_Settings, dbContext);
                            break;

                        case TypesOfSettingOperation.REMOVE:
                            _SettingContentNode.RemoveData(_Settings, dbContext);
                            break;
                    }
                }
            }
            catch (GraphDBException e)
            {
                return new QueryResult(e.GraphDBErrors);
            }


            if (_SettingContentNode.ActOperation == TypesOfSettingOperation.SET)
            {
                #region prepare result

                List<DBObjectReadout> resultingReadouts = new List<DBObjectReadout>();

                switch (_Scope.Scope)
                {
                    case TypesSettingScope.DB:

                        resultingReadouts.Add(CreateNewSettingReadoutOnSet(TypesSettingScope.DB, _Settings));

                        break;

                    case TypesSettingScope.SESSION:

                        resultingReadouts.Add(CreateNewSettingReadoutOnSet(TypesSettingScope.SESSION, _Settings));

                        break;
                    case TypesSettingScope.TYPE:

                        foreach (var aType in ((SettingTypeNode)_Scope.SettingNode).Types)
                        {
                            resultingReadouts.Add(CreateNewTYPESettingReadoutOnSet(TypesSettingScope.TYPE, aType, _Settings));
                        }

                        break;
                    case TypesSettingScope.ATTRIBUTE:

                        foreach (var aType in ((SettingAttrNode)_Scope.SettingNode).Attributes)
                        {
                            resultingReadouts.Add(CreateNewATTRIBUTESettingReadoutOnSet(TypesSettingScope.ATTRIBUTE, aType, _Settings));
                        }

                        break;
                    default:

                        return new QueryResult(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
                }

                return new QueryResult(new List<SelectionResultSet> { new SelectionResultSet(null, resultingReadouts) });

                #endregion
            }

            return new QueryResult();
        }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            try
            {
                _SettingContentNode = new SettingContentNode(dbContext);

                _Scope            = (SettingScopeNode)     parseNode.ChildNodes[1].AstNode;
                var settingOpNode = (SettingOperationNode) parseNode.ChildNodes[2].AstNode;

                _SettingContentNode.ActScope     = _Scope.Scope;
                _SettingContentNode.ActOperation = settingOpNode.OperationType;
                _Settings                        = settingOpNode.Settings;
            }
            catch (GraphDBException e)
            {
                throw new GraphDBException(e.GraphDBErrors);
            }

        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {

            using (var transaction = graphDBSession.BeginTransaction())
            {
                var transactionContext = transaction.GetDBContext();

                var result = DoAction(transaction.GetDBContext());

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;
            }
        }

        private DBObjectReadout CreateNewSettingReadoutOnSet(TypesSettingScope typesSettingScope, Dictionary<string, string> _Settings)
        {
            Dictionary<String, Object> payload = new Dictionary<string, object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());

            foreach (var aSetting in _Settings)
            {
                payload.Add(aSetting.Key, aSetting.Value);
            }

            return new DBObjectReadout(payload);
        }

        private DBObjectReadout CreateNewTYPESettingReadoutOnSet(TypesSettingScope typesSettingScope, GraphDBType aType, Dictionary<string, string> _Settings)
        {
            Dictionary<String, Object> payload = new Dictionary<string, object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());
            payload.Add(typesSettingScope.ToString(), aType.Name);

            foreach (var aSetting in _Settings)
            {
                payload.Add(aSetting.Key, aSetting.Value);
            }

            return new DBObjectReadout(payload);
        }

        private DBObjectReadout CreateNewATTRIBUTESettingReadoutOnSet(TypesSettingScope typesSettingScope, KeyValuePair<string, List<TypeAttribute>> aType, Dictionary<string, string> _Settings)
        {
            Dictionary<String, Object> payload = new Dictionary<string, object>();

            payload.Add(DBConstants.SettingScopeAttribute, typesSettingScope.ToString());
            payload.Add(TypesSettingScope.TYPE.ToString(), aType.Key);

            List<DBObjectReadout> attributes = new List<DBObjectReadout>();

            foreach (var aAttribute in aType.Value)
            {
                Dictionary<String, Object> innerPayload = new Dictionary<string, object>();

                innerPayload.Add(TypesSettingScope.ATTRIBUTE.ToString(), aAttribute.Name);

                foreach (var aSetting in _Settings)
                {
                    innerPayload.Add(aSetting.Key, aSetting.Value);
                }

                attributes.Add(new DBObjectReadout(innerPayload));
            }

            payload.Add(DBConstants.SettingAttributesAttribute, new Edge(attributes, DBConstants.SettingAttributesAttributeTYPE));

            return new DBObjectReadout(payload);
        }

    }

}
