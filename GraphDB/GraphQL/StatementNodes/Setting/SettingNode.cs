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
using sones.GraphDB.Structures.Result;

using sones.Lib.Frameworks.Irony.Parsing;

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
