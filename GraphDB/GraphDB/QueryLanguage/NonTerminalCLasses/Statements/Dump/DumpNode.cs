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



#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Dump
{

    public class DumpNode : AStatement
    {

        private DumpFormats _DumpFormat;
        private DumpTypes   _DumpType;
        private IDumpable   _DumpableGrammar;

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            _DumpType           = (parseNode.ChildNodes[1].AstNode as DumpTypeNode).DumpType;
            _DumpFormat         = (parseNode.ChildNodes[2].AstNode as DumpFormatNode).DumpFormat;
            _DumpableGrammar    = context.Compiler.Language.Grammar as IDumpable;

            if (_DumpableGrammar == null)
            {
                throw new GraphDBException(new Error_NotADumpableGrammar(context.Compiler.Language.Grammar.GetType().ToString()));
            }

        }

        public override String StatementName
        {
            get { return "DUMP"; }
        }

        public override Enums.TypesOfStatements TypeOfStatement
        {
            get { return Enums.TypesOfStatements.Readonly; }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="myIGraphDBSession">The DBSession to start new transactions</param>
        /// <param name="myDBContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="myIGraphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession myIGraphDBSession, DBContext myDBContext)
        {

            var _DumpReadout = new Dictionary<String, Object>();

            if ((_DumpType & DumpTypes.GDDL) == DumpTypes.GDDL)
            {

                var _GraphDDL = _DumpableGrammar.ExportGraphDDL(_DumpFormat, myDBContext);

                if (!_GraphDDL.Success)
                {
                    return new QueryResult(_GraphDDL);
                }

                _DumpReadout.Add("GDDL", _GraphDDL.Value);

            }

            if ((_DumpType & DumpTypes.GDML) == DumpTypes.GDML)
            {

                var _GraphDML = _DumpableGrammar.ExportGraphDML(_DumpFormat, myDBContext);

                if (!_GraphDML.Success)
                {
                    return new QueryResult(_GraphDML);
                }

                _DumpReadout.Add("GDML", _GraphDML.Value);

            }

            return new QueryResult(new SelectionResultSet(new List<DBObjectReadout>() { new DBObjectReadout(_DumpReadout) }));

        }

    }

}
