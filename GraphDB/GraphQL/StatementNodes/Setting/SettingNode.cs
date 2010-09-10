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

/* <id name="GraphDB – SettingNode" />
 * <copyright file="SettingNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures.Setting;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDBInterface.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Setting
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
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.Setting(_TypeOfSettingOperation, _Settings, _ASettingDefinition);
            qresult.AddErrorsAndWarnings(ParsingResult);
            return qresult;

        }

        #endregion

    }

}
