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

/* <id name="DescribeNode" />
 * <copyright file="DescribeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;

using sones.GraphDB.Managers.Structures.Describe;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class DescribeNode : AStatement
    {

        #region Data

        private ADescribeDefinition _DescribeDefinition;

        #endregion

        #region Constructor

        public DescribeNode()
        {            
        }

        #endregion        

        #region AStatement

        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            _DescribeDefinition = ((ADescrNode)myParseTreeNode.ChildNodes[1].AstNode).DescribeDefinition;

        }

        public override String StatementName
        {
            get { return "Describe"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession)
        {

            var qresult = myIGraphDBSession.Describe(_DescribeDefinition);
            qresult.PushIExceptional(ParsingResult);
            return qresult;
        }

        #endregion
        
    }

}
