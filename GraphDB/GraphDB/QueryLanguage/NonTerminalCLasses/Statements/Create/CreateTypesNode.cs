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


/* <id name="sones GraphDB – CreateTypes astnode" />
 * <copyright file="CreateTypesNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an Create Types statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Structures;
using sones.GraphFS.Session;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Warnings;
using sones.Lib;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{
    /// <summary>
    /// This node is requested in case of an Create Types statement.
    /// </summary>
    public class CreateTypesNode : AStatement
    {

        #region Data

        private List<GraphDBTypeDefinition> _TypeDefinitions = new List<GraphDBTypeDefinition>();

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "CreateTypes"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public CreateTypesNode()
        {
            
        }

        #endregion

        #region Execute

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
                var innerContext = transaction.GetDBContext();

                var result = innerContext.DBTypeManager.AddBulkTypes(_TypeDefinitions, innerContext);

                if (!result.Success)
                {

                    #region Rollback transaction and add all Warnings and Errors

                    result.AddErrorsAndWarnings(transaction.Rollback());

                    #endregion

                    return new QueryResult(result.Errors);

                }
                else
                {

                    #region Commit transaction and add all Warnings and Errors

                    result.Value.AddErrorsAndWarnings(transaction.Commit());

                    #endregion

                    return result.Value;

                }

            }

        }

        #endregion

        #region GetContent

        /// <summary>
        /// Gets the content of a CreateTypeStatement.
        /// </summary>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.ChildNodes.Count > 3)
            {

                #region Single type

                BulkTypeNode aTempNode = (BulkTypeNode)myParseTreeNode.ChildNodes[3].AstNode;

                Boolean isAbstract = false;

                if (myParseTreeNode.ChildNodes[1].HasChildNodes())
                {
                    isAbstract = true;
                }

                _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, isAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                
                #endregion

            }

            else
            {

                #region Multi types

                foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[2].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode != null)
                    {
                        BulkTypeListMemberNode aTempNode = (BulkTypeListMemberNode)_ParseTreeNode.AstNode;
                        _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, aTempNode.IsAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                    }
                }

                #endregion

            }


        }

        #endregion
    
    }

}
