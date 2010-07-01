using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Imports;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;

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
            return Importer.Import(SourceLocation, graphDBSession, ParallelTasks, Comments, Offset, Limit, VerbosityType);
        }
    }
}
