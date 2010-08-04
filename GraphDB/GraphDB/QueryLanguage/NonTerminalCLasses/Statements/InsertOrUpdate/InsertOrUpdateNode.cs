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


/* <id name="sones GraphDB – InsertOrUpdateNode" />
 * <copyright file="InsertOrUpdateNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region Usings
using System.Collections.Generic;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Errors;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.ErrorHandling;
using System.Linq;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Managers;
using sones.GraphDB.Exceptions;
using sones.Lib;
using sones.GraphDB.Managers.Structures;
using System;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class InsertOrUpdateNode : AStatement
    {

        #region Constructor
        public InsertOrUpdateNode()
        { }
        #endregion

        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private BinaryExpressionDefinition _WhereExpression;
        private String _Type;

        public override string StatementName
        {
            get { return "InsertOrUpdate"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            if (parseNode.HasChildNodes())
            {
                //get type
                if (parseNode.ChildNodes[1] != null && parseNode.ChildNodes[1].AstNode != null)
                {
                    _Type = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).ReferenceAndType.TypeName;
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }


                if (parseNode.ChildNodes[3] != null && parseNode.ChildNodes[3].HasChildNodes())
                {

                    _AttributeAssignList = (parseNode.ChildNodes[3].AstNode as AttrAssignListNode).AttributeAssigns;

                }

                if (parseNode.ChildNodes[4] != null && ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinExprNode != null)
                {
                    _WhereExpression = ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinExprNode.BinaryExpressionDefinition;
                }
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

            var _ObjectManipulationManager = new ObjectManipulationManager();
            using (var transaction = graphDBSession.BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();

                var graphDBType = dbInnerContext.DBTypeManager.GetTypeByName(_Type);
                if (graphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(_Type));
                }

                var result = _ObjectManipulationManager.InsertOrUpdate(dbInnerContext, graphDBType, _AttributeAssignList, _WhereExpression);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

        #region private helper methods

        
        #endregion  
    }
}
