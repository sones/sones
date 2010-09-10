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
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.ImportExport;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Import
{
    public class ImportNode : AStatement
    {

        #region Overrides

        public override string StatementName
        {
            get { return "Import"; }
        }

        public override Enums.TypesOfStatements TypeOfStatement
        {
            get { return Enums.TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region Properties

        public AGraphDBImport Importer { get; private set; }
        public String SourceLocation { get; private set; }
        public UInt32 ParallelTasks { get; private set; }
        public List<String> Comments { get; private set; }
        public UInt64? Offset { get; private set; }
        public UInt64? Limit { get; private set; }
        public VerbosityTypes VerbosityType { get; private set; }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var dbContext = context.IContext as DBContext;

            // parseNode.ChildNodes[0] - import symbol
            // parseNode.ChildNodes[1] - from symbol
            SourceLocation = parseNode.ChildNodes[2].Token.ValueString;
            // parseNode.ChildNodes[3] - format symbol

            var importFormat = parseNode.ChildNodes[4].Token.Text;
            if (dbContext.DBPluginManager.HasGraphDBImporter(importFormat))
            {
                Importer = dbContext.DBPluginManager.GetGraphDBImporter(importFormat);
            }
            else
            {
                throw new GraphDBException(new Error_ImporterDoesNotExist(importFormat));
            }

            ParallelTasks = (parseNode.ChildNodes[5].AstNode as ParallelTasksNode).ParallelTasks;
            Comments = (parseNode.ChildNodes[6].AstNode as CommentsNode).Comments;
            Offset = (parseNode.ChildNodes[7].AstNode as OffsetNode).Count;
            Limit = (parseNode.ChildNodes[8].AstNode as LimitNode).Count;
            VerbosityType = (parseNode.ChildNodes[9].AstNode as VerbosityNode).VerbosityType;

        }

        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {
            using (var transaction = graphDBSession.BeginTransaction())
            {
                var importResult = Importer.Import(SourceLocation, graphDBSession, ParallelTasks, Comments, Offset, Limit, VerbosityType);

                if (importResult.ResultType == Structures.ResultType.Successful)
                {
                    importResult.AddErrorsAndWarnings(transaction.Commit());
                }

                return importResult;
            }
        }
    }
}
