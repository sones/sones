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

using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

namespace sones.GraphDB.GraphQL.StatementNodes.Drop
{
    public class DeleteNode : AStatement
    {

        #region Data

        private BinaryExpressionDefinition _WhereExpression;
        
        private List<IDChainDefinition> _IDChainDefinitions;

        private List<TypeReferenceDefinition> _TypeReferenceDefinitions;
        

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Delete"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            _IDChainDefinitions = new List<IDChainDefinition>();

            _TypeReferenceDefinitions = (myParseTreeNode.ChildNodes[1].AstNode as TypeListNode).Types;

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {
                IDNode tempIDNode;
                foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[3].ChildNodes[0].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode is IDNode)
                    {
                        tempIDNode = (IDNode)_ParseTreeNode.AstNode;
                        _IDChainDefinitions.Add(tempIDNode.IDChainDefinition);
                    }
                }
            }
            else
            {
                foreach (var type in _TypeReferenceDefinitions)
                {
                    var def = new IDChainDefinition();
                    def.AddPart(new ChainPartTypeOrAttributeDefinition(type.Reference));
                    _IDChainDefinitions.Add(def);
                }
            }

            #region whereClauseOpt

            if (myParseTreeNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)myParseTreeNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinaryExpressionDefinition;

            }

            #endregion
        
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.Delete(_TypeReferenceDefinitions, _IDChainDefinitions, _WhereExpression);
            qresult.PushIExceptional(ParsingResult);
            return qresult;
        }

    }
}
