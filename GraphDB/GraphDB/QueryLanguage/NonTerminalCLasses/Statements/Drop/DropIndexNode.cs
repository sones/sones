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
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Drop
{
    public class DropIndexNode : AStatement
    {

        #region Data

        String _IndexName = String.Empty;
        String _IndexEdition = null;
        GraphDBType graphDBTypeType = null;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "DropType"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.ChildNodes[1].AstNode is ATypeNode)
            {
                graphDBTypeType = (GraphDBType)((ATypeNode)parseNode.ChildNodes[1].AstNode).DBTypeStream;
            }
            else
            {
                throw new NotImplementedException(parseNode.ChildNodes[1].ToString());
            }

            _IndexName = parseNode.ChildNodes[4].Token.ValueString;
            if (parseNode.ChildNodes[5].HasChildNodes())
                _IndexEdition = parseNode.ChildNodes[5].ChildNodes[1].Token.ValueString;
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {            
            if (graphDBTypeType == null)
            {
                var aError = new Error_TypeDoesNotExist("");

                return new QueryResult(aError);
            }


            using (var transaction = graphDBSession.BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();
                var RemoveIdxException = graphDBTypeType.RemoveIndex(_IndexName, _IndexEdition, dbContext.DBTypeManager);

                if (!RemoveIdxException.Success)
                {
                    return new QueryResult(RemoveIdxException);
                }
                else
                {
                    
                    #region Commit transaction and add all Warnings and Errors

                    return new QueryResult(transaction.Commit());

                    #endregion

                }

            }
            
        }
    }
}
