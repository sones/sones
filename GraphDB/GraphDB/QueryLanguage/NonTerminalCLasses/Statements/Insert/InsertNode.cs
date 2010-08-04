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


/* <id name="sones GraphDB – Insert astnode" />
 * <copyright file="InsertNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an Insert statement.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Operators;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.Lib.Frameworks.Irony.Parsing;

using sones.GraphDB.TypeManagement.PandoraTypes;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.DataStructures;
using sones.GraphDB.Settings;
using sones.Lib.DataStructures.UUID;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Managers;
using sones.Lib;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Managers.Structures;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{
    /// <summary>
    /// This node is requested in case of an Insert statement.
    /// </summary>
    class InsertNode : AStatement
    {

        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        
        #region Properties - Statement information

        public override String StatementName { get { return "Insert"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public InsertNode()
        {
        }

        #endregion

        #region public AStatement methods

        /// <summary>
        /// Gets the content of a InsertStatement.
        /// </summary>
        /// <param name="context">CompilerContext of Irony.</param>
        /// <param name="parseNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            #region get type for name

            _TypeName = GetTypeReferenceDefinitions(myCompilerContext).First().TypeName;

            #endregion

            #region get myAttributes

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {

                _AttributeAssignList = ((myParseTreeNode.ChildNodes[3].ChildNodes[1].AstNode as AttrAssignListNode).AttributeAssigns);

            }

            #endregion

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

                var dbInnerContext = transaction.GetDBContext();
                ObjectManipulationManager _ObjectManipulationManager = new ObjectManipulationManager();

                var graphDBType = dbInnerContext.DBTypeManager.GetTypeByName(_TypeName);
                if (graphDBType == null)
                {
                    return new QueryResult(new Error_TypeDoesNotExist(_TypeName));
                }

                var attrsResult = _ObjectManipulationManager.EvaluateAttributes(dbContext, graphDBType, _AttributeAssignList);
                if (!attrsResult.Success)
                {
                    return new QueryResult(attrsResult);
                }

                var result = _ObjectManipulationManager.Insert(dbContext, graphDBType, attrsResult.Value);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }


        #endregion

    }
}
