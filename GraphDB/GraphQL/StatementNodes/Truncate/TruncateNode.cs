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


#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.GraphQL;
using sones.GraphDB.Managers;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.GraphQL.StatementNodes;


using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDBInterface.Result;

#endregion

namespace sones.GraphDB.GraphQL.StatementNodes.Truncate
{

    public class TruncateNode : AStatement
    {

        #region Data

        private String _TypeName = ""; //the name of the type that should be dropped

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Truncate"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            var grammar = GetGraphQLGrammar(myCompilerContext);

            // get Name
            _TypeName = myParseTreeNode.ChildNodes.Last().Token.ValueString;
            if (myParseTreeNode.ChildNodes[1].Token == null || myParseTreeNode.ChildNodes[1].Token.AsSymbol != grammar.S_TYPE)
            {
                ParsingResult.Push(new Warnings.Warning_ObsoleteGQL(
                    String.Format("TRUNCATE {0}", _TypeName),
                    String.Format("TRUNCATE TYPE {0}", _TypeName)));
            }

        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession)
        {

            var qresult = myIGraphDBSession.Truncate(_TypeName);
            qresult.AddErrorsAndWarnings(ParsingResult);
            return qresult;

        }
    }
}
