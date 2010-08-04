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


/* <id name="sones GraphDB – create type astnode" />
 * <copyright file="CreateIndexNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This node is requested in case of an create type statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement;

using sones.Lib.DataStructures.Indices;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Managers;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{

    /// <summary>
    /// This node is requested in case of an create index statement.
    /// </summary>
    public class CreateIndexNode : AStatement
    {

        #region Data

        String                          _IndexName          = null;
        String                          _IndexEdition       = null;
        String                          _DBType             = null;
        List<IndexAttributeDefinition>  _AttributeList      = null;
        String                          _IndexType;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "CreateIndex"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region Constructors

        public CreateIndexNode()
        { }

        #endregion

        #region GetContent

        /// <summary>
        /// Gets the content of an UpdateStatement.
        /// </summary>
        /// <param name="myCompilerContext">CompilerContext of Irony.</param>
        /// <param name="myParseTreeNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            foreach (var child in myParseTreeNode.ChildNodes)
            {
                if (child.AstNode != null)
                {
                    if (child.AstNode is IndexNameOptNode)
                    {
                        _IndexName = (child.AstNode as IndexNameOptNode).IndexName;
                    }
                    else if (child.AstNode is EditionOptNode)
                    {
                        _IndexEdition = (child.AstNode as EditionOptNode).IndexEdition;
                    }
                    else if (child.AstNode is ATypeNode)
                    {
                        _DBType = (child.AstNode as ATypeNode).ReferenceAndType.TypeName;
                    }
                    else if (child.AstNode is IndexAttributeListNode)
                    {
                        _AttributeList = (child.AstNode as IndexAttributeListNode).IndexAttributes;
                    }
                    else if (child.AstNode is IndexTypeOptNode)
                    {
                        _IndexType = (child.AstNode as IndexTypeOptNode).IndexType;
                    }
                }
            }

        }

        #endregion

        #region Execute

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="transactionContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession, DBContext myDBContext)
        {

            using (var _Transaction = myIGraphDBSession.BeginTransaction())
            {

                var transactionContext = _Transaction.GetDBContext();
                var qresult = new QueryResult();

                #region Create the index

                var resultOutput = transactionContext.DBIndexManager.CreateIndex(transactionContext, _DBType, _IndexName, _IndexEdition, _IndexType, _AttributeList);
                qresult.AddErrorsAndWarnings(resultOutput);

                #endregion

                #region Commit transaction and add all Warnings and Errors

                qresult.AddErrorsAndWarnings(_Transaction.Commit());

                #endregion

                if (qresult.ResultType == ResultType.Successful)
                {
                    qresult.AddResult(resultOutput.Value);
                }

                return qresult;

            }

        }

        #endregion

    }

}
