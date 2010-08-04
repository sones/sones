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
using sones.GraphDB.Managers.Structures.Setting;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Setting
{

    public class SettingNode : AStatement
    {

        #region Fields

        private TypesOfSettingOperation _TypeOfSettingOperation;
        private Dictionary<string, string> _Settings = null;
        private ASettingDefinition _ASettingDefinition = null;

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

        #region override AStatement

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            _ASettingDefinition = ((SettingScopeNode)parseNode.ChildNodes[1].AstNode).SettingDefinition;

            var settingOpNode = (SettingOperationNode)parseNode.ChildNodes[2].AstNode;
            _TypeOfSettingOperation = settingOpNode.OperationType;
            _Settings = settingOpNode.Settings;

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

                var result = dbContext.DBSettingsManager.ExecuteSettingOperation(transaction.GetDBContext(), _ASettingDefinition, _TypeOfSettingOperation, _Settings);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;
            }
        }

        #endregion


    }

}
