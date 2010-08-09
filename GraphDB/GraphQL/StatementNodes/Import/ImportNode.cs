
using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ImportExport;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.Structures.Result;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Structures.Enums;

namespace sones.GraphDB.GraphQL.StatementNodes.Import
{

    public class ImportNode : AStatement
    {

        #region Overrides

        public override string StatementName
        {
            get { return "Import"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region Properties

        public String ImportFormat { get; private set; }
        public String SourceLocation { get; private set; }
        public UInt32 ParallelTasks { get; private set; }
        public List<String> Comments { get; private set; }
        public UInt64? Offset { get; private set; }
        public UInt64? Limit { get; private set; }
        public VerbosityTypes VerbosityType { get; private set; }

        #endregion

        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            //var dbContext = context.IContext as DBContext;

            // parseNode.ChildNodes[0] - import symbol
            // parseNode.ChildNodes[1] - from symbol
            SourceLocation = parseNode.ChildNodes[2].Token.ValueString;
            // parseNode.ChildNodes[3] - format symbol

            ImportFormat = parseNode.ChildNodes[4].Token.Text;
            ParallelTasks = (parseNode.ChildNodes[5].AstNode as ParallelTasksNode).ParallelTasks;
            Comments = (parseNode.ChildNodes[6].AstNode as CommentsNode).Comments;
            Offset = (parseNode.ChildNodes[7].AstNode as OffsetNode).Count;
            Limit = (parseNode.ChildNodes[8].AstNode as LimitNode).Count;
            VerbosityType = (parseNode.ChildNodes[9].AstNode as VerbosityNode).VerbosityType;

        }

        public override QueryResult Execute(IGraphDBSession graphDBSession)
        {

            return graphDBSession.Import(ImportFormat, SourceLocation, ParallelTasks, Comments, Offset, Limit, VerbosityType);

        }

    }

}
