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

/* <id name="GraphDB – Insert astnode" />
 * <copyright file="InsertNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of an Insert statement.</summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StructureNodes;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDBInterface.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Insert
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
        /// <param name="typeManager">The TypeManager of the GraphDB.</param>
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
        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            var qresult = graphDBSession.Insert(_TypeName, _AttributeAssignList);
            qresult.AddErrorsAndWarnings(ParsingResult);
            return qresult;

        }


        #endregion

    }

}
