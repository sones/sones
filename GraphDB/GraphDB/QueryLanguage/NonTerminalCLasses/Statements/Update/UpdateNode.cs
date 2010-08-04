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


/* <id name="sones GraphDB – Update astnode" />
 * <copyright file="UpdateNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an Update statement.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.ObjectManagement;

using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{

    /// <summary>
    /// This node is requested in case of an Update statement.
    /// </summary>
    class UpdateNode : AStatement
    {

        #region Data

        private HashSet<AAttributeAssignOrUpdateOrRemove> _listOfUpdates;

        private BinaryExpressionDefinition _WhereExpression;
        private String _TypeName;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Update"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public UpdateNode()
        {
        }

        #endregion       

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

                var graphDBType = transactionContext.DBTypeManager.GetTypeByName(_TypeName);
                if (graphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(_TypeName));
                }

                var objectManipulationManager = new ObjectManipulationManager();
                var queryResult = objectManipulationManager.Update(_listOfUpdates, transactionContext, graphDBType, _WhereExpression);

                #region Commit transaction and add all Warnings and Errors

                queryResult.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return queryResult;

            }
        }

        /// <summary>
        /// Gets the content of an UpdateStatement.
        /// </summary>
        /// <param name="context">CompilerContext of Irony.</param>
        /// <param name="parseNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            #region get Type

            _TypeName = (myParseTreeNode.ChildNodes[1].AstNode as ATypeNode).ReferenceAndType.TypeName;

            #endregion

            #region get myAttributes

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {
                var AttrUpdateOrAssign = (AttrUpdateOrAssignListNode)myParseTreeNode.ChildNodes[3].AstNode;
                _listOfUpdates = AttrUpdateOrAssign.ListOfUpdate;
            }

            #endregion

            #region whereClauseOpt

            if (myParseTreeNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)myParseTreeNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinExprNode.BinaryExpressionDefinition;

            }

            #endregion

        }
    } 
}
